using System;
using System.Collections.Generic;
using Jellyfin.Plugin.LargestFiles.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.LargestFiles;

/// <summary>
/// The Largest Files plugin.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
    /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    /// <inheritdoc />
    public override string Name => "Largest Files";

    /// <inheritdoc />
    public override Guid Id => Guid.Parse("645064fc-7b74-4de2-afe2-56f4cd5a5c45");

    /// <inheritdoc />
    public override string Description => "Shows the largest files in your libraries plus per-library storage statistics.";

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = "LargestFiles",
                DisplayName = "Largest Files",
                EmbeddedResourcePath = GetType().Namespace + ".Web.largestFiles.html",
                EnableInMainMenu = true,
                MenuSection = "server",
                MenuIcon = "storage"
            }
        ];
    }
}
