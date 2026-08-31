# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.4] - 2026-08-31

### Fixed
- The `.addin` manifest now declares the real version. Every release since 1.0.1 shipped a
  manifest still saying `version="1.0"`, so Addin Finder offered an update that installing
  could never clear. A Release-build check (`CheckAddinVersion`) now fails the build when the
  manifest and project version disagree, so this cannot happen again.

## [1.0.3] - 2026-03-03

### Added
- Icon displayed in Edit → Format menu and Embeditor context menu

## [1.0.2] - 2026-02-26

### Changed
- Context menu item now appears only once (removed explicit context menu registration;
  the main menu Edit→Format entry surfaces in the context menu naturally)
- Context menu item and shortcut are hidden entirely when the caret is not inside or
  adjacent to a continuation block — they no longer appear on unrelated code
- Removed menu separators

### Fixed
- Condition evaluator now uses `ActiveWorkbenchWindow.ActiveViewContent` instead of
  `ActiveContent`, which could lose the editor reference when a context menu opened

### Restored
- `Ctrl+Shift+F` shortcut (was accidentally removed in an earlier fix)

## [1.0.1] - 2026-02-26

### Fixed
- Doubled-quote escape sequences (`''`) inside Clarion string literals are now
  handled correctly. Previously a `|` character appearing after a `''` pair
  (e.g. `PROMPT('Don''t fear'),AT(1,1), |`) could be misidentified as a
  continuation pipe, causing incorrect flattening.

## [1.0.0] - 2026-02-26

### Added
- Initial release
- Flattens Clarion line continuations (`|`) into single logical lines
- Collapses adjacent string literals joined with `&` (e.g. `'abc' & 'def'` → `'abcdef'`)
- Operates on selected text or entire document
- Caret is restored to the start of the continuation group it was in before flattening
- `Ctrl+Shift+F` shortcut registered in Edit → Format menu
- Available in CLW source editor context menu
- Available in Embeditor context menu (writable embed regions only)
- Compatible with Clarion 10, 11, 11.1, and 12

[1.0.4]: https://github.com/msarson/FlattenCode/compare/v1.0.3...v1.0.4
[1.0.3]: https://github.com/msarson/FlattenCode/compare/v1.0.2...v1.0.3
[1.0.2]: https://github.com/msarson/FlattenCode/compare/v1.0.1...v1.0.2
[1.0.1]: https://github.com/msarson/FlattenCode/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/msarson/FlattenCode/releases/tag/v1.0.0
