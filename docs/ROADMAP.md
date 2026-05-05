# Star Smuggler Roadmap

This roadmap reflects the project's current state more directly than the earlier long-range phase plan. Star Smuggler already has a playable core loop. The near-term focus is to make that loop feel polished, replayable, and worth expanding before larger systems like combat, factions, or multiplayer are added.

## Current State

### Completed Foundations

- Core trading system with dynamic pricing
- Zone-based economy across Inner, Outer, and Fringe ports
- Port system with atmospheric descriptions
- Save/load functionality with JSON persistence
- Random event system tied to travel
- Wealth-scaling event consequences
- Travel animation screen with moving ship sprite
- Basic terminal-inspired UI framework
- Background music and core audio support

### Current Development Focus

The project is now beyond the "prove the trading loop works" stage and is in the "make it feel good and stay interesting" stage.

That means the highest-priority work is:

- Better moment-to-moment feedback and polish
- More content inside existing systems
- A first lightweight progression layer

## Near-Term Product Goal

The next practical target for Star Smuggler is:

"A polished and replayable 20-30 minute session with stronger feedback, more content variety, and at least one directed progression layer."

## Milestone 1: Core Feel

Goal: Improve moment-to-moment quality without expanding scope too aggressively.

### In Scope

- Add invalid-action feedback such as bad click sounds and clearer disabled states
- Improve button hover, pressed, and selected visual states
- Add screen transition polish and trade confirmation feedback
- Improve the travel experience with parallax stars and stronger route presentation
- Improve font handling and text readability across screens

### Success Criteria

- The game feels more responsive and readable
- Travel feels more dynamic and less placeholder
- Trading feedback is immediate and satisfying

## Milestone 2: Content Depth

Goal: Reduce repetition by expanding the content already supported by the game.

### In Scope

- Add more ports across Inner, Outer, and Fringe zones
- Expand the item catalog significantly beyond the current small pool
- Add more random travel events with a better mix of positive, negative, and tradeoff outcomes
- Add port condition modifiers such as shortages, inspections, lockdowns, or festivals
- Improve descriptive flavor, art hooks, and music hooks for ports

### Success Criteria

- Repeated sessions feel less predictable
- Ports feel more distinct from each other
- Trading decisions have more variety and context

## Milestone 3: Contracts

Goal: Add the first directed objective system beyond open-ended arbitrage.

### In Scope

- Introduce a delivery contract data model
- Add a jobs board or equivalent port-facing contract UI
- Support accepting, tracking, and completing delivery jobs
- Persist active contracts through save/load
- Add clear reward and failure handling

### Success Criteria

- Players can take on jobs with clear goals and payouts
- The game offers a reason to travel besides price optimization
- Contracts integrate cleanly with the existing trade and travel loop

## Milestone 4: Ship Progression

Goal: Give players a tangible sense of growth and a meaningful use for accumulated credits.

### In Scope

- Add a basic ship upgrade data model
- Add a simple upgrade or ship services screen
- Introduce a first upgrade set such as:
- Cargo capacity upgrades
- Travel efficiency upgrades
- Event resistance or travel safety upgrades

### Success Criteria

- Players can invest in long-term improvements
- Credits matter beyond immediate trading capacity
- Progression strengthens the current loop rather than replacing it

## Supporting Work

These tasks should be done in service of milestones rather than as isolated cleanup projects.

### Technical Support

- Standardize content definitions for ports, items, and events where useful
- Add automated tests around economy calculations, save/load behavior, and event effects
- Continue improving content pipeline organization for easier asset additions

### Documentation Support

- Keep `CLAUDE.md`, `README.md`, `ROADMAP.md`, and `BACKLOG.md` aligned
- Fix encoding/readability issues in project documentation
- Prefer milestone-based planning over stale date-based phase targets

## Later, But Not Immediate

These systems are still important to the long-term vision, but they are intentionally not on the critical near-term path.

- Combat system
- Faction reputation and faction warfare
- NPC relationship systems
- Branching story and questlines beyond lightweight contracts
- Galaxy map overhaul
- Minigames and skill systems
- Multiple ship classes and deep ship customization
- Multiplayer and social systems
- Mobile, console, and broader platform expansion
- Endgame systems, achievements, and New Game+

## Planning Principle

Star Smuggler should grow by deepening the existing game first, then layering in larger systems once the base experience is polished and replayable.

If a feature does not clearly improve:

- feel
- replayability
- directed player motivation

it should usually come after Milestones 1 through 4.
