# 🌌 Star Smuggler

**Star Smuggler** is an open-source, retro-futuristic trading and smuggling game inspired by the classic *Dope Wars*, set in a lawless corner of the galaxy. Take on the role of a daring space rogue as you barter, bribe, and blast your way across procedurally generated star systems in search of fortune and fame.

## 🚀 About the Game

You're not a villain. You're just trying to make a few credits running questionable cargo between distant worlds — preferably without drawing the attention of planetary law enforcement or rival syndicates. Navigate between different port zones, each with their own specialties and dangers, as you build your fortune one trade at a time.

## 🎮 Current Features

### ✅ Core Gameplay
- **🏛️ Port System**: Visit diverse locations across Inner, Outer, and Fringe zones
  - **Inner Ports**: Mercury Foundry Complex, Venus Sky Habitats, Luna Central Station, Mars colonies
  - **Outer Ports**: Europa research stations, Titan mining operations
  - **Fringe Ports**: Kuiper Belt outposts, Pluto frontier settlements, distant Ceres stations
- **💰 Dynamic Trading**: Buy low, sell high with fluctuating market prices
- **🎯 Zone-Based Economy**: Item rarities and prices vary by location
  - Common items are cheaper in Inner zones, expensive in Fringe
  - Exotic goods are valuable in Inner zones, common in Fringe
- **� Space Travel**: Navigate between ports with distance-based fuel costs
- **💾 Save System**: Persistent game progress with automatic saves

### ✅ Game Mechanics
- **📊 Market Dynamics**: Prices update based on travel frequency and random events
- **🎲 Random Events**: 30% chance of encounters during travel (pirates, traders, anomalies)
- **� Economic Balance**: Game over conditions when unable to afford travel or trading
- **🎒 Cargo Management**: Limited cargo space (30 units) encourages strategic trading
- **💵 Starting Resources**: Begin with 500 credits at a random Inner port

### ✅ User Interface
- **🖥️ Modern UI**: Clean, terminal-inspired interface with retro-futuristic aesthetic
- **🗺️ Port Overview**: Immersive port descriptions with atmospheric backgrounds
- **📈 Trade Terminal**: Intuitive buy/sell interface with quantity controls
- **🌌 Travel Screen**: Visual port selection with preview images and cost calculations
- **🔊 Audio**: Atmospheric music and sound effects for enhanced immersion

### ✅ Technical Features
- **🎯 MonoGame Framework**: Cross-platform compatibility (Windows, Linux, macOS)
- **⚡ Performance**: Efficient rendering and state management
- **🏗️ Modular Architecture**: Extensible screen system and component-based UI
- **📱 Responsive Design**: Adaptive layouts for different screen sizes

## 🖥️ Screenshots

*(Screenshots coming soon!)*

## ⚙️ Installation & Setup

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [MonoGame 3.8+](https://www.monogame.net/downloads/)

### Quick Start

1. **Clone the repository**:
   ```bash
   git clone https://github.com/YOUR_USERNAME/star-smuggler.git
   cd star-smuggler
   ```

2. **Run the game**:
   ```bash
   dotnet run
   ```

   *Or open `StarSmuggler.sln` in Visual Studio/Rider and press F5*

### Building for Distribution

```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained

# Linux
dotnet publish -c Release -r linux-x64 --self-contained

# macOS
dotnet publish -c Release -r osx-x64 --self-contained
```

## 🎯 How to Play

1. **Start Your Journey**: Begin at a random Inner zone port with 500 credits
2. **Explore Ports**: Visit the trader to see available goods and their prices
3. **Trade Strategically**: 
   - Buy cheap goods where they're common
   - Sell expensive goods where they're rare
   - Watch your cargo space (30 units max)
4. **Travel Wisely**: 
   - Movement costs vary by distance between zones
   - Longer journeys may trigger random events
5. **Manage Resources**: Keep enough credits for travel costs or risk game over
6. **Build Your Fortune**: Accumulate wealth through smart trading decisions

## 🏗️ Project Structure

```
StarSmuggler/
├── Audio/                   # Audio management system
├── Content/                 # Game assets (textures, fonts, audio)
│   ├── Fonts/              # Font files for UI text
│   ├── Music/              # Background music tracks
│   ├── Ports/              # Port images and previews
│   └── UI/                 # Interface textures and icons
├── Data/                   # Data management and serialization
├── Events/                 # Random event system
├── Factions/               # Faction system (future expansion)
├── Items/                  # Item definitions and database
├── Player/                 # Player data and state management
├── Ports/                  # Port system and database
├── Screens/                # Game screens and UI management
└── UI/                     # Reusable UI components
```

## 🛠️ Architecture

### Core Systems
- **GameManager**: Central singleton managing game state and logic
- **ScreenManager**: Handles screen transitions and lifecycle
- **SaveLoadManager**: Persistent data storage using JSON serialization
- **Audio Manager**: Background music and sound effect management

### Key Components
- **Port System**: Hierarchical zones with unique characteristics
- **Item Database**: Categorized trading goods with rarity tiers
- **Event System**: Procedural encounters during travel
- **UI Framework**: Reusable components (buttons, terminals, info panels)

## 🗺️ Roadmap

### 🔄 In Progress
- Enhanced UI polish and visual effects
- Additional port locations and descriptions
- Expanded item catalog

### 📋 Planned Features
- **🗺️ Galaxy Map**: Visual navigation interface
- **⚔️ Combat System**: Space battles and ship upgrades
- **👥 Faction System**: Reputation and relationship mechanics
- **🎭 Character System**: NPCs, crew members, and dialogue
- **📖 Quest System**: Story missions and procedural contracts
- **🎮 Mini-Games**: Hacking, negotiation, and piloting challenges
- **🛸 Ship Customization**: Upgradeable vessels and equipment
- **🌟 Endgame Content**: Victory conditions and new game+ modes

### 🚀 Future Expansions
- Multiplayer trading network
- Mod support and community content
- Additional game modes (permadeath, hardcore economy)
- Expanded universe with new regions

## 🤝 Contributing

We welcome contributions! Here's how you can help:

1. **🐛 Bug Reports**: Open an issue with reproduction steps
2. **💡 Feature Requests**: Suggest new gameplay mechanics or improvements
3. **🎨 Assets**: Contribute artwork, music, or sound effects
4. **💻 Code**: Submit pull requests for bug fixes or new features
5. **📖 Documentation**: Improve guides, comments, or examples

### Development Setup
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Make your changes and test thoroughly
4. Submit a pull request with a clear description

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Inspired by the classic **Dope Wars** trading game
- Built with the excellent **MonoGame** framework
- Special thanks to the retro gaming community for inspiration

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/YOUR_USERNAME/star-smuggler/issues)
- **Discussions**: [GitHub Discussions](https://github.com/YOUR_USERNAME/star-smuggler/discussions)
- **Documentation**: [Wiki](https://github.com/YOUR_USERNAME/star-smuggler/wiki)

---

*"In space, no one can hear you haggle."* ⭐
