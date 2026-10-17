using System;
using System.Collections.Generic;

namespace Jellyfin.Plugin.LargestFiles.Api;

/// <summary>
/// A single media file and its size.
/// </summary>
public class FileEntry
{
    /// <summary>Gets or sets the item id.</summary>
    public Guid ItemId { get; set; }

    /// <summary>Gets or sets the display name, e.g. "Show - S01E02 - Title".</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the item type, e.g. Movie or Episode.</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Gets or sets the series name for episodes.</summary>
    public string? Series { get; set; }

    /// <summary>Gets or sets the file path.</summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>Gets or sets the file extension without the dot.</summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>Gets or sets the size in bytes.</summary>
    public long Size { get; set; }

    /// <summary>Gets or sets the library name.</summary>
    public string Library { get; set; } = string.Empty;
}

/// <summary>
/// A named group of files with a summed size (a series, a file type).
/// </summary>
public class SizeGroup
{
    /// <summary>Gets or sets the group name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the number of files in the group.</summary>
    public int FileCount { get; set; }

    /// <summary>Gets or sets the total size in bytes.</summary>
    public long TotalSize { get; set; }
}

/// <summary>
/// Statistics for one library.
/// </summary>
public class LibraryStats
{
    /// <summary>Gets or sets the library id.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the library name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the collection type, e.g. movies or tvshows.</summary>
    public string? CollectionType { get; set; }

    /// <summary>Gets or sets the number of files.</summary>
    public int FileCount { get; set; }

    /// <summary>Gets or sets the total size in bytes.</summary>
    public long TotalSize { get; set; }

    /// <summary>Gets or sets the average file size in bytes.</summary>
    public long AverageSize { get; set; }

    /// <summary>Gets or sets the largest file in the library.</summary>
    public FileEntry? LargestFile { get; set; }

    /// <summary>Gets or sets the largest series by summed episode size.</summary>
    public IReadOnlyList<SizeGroup> TopSeries { get; set; } = [];

    /// <summary>Gets or sets the size per file extension.</summary>
    public IReadOnlyList<SizeGroup> Extensions { get; set; } = [];
}

/// <summary>
/// The full report returned by the API.
/// </summary>
public class LargestFilesReport
{
    /// <summary>Gets or sets when the report was generated.</summary>
    public DateTime GeneratedAt { get; set; }

    /// <summary>Gets or sets the total number of files across all libraries.</summary>
    public int TotalFiles { get; set; }

    /// <summary>Gets or sets the total size across all libraries in bytes.</summary>
    public long TotalSize { get; set; }

    /// <summary>Gets or sets the per-library statistics, largest library first.</summary>
    public IReadOnlyList<LibraryStats> Libraries { get; set; } = [];

    /// <summary>Gets or sets the largest files across all libraries, largest first.</summary>
    public IReadOnlyList<FileEntry> Files { get; set; } = [];
}
