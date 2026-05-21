# Star Smuggler Menu Layout Editor Guide

This guide explains how to use the standalone main-menu layout editor to align text and invisible button hit regions for the Star Smuggler main menu.

The editor is an MVP focused on `Content/UI/MainMenu.png` and `Content/UI/MenuLayouts/main-menu.json`. It does not edit other screens yet.

## What The Editor Produces

The editor exports a JSON layout file at:

```text
Content/UI/MenuLayouts/main-menu.json
```

The game loads that file at runtime for `MainMenuScreen`. If the file is missing, malformed, unsupported, or invalid, the game logs a warning and uses the existing hardcoded main menu layout.

The exported layout stores:

- Layout version
- Canvas width and height
- Background asset key, such as `UI/MainMenu`
- Text elements
- Button mask elements

Coordinates are stored as source-canvas pixels on a 1536x1024 canvas. The game scales those coordinates to the current viewport when it renders and hit-tests the menu.

## Run The Editor

From the repository root:

```bash
dotnet run --project StarSmuggler.Editor/StarSmuggler.Editor.csproj
```

The editor uses Avalonia and is separate from the MonoGame runtime. The game project does not reference Avalonia.

## Main Window

The editor has three main areas:

- Toolbar: commands for opening the default image/layout, saving, and adding or deleting elements.
- Design surface: a scaled preview of the 1536x1024 main-menu canvas.
- Property panel: editable fields for the selected text box or button mask.

The current MVP toolbar commands use repo-default paths:

- `Open Image` opens `Content/UI/MainMenu.png`.
- `Open Layout` opens `Content/UI/MenuLayouts/main-menu.json`.
- `Save Layout` writes `Content/UI/MenuLayouts/main-menu.json`.

## Open The Main Menu Image

1. Start the editor.
2. Click `Open Image`.
3. Confirm the main-menu art appears on the design surface.
4. Confirm the toolbar shows the background asset key `UI/MainMenu`.

The JSON stores the content asset key, not the absolute local image path. For `Content/UI/MainMenu.png`, the stored runtime asset key is:

```text
UI/MainMenu
```

If the image is missing or unreadable, the validation panel shows an error and the current draft is left unchanged.

## Open An Existing Layout

Click `Open Layout` to load:

```text
Content/UI/MenuLayouts/main-menu.json
```

The editor validates the layout through the shared layout library. If the layout cannot be loaded, the validation panel shows the load problem.

## Add Text

Click `Add Text` to create a new text box. Select it on the design surface or keep the newly added selection active, then edit its properties in the property panel.

Text properties:

- `Id`: globally unique element id.
- `X`, `Y`, `Width`, `Height`: source-canvas pixel bounds.
- `Text`: displayed string.
- `Font Key`: MonoGame content font key, such as `Fonts/TerminalBold`.
- `Font Scale`: positive runtime scale value.
- `Font Color`: preset color options plus a custom hex value.
- `Color`: `#RRGGBB` or `#AARRGGBB` for the value saved to JSON.
- `Horizontal Alignment`: `Left`, `Center`, or `Right`.

Editor text preview is approximate. The MonoGame runtime SpriteFont rendering is authoritative.

The design surface previews text with the bundled Share Tech Mono font, the selected font color, horizontal alignment, and the configured font scale. This preview is intended for alignment; final runtime SpriteFont output can still differ slightly.

## Add A Button Mask

Click `Add Button Mask` to create an invisible runtime hit region. Button masks are outlined in the editor but are not drawn by the game.

Button mask properties:

- `Id`: globally unique element id.
- `X`, `Y`, `Width`, `Height`: source-canvas pixel bounds.
- `Action`: one of `NewGame`, `LoadGame`, `SaveGame`, or `Quit`.
- `Editor Label`: optional label for authoring context.
- `Enabled`: whether the mask is active at runtime.

Only enabled masks are used for hit testing. Disabled masks can remain in the layout for reference without triggering runtime actions.

## Select, Move, Resize, And Delete

To select an element, click its outline on the design surface. The selected element uses a stronger outline and shows a resize handle in the lower-right corner.

To move an element:

1. Select it.
2. Drag inside its rectangle.
3. Release the mouse.

To resize an element:

1. Select it.
2. Drag the lower-right resize handle.
3. Release the mouse.

Move and resize operations are clamped inside the 1536x1024 source canvas so exported rectangles remain valid.

To delete an element:

1. Select it.
2. Click `Delete`.

## Validation Rules

`Save Layout` validates the draft before writing JSON. Save is blocked when required rules fail.

Common validation errors:

- Missing or duplicate element ids.
- Width or height is zero or negative.
- Bounds extend outside the canvas.
- Background asset is missing or an absolute local path.
- Text font key, text, color, scale, or alignment is invalid.
- Button action is not one of `NewGame`, `LoadGame`, `SaveGame`, or `Quit`.
- Enabled button masks overlap.

Overlapping disabled masks are allowed because they do not participate in runtime hit testing.

## Save And Reopen

1. Add or edit text and button masks.
2. Click `Save Layout`.
3. Fix any validation messages if save is blocked.
4. Click `Open Layout`.
5. Confirm ids, positions, sizes, text, actions, labels, enabled states, and background asset values are preserved.

The JSON is written with deterministic PascalCase property names and indented formatting.

## Runtime Check

After saving a valid layout:

1. Run the game.
2. Confirm the main menu loads without a layout fallback warning.
3. Confirm text appears as expected.
4. Click each button mask:
   - `NewGame` starts a new game.
   - `LoadGame` uses the existing load behavior.
   - `SaveGame` uses the existing active-game and stranded-game save rules.
   - `Quit` exits the game.

The runtime caches loaded layout data, font lookups, and scaled rectangles. JSON and assets are not reloaded every frame.

## Fallback Check

To confirm the game remains safe when a layout is bad:

1. Temporarily rename or corrupt `Content/UI/MenuLayouts/main-menu.json`.
2. Run the game.
3. Confirm the hardcoded main menu still works.
4. Confirm the console logs a clear fallback warning.
5. Restore a valid `main-menu.json`.

## JSON Contract Notes

Do:

- Use content asset keys such as `UI/MainMenu` and `Fonts/TerminalBold`.
- Keep coordinates in 1536x1024 source-canvas pixels.
- Keep ids globally unique across text boxes and button masks.
- Keep enabled button masks non-overlapping.

Do not:

- Store absolute local paths in JSON.
- Add layout JSON files to `Content/Content.mgcb`.
- Add Avalonia references to the MonoGame runtime project.

The JSON files under `Content/UI/MenuLayouts/*.json` are copied to the output directory by MSBuild so the runtime can read them directly.

## Current MVP Limits

The editor currently focuses only on the main menu. These are intentionally out of scope for the MVP:

- Editing every game screen.
- Polygon or alpha-based masks.
- Animation timelines.
- Audio editing.
- Full content-pipeline management.
- Pixel-perfect editor font rendering.
- Action mapping beyond `NewGame`, `LoadGame`, `SaveGame`, and `Quit`.
