# Quickstart: Main Menu Layout Editor

This quickstart describes the expected implementation and validation flow once the planned projects exist.

## Build

```bash
dotnet build StarSmuggler.sln
```

## Focused Automated Tests

```bash
dotnet test StarSmuggler.Tests/StarSmuggler.Tests.csproj
```

Expected focused coverage:

- JSON round-trip for `MenuLayoutDocument`
- Validation failures for missing ids, duplicate ids, invalid action names, zero or negative bounds, out-of-canvas rectangles, and overlapping enabled masks
- Runtime load result for valid layout
- Missing-layout fallback result
- Invalid-layout fallback result
- Coordinate scaling from 1536x1024 to at least one alternate viewport size

## Runtime Content Copy Check

Confirm `StarSmuggler.csproj` copies layout JSON files without MGCB processing:

```xml
<Content Include="Content/UI/MenuLayouts/*.json" CopyToOutputDirectory="PreserveNewest" />
```

Do not add `Content/UI/MenuLayouts/main-menu.json` to `Content/Content.mgcb`.

## Manual Editor Smoke Test

1. Start `StarSmuggler.Editor`.
2. Open `Content/UI/MainMenu.png`.
3. Associate the background with content asset key `UI/MainMenu`.
4. Add four button masks for `NewGame`, `LoadGame`, `SaveGame`, and `Quit`.
5. Move and resize masks with visible editor outlines.
6. Confirm bounds clamp inside the 1536x1024 canvas.
7. Confirm overlapping enabled masks block save/export with a validation error.
8. Save/export to `Content/UI/MenuLayouts/main-menu.json`.
9. Reopen the saved layout and verify all ids, bounds, actions, labels, enabled states, and background asset values persisted.
10. Confirm the JSON contains no absolute local paths.

## Manual Runtime Smoke Test

1. Run the game.
2. Confirm `MainMenuScreen` loads the exported layout without fallback warnings.
3. Verify visible text elements render with existing SpriteFont assets where possible.
4. Click each hit region and confirm:
   - `NewGame` starts a new game.
   - `LoadGame` uses existing load behavior.
   - `SaveGame` preserves existing active-game and stranded-game behavior.
   - `Quit` exits the game.
5. Temporarily rename or corrupt `Content/UI/MenuLayouts/main-menu.json`.
6. Run the game again and confirm the hardcoded menu works with a clear fallback warning.

## Final Validation

```bash
dotnet build StarSmuggler.sln
dotnet test StarSmuggler.Tests/StarSmuggler.Tests.csproj
git diff --check
```
