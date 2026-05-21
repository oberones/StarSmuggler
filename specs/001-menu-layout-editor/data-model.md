# Data Model: Main Menu Layout Editor

## MenuLayoutDocument

Represents one exported main-menu layout.

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Version` | integer | Yes | Starts at `1`; unsupported versions are invalid at runtime and in the editor. |
| `CanvasWidth` | integer | Yes | Source canvas width, expected `1536` for MVP. |
| `CanvasHeight` | integer | Yes | Source canvas height, expected `1024` for MVP. |
| `BackgroundAsset` | string | Yes | Game content asset key or repo-relative path, default `UI/MainMenu`; absolute paths are invalid. |
| `Elements` | array of layout elements | Yes | Ordered list of `Text` and `ButtonMask` elements. |

### Validation Rules

- `Version` must be supported by the shared library.
- `CanvasWidth` and `CanvasHeight` must be positive.
- `BackgroundAsset` must be non-empty and must not be an absolute local path.
- `Elements` must not contain duplicate ids across any element type.
- All element rectangles must have positive `Width` and `Height`.
- All element rectangles must be inside the source canvas.
- Enabled button masks must not overlap each other.

## MenuLayoutElement

Shared element fields for every record in `Elements`.

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Type` | enum/string | Yes | `Text` or `ButtonMask`. |
| `Id` | string | Yes | Globally unique within the document. |
| `X` | integer | Yes | Pixel coordinate in the source canvas. |
| `Y` | integer | Yes | Pixel coordinate in the source canvas. |
| `Width` | integer | Yes | Pixel width in the source canvas. |
| `Height` | integer | Yes | Pixel height in the source canvas. |

### Validation Rules

- `Type` must be supported.
- `Id` must be non-empty after trimming.
- `X` and `Y` may be zero or positive.
- `Width` and `Height` must be greater than zero.
- `X + Width` must be less than or equal to `CanvasWidth`.
- `Y + Height` must be less than or equal to `CanvasHeight`.

## TextElement

Represents visible menu text rendered by the game and approximated by the editor.

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Text` | string | Yes | Display text; empty or whitespace-only text is invalid. |
| `FontKey` | string | Yes | Existing SpriteFont asset key where possible, such as `Fonts/TerminalBold`. |
| `FontScale` | number | Yes | Positive scale applied to the runtime SpriteFont. |
| `Color` | string | Yes | Hex color, preferably `#RRGGBB` or `#AARRGGBB`. |
| `HorizontalAlignment` | enum/string | Yes | `Left`, `Center`, or `Right`. |

### Validation Rules

- `FontKey` must be non-empty.
- `FontScale` must be greater than zero.
- `Color` must parse as a supported hex color.
- `HorizontalAlignment` must be supported.

## ButtonMaskElement

Represents an invisible runtime hit region with visible editor outline.

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Action` | enum/string | Yes | `NewGame`, `LoadGame`, `SaveGame`, or `Quit`. |
| `Label` | string | No | Editor-only annotation; never rendered by the game. |
| `Enabled` | boolean | Yes | Disabled masks are saved but ignored by runtime hit testing. |

### Validation Rules

- `Action` must be one of the four supported MVP actions.
- `Label` may be empty or omitted.
- Enabled button masks must not overlap other enabled button masks.

## MenuButtonAction

Supported action values:

- `NewGame`
- `LoadGame`
- `SaveGame`
- `Quit`

Unknown action strings are invalid and trigger runtime fallback.

## MenuLayoutValidationResult

Represents validation output from the shared library.

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `IsValid` | boolean | Yes | True only when no validation issues exist. |
| `Issues` | array | Yes | One or more validation issues when invalid. |

## MenuLayoutValidationIssue

Represents one validation problem.

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Code` | string | Yes | Stable issue code for tests and editor display. |
| `Message` | string | Yes | Human-readable validation message. |
| `ElementId` | string | No | Element id when the issue is element-specific. |

## MenuLayoutLoadResult

Represents runtime/editor file loading outcome.

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Loaded` | boolean | Yes | True only when file read, JSON parse, and validation succeed. |
| `Document` | `MenuLayoutDocument` | No | Present only when `Loaded` is true. |
| `FallbackReason` | enum/string | No | Missing file, invalid JSON, invalid layout, unsupported version, unsupported action, or IO error. |
| `WarningMessage` | string | No | Clear message suitable for console logging or editor feedback. |

## State Transitions

### Editor Document Lifecycle

```text
No document
-> image opened/imported
-> editable draft layout
-> validation requested before save/export
-> saved JSON layout
-> reopened JSON layout
```

Invalid drafts remain editable. Save/export is blocked until validation passes.

### Runtime Layout Lifecycle

```text
MainMenuScreen.LoadContent or Refresh
-> attempt to load Content/UI/MenuLayouts/main-menu.json
-> parse JSON
-> validate schema and layout rules
-> cache layout and source-to-viewport scaling data
-> render/hit-test cached layout
```

Any failure from file load, parse, validation, unsupported version, unsupported action, unavailable critical asset, or overlapping enabled masks transitions to hardcoded fallback mode with a clear warning.
