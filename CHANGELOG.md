# Changelog

All notable changes to the StarSmuggler project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [v0.6.0] - 2025-07-06

### Added
- Dynamic event-based price indicators in trade window with color coding
- Visual indicators (++/--) for event-affected prices 
- Item descriptions displayed below item names in trade terminal
- Enhanced InfoPanel with event information display
- Colorful text formatting for different UI elements (port names, descriptions, events)
- Event information in port overview with visual hierarchy

### Changed
- Trade window layout: items on left, controls on right, improved organization
- Trade terminal now uses port-specific background images
- Item prices show different colors based on event modifications:
  - Red: Doubled prices (Merchant Strike)
  - Cyan: Halved prices (Market Glut)  
  - Orange: Higher than normal prices
  - Light Green: Normal or reduced prices
- Enhanced event system with proper triggering during travel
- InfoPanel text now uses different colors and fonts for better visual hierarchy

### Fixed
- Events now properly change when traveling between ports
- Event information correctly updates in port overview screen
- Removed duplicate event triggering that caused events to never change
- Corrected random event probability logic

## [v0.5.0] - 2025-07-06

### Added
- Refactored port overview screen with new info panel
- Action buttons with icons (Trade, Ship, Fuel)
- Port-specific background images for trade screens
- InfoPanel UI component for reusable information display
- Icons for different port actions

### Changed
- Port overview layout moved from vertical to horizontal button arrangement
- Buttons now positioned at bottom of screen with icons above
- Info panel replaces old terminal window for port information

## [v0.4.0] - 2025-06-08

### Added
- Travel screen with port preview images
- Port preview functionality when selecting destinations
- Navigation buttons (Previous/Next) for browsing ports
- Travel button integrated into terminal interface

### Changed
- Travel screen UI completely redesigned with preview system
- Port selection now shows preview images
- Improved travel cost display and port information

## [v0.3.1] - 2025-06-07

### Fixed
- Resolved bug in buy and sell price calculations for items
- Item pricing now works correctly in trade interface

## [v0.3.0] - 2025-06-04

### Added
- Additional random events during travel
- Event system with multiple event types:
  - Customs Shake-Down
  - Merchant Strike
  - Market Glut
  - Lost Cargo
  - Pirate Ambush
  - Engine Malfunction
  - Crew Mutiny

### Changed
- Simplified event code structure
- Events now properly affect game state and item prices

### Fixed
- Game over calculations now use current item prices instead of base prices
- Events no longer impact base item prices, only current market prices

## [v0.2.0] - 2025-06-04

### Added
- Improved trade menu interface
- Better item display and organization
- Enhanced UI components for trading

### Changed
- Trade screen layout and visual design
- Improved user interaction flow

## [v0.1.0] - Initial Release

### Added
- Basic space trading game mechanics
- Port system with different zones (Inner, Outer, Fringe)
- Item trading with different rarities (Common, Mid-Tier, Exotic)
- Player progression system with credits and cargo
- Basic event system
- Save/load functionality
- Audio system with background music and sound effects
- Multiple game screens (Main Menu, Port Overview, Trade, Travel, Game Over)

### Features
- 9 unique ports across the solar system
- Dynamic item pricing based on port zones and item rarity
- Random events that affect gameplay
- Cargo management system
- Travel cost calculation based on distance between ports
- Game over conditions based on player resources
