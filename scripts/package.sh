#!/usr/bin/env bash
# Builds the plugin for one Jellyfin version and packages it as an installable zip.
#
#   scripts/package.sh <jellyfin-version> [plugin-version]
#   scripts/package.sh 10.10.7 1.0.0
#
# Output: dist/largestfiles_<plugin-version>_jellyfin-<major.minor>.zip
set -euo pipefail

JELLYFIN_VERSION="${1:?usage: package.sh <jellyfin-version> [plugin-version]}"
PLUGIN_VERSION="${2:-1.0.0}"

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
PROJECT="$ROOT/Jellyfin.Plugin.LargestFiles"
IFS=. read -r MAJOR MINOR _ <<< "$JELLYFIN_VERSION"
LINE="$MAJOR.$MINOR"
FULL_VERSION="$PLUGIN_VERSION.$((MAJOR * 100 + MINOR))"
OUT="$ROOT/artifacts/jellyfin-$LINE"
STAGE="$OUT/stage"
ZIP="$ROOT/dist/largestfiles_${PLUGIN_VERSION}_jellyfin-$LINE.zip"

rm -rf "$OUT"
mkdir -p "$STAGE" "$ROOT/dist"

dotnet publish "$PROJECT" -c Release \
  -p:JellyfinVersion="$JELLYFIN_VERSION" \
  -p:PluginVersion="$PLUGIN_VERSION" \
  -o "$OUT/publish" \
  --nologo -v quiet

cp "$OUT/publish/Jellyfin.Plugin.LargestFiles.dll" "$STAGE/"

cat > "$STAGE/meta.json" <<JSON
{
  "category": "General",
  "changelog": "See https://github.com/oppahansi/largestfiles/blob/main/CHANGELOG.md",
  "description": "Lists media files sorted by size and shows per-library statistics: file count, total and average size, largest file, largest series and size by file type.",
  "guid": "645064fc-7b74-4de2-afe2-56f4cd5a5c45",
  "name": "Largest Files",
  "overview": "See the largest files in your libraries and per-library storage statistics",
  "owner": "oppahansi",
  "targetAbi": "$LINE.0.0",
  "timestamp": "$(date -u +%Y-%m-%dT%H:%M:%SZ)",
  "version": "$FULL_VERSION",
  "status": "Active",
  "autoUpdate": true,
  "assemblies": []
}
JSON

rm -f "$ZIP"
(cd "$STAGE" && zip -q -X "$ZIP" Jellyfin.Plugin.LargestFiles.dll meta.json)
echo "$ZIP"
