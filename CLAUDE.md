# CLAUDE.md - Star Smuggler Project Guide

## Project Overview

Star Smuggler is a retro-futuristic space-trading game inspired by *Dope Wars*.
The playable runtime uses C#/.NET 8 and MonoGame DesktopGL. The repository also
contains an Avalonia desktop editor for the main-menu layout, a shared layout
contract library, and an xUnit test project.

Windows is the primary development target. MonoGame DesktopGL and the Avalonia
editor are intended to remain cross-platform.

## Solution Structure

The solution contains four .NET 8 projects:

- `StarSmuggler.csproj`: MonoGame runtime.
- `StarSmuggler.MenuLayouts/`: shared menu-layout DTOs, JSON serialization,
  validation, loading, and coordinate scaling.
- `StarSmuggler.Editor/`: standalone Avalonia main-menu layout editor. Avalonia
  dependencies belong here and must not be added to the game runtime.
- `StarSmuggler.Tests/`: xUnit tests for layout contracts, validation, scaling,
  editor services, and runtime fallback behavior.

Important directories and files:

```text
StarSmuggler/
|-- Audio/                         # AudioManager
|-- Content/                       # MonoGame assets and Content.mgcb
|   |-- Fonts/
|   |-- FX/
|   |-- Music/
|   |-- Ports/
|   |-- Screens/
|   |-- Trade/
|   `-- UI/MenuLayouts/            # Runtime-readable layout JSON
|-- Events/                        # GameEvent and EventDatabase
|-- Factions/                      # Faction definitions/future work
|-- Items/                         # Item and ItemsDatabase
|-- Player/                        # PlayerData
|-- Ports/                         # Port and PortsDatabase
|-- Screens/                       # Runtime screens and ScreenManager
|-- UI/                            # Reusable MonoGame UI components
|-- StarSmuggler.Editor/           # Avalonia layout editor
|-- StarSmuggler.MenuLayouts/      # Shared layout library
|-- StarSmuggler.Tests/            # Automated tests
|-- docs/                          # Project and editor documentation
|-- specs/001-menu-layout-editor/  # Feature design artifacts
|-- Game1.cs                       # MonoGame entry point and registration
|-- GameManager.cs                 # Gameplay state and economy coordinator
`-- SaveLoadManager.cs             # JSON persistence
```

## Runtime Architecture

### Game and Screen State

`GameManager.Instance` is the central authority for the active `PlayerData`,
game state, prices, travel, random events, and game-over routing. Keep state
changes going through its public behavior rather than duplicating rules in a
screen.

`ScreenManager` maps `GameState` values to `IScreen` implementations. `Game1`
currently registers:

- `MainMenuScreen`
- `PortOverviewScreen`
- `TradeScreen`
- `TravelScreen`
- `TravelAnimationScreen`
- `GameOverScreen`

Every screen implements:

```csharp
public interface IScreen
{
    void LoadContent(GraphicsDevice graphics, ContentManager content);
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch);
    void Refresh(ContentManager content);
}
```

To add a screen, implement `IScreen`, add a `GameState` value, and register the
screen in `Game1.LoadContent()`.

### Main-Menu Layout

`MainMenuScreen` loads `Content/UI/MenuLayouts/main-menu.json` through the
shared layout library. The JSON uses source coordinates for a 1536x1024 canvas;
the runtime scales them for drawing and hit testing. Missing or invalid layout
data falls back to the hardcoded menu so the game remains playable.

The standalone editor authors this one layout. See
`docs/MENU_LAYOUT_EDITOR_GUIDE.md` and
`specs/001-menu-layout-editor/plan.md`. Layout JSON is copied to the output by
MSBuild and must not be added to `Content/Content.mgcb`.

### Trading and Travel

- A new player starts with 500 credits, a 30-unit cargo limit, no cargo, and a
  random Inner-zone port.
- Ports are grouped into `Inner`, `Outer`, and `Fringe` zones.
- A port stocks four zone-appropriate items and two cross-zone items.
- Prices initialize at game start and update after more than three completed
  jumps. Price variance is combined with rarity/zone markup.
- Travel costs 15 credits plus 2 per zone crossed. Crossing two zones doubles
  that subtotal.
- Travel runs through `TravelAnimationScreen`; completion applies the actual
  port change, cost, price update, and possible event.
- Each completed trip has a 30% chance to trigger an event.
- The game ends when the player has fewer than 15 credits and cannot sell enough
  locally available cargo to reach 15 credits.

Current ports are Mercury, Venus, Luna, Mars, Ceres, Europa, Titan, Pluto, and
the Kuiper Flotilla. Eris assets exist, but the Eris port definition is disabled.

### Persistence

`SaveLoadManager` serializes `SaveData` as JSON to:

```text
%AppData%/StarSmugglerGame/save.json
```

Save data records credits, cargo capacity and quantities, current port, prices,
and saved game state. Persistence changes must consider compatibility with
existing save files and document schema assumptions.

## Common Changes

Use the actual constructor signatures when extending databases:

```csharp
// Items/ItemsDatabase.cs
new Item("item_id", "Item Name", "Description", ItemRarity.Common, basePrice)

// Ports/PortsDatabase.cs
new Port("port_id", "Port Name", "Description", PortZone.Inner,
    "Ports/image", "Ports/imagePreview", "music_asset")

// Events/EventDatabase.cs
new GameEvent("Event Name", "Description", player =>
{
    // Apply event effects to player state.
})
```

Content asset keys omit file extensions. Add pipeline-managed textures, fonts,
and audio to `Content/Content.mgcb`. The menu-layout JSON is the exception: it
is a directly read content file copied through the runtime project file.

## Build, Run, and Test

Prerequisites are the .NET 8 SDK and restored local tools. The repository pins
`dotnet-mgcb` 3.8.4.1 in `.config/dotnet-tools.json`; project restore also has a
target that restores local tools.

```powershell
dotnet tool restore
dotnet build StarSmuggler.sln
dotnet test StarSmuggler.Tests/StarSmuggler.Tests.csproj
dotnet run --project StarSmuggler.csproj
dotnet run --project StarSmuggler.Editor/StarSmuggler.Editor.csproj
```

Publish the Windows runtime with:

```powershell
dotnet publish StarSmuggler.csproj -c Release -r win-x64 --self-contained
```

Before handing off a change, run the solution build, relevant focused tests,
and `git diff --check`. Rendering, audio, and input changes also require a
documented manual playtest.

## Development Requirements

`.specify/memory/constitution.md` is authoritative for code quality, TDD,
testing, user-experience consistency, and performance. In particular:

- Start behavior changes with a failing automated test or explicit manual
  reproduction whenever practical.
- Add regression coverage for gameplay and persistence behavior that can be
  tested deterministically.
- Keep comments focused on intent, invariants, formulas, transitions, content
  pipeline assumptions, and save compatibility; update stale comments.
- Avoid per-frame asset loads, unnecessary allocations, excessive logging, and
  avoidable `SpriteBatch.Begin`/`End` churn.
- Keep navigation, disabled states, invalid-action feedback, and save/load
  outcomes consistent across screens.

The working tree may contain user changes. Preserve unrelated edits and do not
discard or overwrite them while completing a task.

## Documentation and Roadmap

- `README.md`: contributor-facing project overview and setup.
- `docs/MENU_LAYOUT_EDITOR_GUIDE.md`: editor workflow and layout constraints.
- `docs/BACKLOG.md`: outstanding work and feature ideas.
- `docs/ROADMAP.md`: planned milestones and direction.
- `docs/NOTES.md`: implementation notes and references.
- `docs/TRAVEL_ANIMATION_SETUP.md`: travel animation setup notes.

Quest, reputation, ship upgrades, combat, expanded characters, and other major
systems described in roadmap documents are planned work, not current runtime
capabilities. Do not describe roadmap items as implemented without verifying
the code first.
