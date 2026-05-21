# Phase 0 Research: Main Menu Layout Editor

## Decision: Use .NET 8 across all new projects

**Rationale**: The existing game project already targets .NET 8. Keeping the shared layout library, editor, and tests on .NET 8 avoids cross-targeting friction and lets the runtime consume the shared contract directly.

**Alternatives considered**:

- Multi-target the shared library. Rejected because the current solution only needs .NET 8.
- Separate editor runtime version. Rejected because it increases dependency and build complexity without MVP value.

## Decision: Build the editor with Avalonia in `StarSmuggler.Editor`

**Rationale**: Avalonia gives a standalone cross-platform C# desktop UI without a browser or npm stack. It fits the requested authoring UI: image canvas, visible outlines, property panel, toolbar commands, drag-to-move, and resize handles.

**Alternatives considered**:

- Embed editor tools in MonoGame. Rejected because the feature explicitly requires a standalone desktop app.
- Browser or web frontend. Rejected because the feature explicitly excludes npm and a browser frontend.
- Platform-specific Windows UI. Rejected because the requested editor is cross-platform.

## Decision: Add `StarSmuggler.MenuLayouts` as the shared contract library

**Rationale**: Shared DTOs, serialization, validation, and coordinate scaling keep the editor and runtime on one contract. The MonoGame runtime can reference the shared library without referencing Avalonia.

**Alternatives considered**:

- Duplicate DTOs in game and editor. Rejected because it creates schema drift risk.
- Store layout parsing only in the editor. Rejected because runtime fallback and validation still need the same rules.

## Decision: Use `System.Text.Json` with explicit options and PascalCase property names

**Rationale**: The existing save/load code uses `System.Text.Json` and emits PascalCase by default. The requested DTO names are PascalCase, and explicit shared serializer options make the layout JSON deterministic and easy to test.

**Alternatives considered**:

- camelCase JSON. Rejected because it would differ from the existing save-file style and the requested DTO names.
- Newtonsoft.Json. Rejected because `System.Text.Json` is sufficient and avoids an extra dependency.

## Decision: Store coordinates as integer pixel values in the source canvas coordinate system

**Rationale**: The current menu code uses integer rectangles, and the design canvas is a pixel-based 1536x1024 source coordinate space. Integer coordinates keep JSON readable, deterministic, and directly testable.

**Alternatives considered**:

- Floating-point coordinates. Rejected because the MVP needs pixel alignment and current runtime rectangles are integer based.
- Normalized 0-1 coordinates. Rejected because the editor goal is to remove hardcoded pixel coordinates, not introduce a second coordinate system.

## Decision: Scale runtime rectangles independently on X and Y

**Rationale**: The current game draws the main menu background stretched to the full viewport. Independent scaling keeps text and hit masks aligned with that full-window background behavior when the viewport differs from 1536x1024.

**Alternatives considered**:

- Require exactly 1536x1024. Rejected because the clarified spec requires scaling.
- Uniform scale with letterboxing. Rejected because it would not match the current full-window background draw behavior.

## Decision: Treat invalid runtime layout files as fallback triggers

**Rationale**: The main menu must remain playable even if the layout file is missing, malformed, unsupported, or invalid. Runtime validation should return a clear load result and warning reason while preserving the hardcoded fallback.

**Alternatives considered**:

- Crash or block startup. Rejected because the menu is the game entry point.
- Partially load invalid layouts. Rejected because unsupported actions, duplicate ids, or overlapping masks create ambiguous behavior.

## Decision: Copy layout JSON through MSBuild content items, not MGCB

**Rationale**: Layout JSON is runtime configuration, not a MonoGame content pipeline asset. Copying `Content/UI/MenuLayouts/*.json` to output keeps files readable, editable, and testable while avoiding MGCB processing.

**Alternatives considered**:

- Add JSON to `Content.mgcb`. Rejected because it would treat editor-authored configuration as pipeline content.
- Read only from source tree. Rejected because packaged builds need the file copied beside runtime content.

## Decision: Test shared contract and loader behavior with xUnit

**Rationale**: JSON round trips, validation failures, coordinate scaling, and missing/invalid fallback behavior are deterministic and practical to automate. Editor drag and resize behavior still needs documented manual smoke validation.

**Alternatives considered**:

- Manual-only validation. Rejected because the layout contract and fallback behavior are regression-prone.
- UI automation for Avalonia MVP. Deferred because the current MVP can be protected by shared contract tests plus manual editor smoke tests.
