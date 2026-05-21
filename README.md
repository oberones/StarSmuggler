# Star Smuggler

Star Smuggler is an open-source retro-futuristic trading game inspired by the classic *Dope Wars* formula, reimagined as a space-smuggling adventure built with MonoGame and .NET 8.

You play a freelance runner moving cargo between dangerous ports across the solar system, trying to turn limited credits into a fortune before bad markets, bad luck, or bad decisions leave you stranded.

## Current Status

Star Smuggler currently has a playable core loop built around:

- Traveling between ports in different economic zones
- Buying and selling goods in a dynamic market
- Managing cargo space and travel costs
- Surviving random events during travel
- Preserving progress through save/load functionality

The game is now in a "core polish and expansion" stage. The near-term focus is improving feel, increasing variety, and adding a first progression layer before larger systems like combat, factions, and narrative arcs.

See:

- [ROADMAP.md](ROADMAP.md) for milestone-level direction
- [BACKLOG.md](BACKLOG.md) for prioritized implementation work
- [CLAUDE.md](CLAUDE.md) for project architecture and design context

## Core Gameplay

The current experience centers on a simple but expandable trading loop:

1. Start at a random Inner-zone port with 500 credits and an empty cargo hold.
2. Check local prices and buy goods that are cheap in the current zone.
3. Travel to another port while paying travel costs and risking random events.
4. Sell goods where they are more valuable.
5. Repeat the loop while managing cash flow, cargo limits, and risk.

### Economic Structure

- Inner zones favor common goods
- Fringe zones favor exotic goods
- Outer zones sit between the two
- Prices shift over time and with travel
- Events can positively or negatively affect your run

## Current Features

### Implemented

- Zone-based trading economy
- Multiple ports with atmospheric descriptions
- Dynamic price generation
- Travel cost system
- Random travel events
- Save/load with JSON persistence
- Terminal-inspired UI flow
- Travel animation screen
- Background music support

### In Progress

- UI and travel polish
- Content expansion for ports, items, and events
- Better feedback and presentation across the main loop

### Planned

- Delivery contracts and jobs
- Ship upgrades and progression
- More port activities
- Faction systems
- Combat and conflict mechanics
- Quest and narrative systems
- Galaxy map and broader exploration

## Project Structure

```text
StarSmuggler/
|-- Audio/          # Audio management
|-- Content/        # MonoGame assets
|   |-- Fonts/
|   |-- Music/
|   |-- Ports/
|   `-- UI/
|-- Data/           # Data and serialization support
|-- Events/         # Random event system
|-- Factions/       # Future faction-related work
|-- Items/          # Item definitions and data
|-- Player/         # Player state
|-- Ports/          # Port data and behavior
|-- Screens/        # Screen flow and game views
|-- UI/             # Reusable interface components
|-- GameManager.cs  # Core game state and economy logic
|-- Game1.cs        # MonoGame entry point
`-- SaveLoadManager.cs
```

## Architecture Overview

The game is organized around a few central ideas:

- `GameManager` acts as the main authority for game state and economy logic.
- Screens are separated into distinct views for menu, port overview, trade, travel, and game over flow.
- UI is built from reusable components rather than one-off interfaces.
- Save data is serialized to JSON for persistence.
- Content is managed through the MonoGame content pipeline.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MonoGame 3.8+](https://www.monogame.net/downloads/)

### Run Locally

```bash
dotnet tool restore
dotnet run
```

You can also open `StarSmuggler.sln` in Visual Studio or Rider and run the project there.

The project uses the MonoGame content pipeline during builds. The required
`dotnet-mgcb` CLI is pinned in `.config/dotnet-tools.json`, so `dotnet tool
restore` should be run once after cloning, or automatically by the project
restore target when building through an IDE.

The terminal UI font, Share Tech Mono, is bundled in `Content/Fonts` under the
SIL Open Font License so contributors do not need to install it system-wide.

### Build

```bash
dotnet publish -c Release -r win-x64 --self-contained
```

Example alternate targets:

- `linux-x64`
- `osx-x64`

## Contributing

Contributions are welcome across code, design, writing, audio, and art.

Helpful contribution areas include:

- Bug fixes and stability improvements
- UI and UX polish
- New ports, items, and events
- Documentation cleanup
- Balancing and playtesting feedback
- Art and music support

If you are contributing code:

1. Fork the repository.
2. Create a branch for your work.
3. Follow the project constitution in `.specify/memory/constitution.md`.
4. Write the failing test or manual reproduction before implementation when practical.
5. Add or update comments for public APIs, gameplay formulas, state transitions,
   library use, and content-pipeline or save-schema assumptions.
6. Build and test your changes, including manual playtests for UI/audio behavior.
7. Open a pull request with a clear summary and validation notes.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

## Acknowledgments

- Inspired by the original *Dope Wars* trading formula
- Built with MonoGame
- Shaped by retro sci-fi, trading sims, and terminal-style interfaces

---

"In space, no one can hear you haggle."
