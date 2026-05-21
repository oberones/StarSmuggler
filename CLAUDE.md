# CLAUDE.md - Star Smuggler Project Guide

## Project Overview

**Star Smuggler** is a retro-futuristic space trading game inspired by classic *Dope Wars* but with expanded mechanics. Built with MonoGame/C#/.NET 8.0, it's designed to be cross-platform with a focus on immersive world-building, atmospheric storytelling, and strategic trading mechanics.

### Core Vision
- **Retro-futuristic aesthetic**: Terminal-inspired UI with atmospheric backgrounds
- **World-building focus**: Rich port descriptions, character interactions, evolving narrative
- **Strategic depth**: Beyond simple trading - quests, minigames, faction systems planned
- **Atmospheric immersion**: Dynamic music, sound effects, visual storytelling

## Technical Architecture

### Framework & Dependencies
- **.NET 8.0** with **MonoGame 3.8+** framework
- **Windows primary platform** (cross-platform planned)
- **JSON serialization** for save/load system
- **Content Pipeline** for assets (textures, fonts, audio)

### Core Design Patterns

#### Singleton GameManager
```csharp
public class GameManager
{
    public static GameManager Instance { get; private set; } = new GameManager();
    // Central authority for game state, player data, and core mechanics
}
```

#### Screen-Based Architecture
- **ScreenManager**: Handles screen lifecycle and transitions
- **IScreen interface**: Standardized screen implementation
- **Game state enum**: Drives screen transitions and game flow

#### Component-Based UI
- Reusable UI components (`Button`, `Terminal`, `InfoPanel`, etc.)
- Modular design for consistent styling and behavior

## Game Systems

### Economic System

#### Zone-Based Trading
```
Inner Zone (Mercury, Venus, Luna, Mars)
├── Common items: Cheap to buy, expensive to sell in Fringe
├── Exotic items: Expensive to buy, cheap to sell
└── Starting location for new players

Outer Zone (Ceres, Europa, Titan)  
├── Mid-tier items: Balanced pricing
├── Mix of common and exotic goods
└── Intermediate trading posts

Fringe Zone (Pluto, Kuiper)
├── Exotic items: Cheap to buy, expensive to sell in Inner
├── Common items: Expensive to buy, cheap to sell  
└── High-risk, high-reward trading
```

#### Dynamic Pricing
- **Price updates**: Every 4+ jumps based on random variance + zone markup
- **Market events**: Random events can crash/boost specific item prices
- **Rarity-based markup**: Common/MidTier/Exotic items have different zone preferences

#### Travel Economics
- **Base cost**: 15 credits minimum
- **Zone distance**: +2 credits per zone difference
- **Long distance**: 2x multiplier for 2+ zone jumps
- **Game over condition**: <15 credits + unable to sell cargo for travel funds

### Player Progression

#### Starting Conditions
- **500 credits** starting funds
- **30 unit cargo limit**
- **Random Inner zone port** starting location
- **No starting cargo**

#### Progression Mechanics
- **Credit accumulation**: Primary success metric
- **Trading efficiency**: Learning optimal buy/sell routes
- **Risk management**: Balancing travel costs vs. profit potential
- **Event handling**: Dealing with random encounters (30% chance per travel)

### Random Events System
```csharp
// Event structure
public class GameEvent
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Action<PlayerData> ApplyEffect { get; set; }
}
```

#### Current Events
- **Market Glut**: Halves item price at current port
- **Lost Cargo**: Removes random cargo item
- **Pirate Ambush**: Loses 50-200 credits
- **Engine Malfunction**: Loses 100-300 credits

### Port System

#### Port Zones & Characteristics
```csharp
public enum PortZone { Inner, Outer, Fringe }

// Example ports with atmospheric descriptions
"Mercury Foundry Complex" - Industrial heat-tech specialist
"Venus Sky Habitats" - Corporate cartel floating cities  
"Luna Central Station" - Major Inner zone trading hub
"Pluto Relic Vault" - Forbidden ancient tech discoveries
"Kuiper Flotilla" - Nomadic pirate fleet beyond Neptune
```

#### Port Features
- **Available items**: 4 zone-appropriate + 2 cross-zone items
- **Dynamic prices**: Updated based on economic cycles
- **Atmospheric assets**: Background images, preview images, music tracks
- **Rich descriptions**: Immersive world-building text

## Technical Implementation

### File Structure
```
StarSmuggler/
├── Audio/              # AudioManager class
├── Content/            # MonoGame assets
│   ├── Fonts/         # SpriteFont files
│   ├── Music/         # Background tracks  
│   ├── Ports/         # Port images/previews
│   └── UI/            # Interface textures
├── Data/              # Future data management
├── docs/              # Project planning and setup documentation
│   ├── BACKLOG.md     # Outstanding work and feature ideas
│   ├── NOTES.md       # Development notes and reference material
│   ├── ROADMAP.md     # Planned milestones and feature direction
│   └── TRAVEL_ANIMATION_SETUP.md # Travel animation implementation notes
├── Events/            # Event system (GameEvent, EventDatabase)
├── Items/             # Item system (Item, ItemsDatabase)
├── Player/            # PlayerData class
├── Ports/             # Port system (Port, PortsDatabase)
├── Screens/           # Screen implementations
└── UI/                # Reusable UI components
```

### Key Classes

#### GameManager.cs
- **Singleton pattern**: Central game authority
- **State management**: Game state transitions
- **Economic logic**: Price calculations, travel costs, markups
- **Game loop coordination**: Save/load, event triggering, game over detection

#### PlayerData.cs
```csharp
public class PlayerData
{
    public int Credits { get; set; }
    public Dictionary<Item, int> CargoHold { get; set; }
    public int CargoLimit { get; set; } // 30 units
    public Port CurrentPort { get; set; }
    public Dictionary<string, Dictionary<string, int>> CurrentPrices { get; set; }
    public int JumpsSinceLastUpdate { get; set; }
    public GameEvent CurrentEvent { get; set; }
}
```

#### Screen System
```csharp
public interface IScreen
{
    void LoadContent(GraphicsDevice graphics, ContentManager content);
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch);
    void Refresh(ContentManager content);
}
```

### Current Screens
- **MainMenuScreen**: Entry point, continue/new game
- **PortOverviewScreen**: Port description and atmosphere
- **TradeScreen**: Buy/sell interface with quantity controls
- **TravelScreen**: Port selection with cost preview
- **GameOverScreen**: End state with restart option

## Development Guidelines

### Code Style & Patterns
- **Clear documentation**: Comprehensive XML comments for public methods
- **Generous intent comments**: Document library usage, gameplay formulas, screen
  state transitions, content-pipeline assumptions, and save/load schema behavior
- **Meaningful naming**: Self-documenting variable and method names
- **Single responsibility**: Classes focused on specific functionality
- **Event-driven architecture**: Loose coupling between systems

### Constitution & Quality Gates
- Follow `.specify/memory/constitution.md` for code quality, testing, TDD,
  user experience consistency, and performance requirements.
- Define a failing automated test or manual reproduction before production
  behavior changes whenever practical.
- Validate gameplay logic with focused tests where possible and manual
  playtests for rendering, audio, and input flows.
- Include build, test, and manual validation notes in pull requests.

### MonoGame Best Practices
- **Content Pipeline**: Use .mgcb for asset management
- **SpriteBatch efficiency**: Minimize Begin/End calls
- **Texture atlasing**: Consider for UI elements
- **Font optimization**: SpriteFont files for different sizes

### Future Architecture Considerations
- **Component system**: For complex game objects (ships, equipment)
- **Data-driven design**: JSON/XML configuration for ports, items, events
- **Localization support**: String externalization for multi-language
- **Mod support**: Plugin architecture for community content

## Planned Features & Roadmap

### Near-term Enhancements
- **UI polish**: Improved visual effects, animations
- **Additional ports**: Expand universe with more locations
- **Enhanced events**: More diverse random encounters
- **Audio expansion**: Port-specific music, more SFX

### Major Feature Additions
- **Quest system**: Story-driven missions and contracts
- **Faction reputation**: Relationship mechanics with different groups
- **Ship upgrades**: Cargo capacity, fuel efficiency, combat capabilities
- **Combat system**: Space battles with tactical elements
- **Character system**: NPCs, crew members, dialogue trees

### Advanced Systems
- **Galaxy map**: Visual navigation interface
- **Minigames**: Hacking, negotiation, piloting challenges
- **Procedural generation**: Dynamic events, port conditions
- **Multiplayer**: Shared economy, player interaction

## Working with the Codebase

### Getting Started
1. **Prerequisites**: .NET 8.0 SDK, MonoGame 3.8+
2. **Run**: `dotnet run` or F5 in Visual Studio
3. **Build**: `dotnet publish -c Release -r win-x64 --self-contained`

### Project Documentation
- **docs/BACKLOG.md**: Pending features, improvements, and future tasks
- **docs/NOTES.md**: Working notes, implementation details, and references
- **docs/ROADMAP.md**: Planned milestones and broader project direction
- **docs/TRAVEL_ANIMATION_SETUP.md**: Setup notes for the travel animation flow

### Common Development Tasks

#### Adding New Items
```csharp
// In ItemsDatabase.cs
new Item("item_id", "Item Name", basePrice, ItemRarity.Common)
```

#### Adding New Ports
```csharp
// In PortsDatabase.cs  
new Port("port_id", "Port Name", "Description", PortZone.Inner, "imagePath", "previewPath", "musicTrack")
```

#### Adding New Events
```csharp
// In EventDatabase.cs
new GameEvent("Event Name", "Description", player => {
    // Modify player state
})
```

#### Creating New Screens
1. Implement `IScreen` interface
2. Register in `Game1.LoadContent()`
3. Add corresponding `GameState` enum value

### Testing & Debugging
- **Console output**: Extensive logging for trading, events, state changes
- **Save system**: JSON files in `%AppData%/StarSmugglerGame/`
- **Error handling**: Game over conditions, insufficient funds validation

## Art & Asset Guidelines

### Visual Style
- **Retro-futuristic aesthetic**: Clean lines, terminal-inspired UI
- **Atmospheric backgrounds**: Distinctive port environments
- **Consistent color palette**: Space-themed blues, grays, accent colors
- **Readable fonts**: Terminal-style typefaces for immersion

### Asset Specifications
- **Port images**: Full-screen backgrounds (1536x1024 target)
- **UI elements**: PNG with transparency support
- **Audio**: MP3 for music, WAV for sound effects
- **Fonts**: MonoGame SpriteFont format (.spritefont)

### Content Organization
```
Content/
├── Fonts/     # Different sizes: 12pt, 16pt, 18pt, bold variants
├── Music/     # Atmospheric tracks per port/zone
├── Ports/     # Background + preview image pairs
├── UI/        # Buttons, panels, icons, terminal graphics
└── FX/        # Sound effects (click, ambient)
```

## Performance Considerations

### MonoGame Optimization
- **Texture loading**: Lazy load port-specific assets
- **Memory management**: Dispose unused textures
- **Update efficiency**: Only update active screen
- **Draw batching**: Minimize state changes in SpriteBatch

### Save System
- **JSON serialization**: Human-readable save files
- **Incremental saves**: Save after significant state changes
- **Error recovery**: Graceful handling of corrupted saves

## Community & Contribution

### Open Source Philosophy
- **MIT License**: Permissive for community contributions
- **GitHub workflow**: Issues, pull requests, discussions
- **Documentation first**: Clear guides for contributors
- **Modular design**: Easy to extend and modify

### Contribution Areas
- **Code**: Bug fixes, new features, optimizations
- **Art**: Port backgrounds, UI improvements, animations
- **Audio**: Music tracks, sound effects, ambient audio
- **Writing**: Port descriptions, event text, world-building
- **Testing**: Platform testing, balance feedback, bug reports

---

*This guide should help you understand the codebase structure, development patterns, and vision for Star Smuggler. The project balances retro gaming nostalgia with modern development practices, aiming to create an immersive trading experience that goes beyond simple number manipulation.*

<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read `specs/001-menu-layout-editor/plan.md`.
<!-- SPECKIT END -->
