# Implementation Plan: Main Menu Layout Editor

**Branch**: `001-menu-layout-editor` | **Date**: 2026-05-21 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/001-menu-layout-editor/spec.md`

## Summary

Build a standalone main-menu layout editor for Star Smuggler and teach the existing MonoGame `MainMenuScreen` to consume the exported JSON layout at runtime. The solution will add a shared `StarSmuggler.MenuLayouts` class library for DTOs, JSON serialization, validation, coordinate conversion helpers, and load-result modeling; an Avalonia-only `StarSmuggler.Editor` desktop app for authoring the 1536x1024 menu layout; and an xUnit `StarSmuggler.Tests` project for contract, validation, coordinate-scaling, and runtime-loader fallback behavior. The current game project remains the runtime owner and preserves the hardcoded main menu fallback when JSON is missing or invalid.

## Technical Context

**Language/Version**: C# on .NET 8
**Primary Dependencies**: Existing MonoGame 3.8 runtime project, Avalonia for `StarSmuggler.Editor` only, `System.Text.Json` for layout serialization, xUnit for tests
**Storage**: Repository-local JSON layout file at `Content/UI/MenuLayouts/main-menu.json`; background image remains a game content asset key such as `UI/MainMenu`
**Testing**: xUnit focused tests in `StarSmuggler.Tests`, solution build via `dotnet build StarSmuggler.sln`, manual editor/game smoke tests, `git diff --check`
**Target Platform**: Cross-platform desktop editor; existing MonoGame desktop runtime remains the game target
**Project Type**: Multi-project desktop solution with an existing game executable, a shared class library, a standalone editor executable, and a test project
**Performance Goals**: Runtime loads and validates layout data during screen/content setup or refresh, not in `Draw` or per-frame `Update`; editor drag/resize reuses loaded image data and remains responsive on the 1536x1024 menu image
**Constraints**: No npm or browser frontend; Avalonia must not be referenced by the MonoGame runtime; no absolute local paths in JSON; preserve playable hardcoded main menu fallback; JSON files under `Content/UI/MenuLayouts/*.json` are copied to output without MGCB processing
**Scale/Scope**: One menu layout, one background asset, rectangular text boxes, rectangular button masks, four button actions, one exported JSON contract

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Code Quality & Comments**: PASS. The plan isolates the layout schema and validation in a shared library, keeps Avalonia out of the runtime, and requires intent-level comments for schema versioning, JSON property assumptions, runtime fallback paths, coordinate scaling, and editor selection/drag behavior.
- **TDD**: PASS. Implementation must start with failing JSON round-trip, validation, missing/invalid loader fallback, and coordinate-scaling tests. Manual red/green coverage is required for the Avalonia editor interactions that are not practical to automate in this repo.
- **Testing Standards**: PASS. Required validation is `dotnet build StarSmuggler.sln`, focused xUnit tests for the new test project, `git diff --check`, and documented manual smoke tests for opening art, placing four masks, saving, reopening, running the game, and dispatching all four actions.
- **UX Consistency**: PASS. The editor must provide predictable selection outlines, drag/resize handles, property editing, validation errors, and save/export feedback. The game must preserve existing new/load/save/quit behavior and hardcoded fallback.
- **Performance**: PASS. Runtime layout and asset loading are lifecycle work, not per-frame work. Drawing and hit-testing use cached validated layout data and scaled rectangles. Editor drag/resize must not reload images.

## Project Structure

### Documentation (this feature)

```text
specs/001-menu-layout-editor/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── main-menu-layout.schema.json
└── tasks.md
```

### Source Code (repository root)

```text
StarSmuggler.csproj
StarSmuggler.sln
Screens/
└── MainMenuScreen.cs
Content/
└── UI/
    └── MenuLayouts/
        └── main-menu.json

StarSmuggler.MenuLayouts/
├── StarSmuggler.MenuLayouts.csproj
├── MenuLayoutDocument.cs
├── MenuLayoutElement.cs
├── MenuLayoutJson.cs
├── MenuLayoutValidator.cs
├── MenuLayoutLoader.cs
└── CoordinateScaler.cs

StarSmuggler.Editor/
├── StarSmuggler.Editor.csproj
├── Program.cs
├── App.axaml
├── App.axaml.cs
├── ViewModels/
│   ├── MainWindowViewModel.cs
│   ├── LayoutElementViewModel.cs
│   └── ValidationMessageViewModel.cs
├── Views/
│   ├── MainWindow.axaml
│   ├── DesignSurface.axaml
│   └── PropertyPanel.axaml
└── Services/
    ├── ImageImportService.cs
    ├── LayoutFileService.cs
    └── EditorCoordinateService.cs

StarSmuggler.Tests/
├── StarSmuggler.Tests.csproj
├── MenuLayouts/
│   ├── MenuLayoutJsonTests.cs
│   ├── MenuLayoutValidationTests.cs
│   └── CoordinateScalerTests.cs
└── Runtime/
    └── MainMenuLayoutLoaderTests.cs
```

**Structure Decision**: Use a multi-project solution. The existing `StarSmuggler` project remains the MonoGame runtime and references `StarSmuggler.MenuLayouts`. `StarSmuggler.Editor` references `StarSmuggler.MenuLayouts` and Avalonia. `StarSmuggler.Tests` references `StarSmuggler.MenuLayouts` and, for pure loader behavior, the runtime project or a runtime-facing loader abstraction that does not require graphics devices.

## Phase 0 Research Summary

Research decisions are captured in [research.md](./research.md). Key outcomes:

- Use Avalonia only in the standalone editor project.
- Use a shared contract library so editor and game cannot drift.
- Use `System.Text.Json` with explicit options and PascalCase JSON property names aligned with DTO names and the existing save-file style.
- Store source-canvas coordinates as integer pixel values.
- Validate layouts before save/export and before runtime use.
- Copy layout JSON files as content files through MSBuild rather than MGCB.

## Phase 1 Design Summary

Design artifacts are captured in:

- [data-model.md](./data-model.md)
- [contracts/main-menu-layout.schema.json](./contracts/main-menu-layout.schema.json)
- [quickstart.md](./quickstart.md)

Post-design constitution re-check: PASS. The design preserves the shared contract boundary, keeps Avalonia isolated to the editor, explicitly covers fallback and coordinate conversion comments, and defines automated plus manual validation paths.

## Implementation Notes

- `StarSmuggler.MenuLayouts` owns DTOs, JSON serialization options, validation, coordinate scaling, and file load results. This keeps runtime and editor behavior aligned without coupling the game to Avalonia.
- `MenuLayoutDocument.Version` starts at `1`. Version handling should be commented where the schema is defined and where runtime fallback rejects unsupported versions.
- JSON uses deterministic PascalCase property names: `Version`, `CanvasWidth`, `CanvasHeight`, `BackgroundAsset`, `Elements`, `Type`, `Id`, `X`, `Y`, `Width`, `Height`, and type-specific fields.
- `Elements` uses a `Type` discriminator with `Text` and `ButtonMask`.
- Runtime text rendering should map `FontKey` to existing SpriteFont assets where possible and fall back to a known menu font with a warning if a layout references an unknown font.
- Runtime fallback must distinguish at least missing file, invalid JSON, invalid schema/content, unsupported action, unsupported version, overlapping enabled masks, and unavailable background/font assets where practical.
- `MainMenuScreen` should load/validate layout data once during `LoadContent` or `Refresh`, cache scaled rectangles for the current viewport, and recompute scaling only when viewport dimensions change.
- `StarSmuggler.csproj` should include `Content/UI/MenuLayouts/*.json` as copied content without adding those files to `Content/Content.mgcb`.
- The editor stores repo-relative paths or content asset keys only. If a user opens a file by absolute path, the save/export path must normalize to `UI/MainMenu` or a repo-relative asset reference before writing JSON.

## Complexity Tracking

No constitution violations or complexity exceptions are required.
