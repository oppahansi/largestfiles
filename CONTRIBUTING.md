# Contributing

Thanks for helping improve Largest Files. Bug reports, feature ideas and pull requests are all welcome.

## Reporting bugs

Open an [issue](https://github.com/oppahansi/largestfiles/issues/new/choose) and include:

- your Jellyfin version and how it runs (Docker, Linux package, Windows, …)
- the plugin version, shown under **Dashboard → Plugins**
- what you expected and what happened instead
- related lines from the Jellyfin log, if any (**Dashboard → Logs**)

Please remove file paths or library names you don't want to share.

## Development setup

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download).
2. Clone the repository and build:

   ```sh
   dotnet build Jellyfin.Plugin.LargestFiles -c Release
   ```

3. To try your build, copy `Jellyfin.Plugin.LargestFiles/bin/Release/<framework>/Jellyfin.Plugin.LargestFiles.dll` into a folder under your test server's `plugins/` directory and restart Jellyfin. A throwaway Docker container works well for this:

   ```sh
   docker run --rm -p 8096:8096 \
     -v "$PWD/test-config:/config" \
     -v /path/to/some/media:/media:ro \
     jellyfin/jellyfin:10.11.11
   ```

   Build with `-p:JellyfinVersion=<version>` that matches the server image. See [Building](README.md#building).

## Project layout

| Path | Contents |
|---|---|
| `Jellyfin.Plugin.LargestFiles/Plugin.cs` | Plugin entry point and dashboard page registration |
| `Jellyfin.Plugin.LargestFiles/Api/` | The `/LargestFiles/Report` controller and response models |
| `Jellyfin.Plugin.LargestFiles/Web/largestFiles.html` | The dashboard page (HTML, CSS and JavaScript in one file) |
| `targets.json` | Jellyfin versions that get a release build |
| `scripts/package.sh` | Builds and zips one Jellyfin version |
| `scripts/update-manifest.py` | Updates `manifest.json`, the Jellyfin plugin repository file |

## Pull requests

- Keep changes focused. Open an issue first for larger features so we can agree on the approach.
- Make sure every supported Jellyfin version still builds. CI checks this on every pull request, and locally you can run:

  ```sh
  for v in $(jq -r '.[]' targets.json); do scripts/package.sh "$v"; done
  ```

- If the Jellyfin API differs between versions, use the built-in target framework symbols (`#if NET9_0_OR_GREATER`, `#if NET10_0_OR_GREATER`) instead of dropping support for older versions.
- Warnings are treated as errors. Match the existing code style; `.editorconfig` covers the basics.
- Add an entry under `## [Unreleased]` in `CHANGELOG.md` for user-facing changes.
- Don't edit `manifest.json` by hand. The release workflow maintains it.

## License

By contributing, you agree that your contributions are licensed under the [MIT License](LICENSE).
