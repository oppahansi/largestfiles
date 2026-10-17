# Changelog

All notable changes to this project are documented here.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and the project uses [semantic versioning](https://semver.org).

## [Unreleased]

## [1.0.1] - 2026-10-17

### Changed

- The bar on each library card now shows the library's share of all libraries combined, matching the percentage next to it. Before, it was relative to the largest library.

### Removed

- Collections libraries are no longer listed. They only group items stored in other libraries and have no files of their own.

## [1.0.0] - 2026-10-17

### Added

- Dashboard page listing media files by size, filterable by library.
- Per-library cards with total size, share of the server, file count, average size and largest file.
- Largest series per library and size breakdown by file type.
- Admin-only JSON endpoint `GET /LargestFiles/Report`.
- Builds for Jellyfin 10.9, 10.10, 10.11, 12.0, 12.1 and 12.2.

[Unreleased]: https://github.com/oppahansi/largestfiles/compare/v1.0.1-jellyfin-12.2...HEAD
[1.0.1]: https://github.com/oppahansi/largestfiles/compare/v1.0.0-jellyfin-12.2...v1.0.1-jellyfin-12.2
[1.0.0]: https://github.com/oppahansi/largestfiles/releases/tag/v1.0.0-jellyfin-12.2
