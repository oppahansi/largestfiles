using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.LargestFiles.Api;

/// <summary>
/// Serves the largest-files report. Admin only.
/// </summary>
[ApiController]
[Authorize(Policy = "RequiresElevation")]
[Route("LargestFiles")]
[Produces("application/json")]
public class LargestFilesController : ControllerBase
{
    private const int TopGroupCount = 10;

    private readonly ILibraryManager _libraryManager;
    private readonly ILogger<LargestFilesController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LargestFilesController"/> class.
    /// </summary>
    /// <param name="libraryManager">Instance of the <see cref="ILibraryManager"/> interface.</param>
    /// <param name="logger">Instance of the <see cref="ILogger{T}"/> interface.</param>
    public LargestFilesController(ILibraryManager libraryManager, ILogger<LargestFilesController> logger)
    {
        _libraryManager = libraryManager;
        _logger = logger;
    }

    /// <summary>
    /// Builds the report: per-library statistics and the largest files.
    /// </summary>
    /// <param name="limit">How many files to return in the overall list.</param>
    /// <param name="libraryId">Only include this library in the file list.</param>
    /// <returns>The report.</returns>
    [HttpGet("Report")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<LargestFilesReport> GetReport([FromQuery] int limit = 100, [FromQuery] Guid? libraryId = null)
    {
        limit = Math.Clamp(limit, 1, 5000);

        var libraries = new List<LibraryStats>();
        var allFiles = new List<FileEntry>();

        foreach (var folder in _libraryManager.GetVirtualFolders())
        {
            var collectionType = folder.CollectionType?.ToString();

            // Collections only group items that live in other libraries, they have no files of their own.
            if (!Guid.TryParse(folder.ItemId, out var id) || string.Equals(collectionType, "boxsets", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var files = GetFiles(id, folder.Name);
            libraries.Add(BuildStats(id, folder.Name, collectionType, files));

            if (libraryId is null || libraryId == id)
            {
                allFiles.AddRange(files);
            }
        }

        return new LargestFilesReport
        {
            GeneratedAt = DateTime.UtcNow,
            TotalFiles = libraries.Sum(l => l.FileCount),
            TotalSize = libraries.Sum(l => l.TotalSize),
            Libraries = libraries.OrderByDescending(l => l.TotalSize).ToList(),
            Files = allFiles.OrderByDescending(f => f.Size).Take(limit).ToList()
        };
    }

    private List<FileEntry> GetFiles(Guid libraryId, string libraryName)
    {
        var items = _libraryManager.GetItemList(new InternalItemsQuery
        {
            ParentId = libraryId,
            Recursive = true,
            IsFolder = false,
            IsVirtualItem = false
        });

        var files = new List<FileEntry>();
        // Multi-episode files map several items to one path, count each file once.
        var seenPaths = new HashSet<string>(StringComparer.Ordinal);

        foreach (var item in items)
        {
            if (string.IsNullOrEmpty(item.Path) || !seenPaths.Add(item.Path))
            {
                continue;
            }

            var size = GetSize(item);
            if (size <= 0)
            {
                continue;
            }

            files.Add(new FileEntry
            {
                ItemId = item.Id,
                Name = GetDisplayName(item),
                Type = item.GetBaseItemKind().ToString(),
                Series = (item as Episode)?.SeriesName,
                Path = item.Path,
                Extension = System.IO.Path.GetExtension(item.Path).TrimStart('.').ToLowerInvariant(),
                Size = size,
                Library = libraryName
            });
        }

        return files;
    }

    private long GetSize(BaseItem item)
    {
        if (item.Size is > 0)
        {
            return item.Size.Value;
        }

        // Size is filled in during scans, fall back to the file system for items that lack it.
        try
        {
            var info = new FileInfo(item.Path);
            return info.Exists ? info.Length : 0;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            _logger.LogDebug(ex, "Could not read size of {Path}", item.Path);
            return 0;
        }
    }

    private static string GetDisplayName(BaseItem item)
    {
        if (item is Episode episode)
        {
            var number = episode.ParentIndexNumber is not null && episode.IndexNumber is not null
                ? $"S{episode.ParentIndexNumber:00}E{episode.IndexNumber:00}"
                : null;
            return string.Join(" - ", new[] { episode.SeriesName, number, episode.Name }.Where(s => !string.IsNullOrEmpty(s)));
        }

        return item.ProductionYear is not null ? $"{item.Name} ({item.ProductionYear})" : item.Name;
    }

    private static LibraryStats BuildStats(Guid id, string name, string? collectionType, List<FileEntry> files)
    {
        var total = files.Sum(f => f.Size);

        return new LibraryStats
        {
            Id = id,
            Name = name,
            CollectionType = collectionType,
            FileCount = files.Count,
            TotalSize = total,
            AverageSize = files.Count > 0 ? total / files.Count : 0,
            LargestFile = files.MaxBy(f => f.Size),
            TopSeries = Group(files.Where(f => !string.IsNullOrEmpty(f.Series)), f => f.Series!).Take(TopGroupCount).ToList(),
            Extensions = Group(files, f => string.IsNullOrEmpty(f.Extension) ? "(none)" : f.Extension).ToList()
        };
    }

    private static IEnumerable<SizeGroup> Group(IEnumerable<FileEntry> files, Func<FileEntry, string> key)
    {
        return files
            .GroupBy(key)
            .Select(g => new SizeGroup { Name = g.Key, FileCount = g.Count(), TotalSize = g.Sum(f => f.Size) })
            .OrderByDescending(g => g.TotalSize);
    }
}
