# Star Smuggler Prioritized Backlog

This backlog reframes the current roadmap into a shorter, execution-focused plan based on the game's apparent current state: a playable trading loop with room for polish, content depth, and a first meaningful progression layer.

## Planning Assumptions

- The current game loop is playable: trading, travel, random events, save/load, and core screen flow already exist.
- The next highest-leverage work is not a large new system, but making the current loop feel better and stay interesting longer.
- Scope should stay controlled until the game is more polished and replayable in short sessions.

## Priority Order

1. Improve moment-to-moment feedback and feel.
2. Expand content inside existing systems.
3. Add a lightweight contract system.
4. Add a simple progression layer through ship upgrades.
5. Strengthen maintainability and testing where it directly supports shipping features.

## Milestone A: Core Feel

Goal: Make the existing game loop feel polished in the first 10-15 minutes of play.

### A1. Invalid Action Feedback

- Priority: P0
- Effort: S
- Dependencies: Existing button/input handling, audio manager
- Why: Prevents the UI from feeling silent or unresponsive when actions fail
- Tasks:
- Add a bad click or error sound for invalid actions.
- Add consistent visual feedback for disabled or unavailable actions.
- Standardize error messaging for insufficient credits, full cargo, or invalid trade quantities.
- Acceptance criteria:
- Invalid trade and travel actions provide both visual and audible feedback.
- Disabled states are visually distinct from enabled actions.

### A2. Button Hover and Selection States

- Priority: P0
- Effort: S
- Dependencies: Existing reusable UI components
- Why: Improves responsiveness and clarity across all core screens
- Tasks:
- Refine hover, pressed, and selected states for buttons.
- Ensure keyboard/controller focus states are visually clear if supported.
- Acceptance criteria:
- Interactive elements visibly react to hover and selection.
- Core screens use consistent interaction styling.

### A3. UI Transition Polish

- Priority: P1
- Effort: M
- Dependencies: Screen system, UI rendering flow
- Why: Makes screen changes feel intentional rather than abrupt
- Tasks:
- Add lightweight screen fade or slide transitions.
- Add confirmation feedback for successful buy/sell actions.
- Add subtle animation when opening travel or trade views.
- Acceptance criteria:
- Screen transitions are present and do not noticeably degrade performance.
- Buy/sell actions have immediate confirmation feedback.

### A4. Travel Presentation Upgrade

- Priority: P1
- Effort: M
- Dependencies: Existing travel animation screen
- Why: Travel is central to the game loop and should feel rewarding
- Tasks:
- Add star parallax background layers.
- Improve travel distance or route presentation.
- Improve random encounter visual presentation during travel.
- Acceptance criteria:
- Travel sequences feel more dynamic than the current baseline.
- Route/travel information is easier to understand at a glance.

### A5. Font and Text Scalability

- Priority: P1
- Effort: M
- Dependencies: MonoGame content pipeline, UI text rendering
- Why: Better text handling will improve readability and future UI iteration speed
- Tasks:
- Replace or augment current font setup with easier-to-scale options.
- Audit text sizing across the main screens.
- Acceptance criteria:
- Text remains readable across key resolutions.
- UI tuning no longer depends on fragile per-screen font workarounds.

## Milestone B: Content Depth

Goal: Make trading sessions feel less repetitive without introducing major new systems.

### B1. Expand Port Roster

- Priority: P0
- Effort: M
- Dependencies: Existing port database, travel screen, content pipeline
- Why: More locations improve variety and strengthen world-building
- Tasks:
- Add 4-6 new ports across Inner, Outer, and Fringe zones.
- Ensure each new port has distinct descriptive flavor and economic identity.
- Add placeholder or final art/music hooks for each new port.
- Acceptance criteria:
- Each zone has a broader set of destinations.
- New ports appear correctly in travel, overview, and trading flows.

### B2. Expand Item Catalog

- Priority: P0
- Effort: M
- Dependencies: Existing item database and trade UI
- Why: A deeper item pool gives the economy more texture and replay value
- Tasks:
- Expand the current item set toward 20-30 total items.
- Keep rarity and zone logic easy to reason about.
- Add item descriptions if the UI can surface them.
- Acceptance criteria:
- Trading choices feel meaningfully broader than the current baseline.
- Added items integrate cleanly into pricing and cargo systems.

### B3. Expand Event Variety

- Priority: P0
- Effort: M
- Dependencies: Existing event system
- Why: Random travel events are already in the loop and can add a lot of freshness
- Tasks:
- Add 8-12 new events.
- Include a mix of positive, negative, and tradeoff-driven outcomes.
- Add more price-affecting and cargo-affecting events.
- Acceptance criteria:
- Repeated sessions surface noticeably more varied travel outcomes.
- Event effects remain understandable and balanced.

### B4. Port Condition Modifiers

- Priority: P1
- Effort: M
- Dependencies: Port pricing/state model
- Why: Adds local economic flavor without a full economic simulation rewrite
- Tasks:
- Add temporary port states such as shortages, lockdowns, festivals, or inspections.
- Tie those states to pricing, item availability, or travel risk.
- Surface active conditions in the port overview UI.
- Acceptance criteria:
- Ports can temporarily feel different from one visit to the next.
- Conditions are visible and meaningfully affect player decisions.

### B5. Port Flavor Pass

- Priority: P2
- Effort: S
- Dependencies: Content writing, asset assignment
- Why: Supports the project's world-building goals at relatively low cost
- Tasks:
- Improve descriptions for existing ports.
- Assign music and image hooks more consistently.
- Add small bits of flavor text for trading or travel arrival when useful.
- Acceptance criteria:
- Ports feel more memorable and less interchangeable.

## Milestone C: Contract System

Goal: Introduce the first lightweight progression and intent-driving layer beyond freeform arbitrage.

### C1. Contract Data Model

- Priority: P0
- Effort: M
- Dependencies: Save/load, player state, ports/items data
- Why: Contracts need durable state before UI polish
- Tasks:
- Define a contract model for delivery jobs.
- Support fields like origin, destination, item, quantity, payout, and deadline if used.
- Add persistence for active and completed contracts.
- Acceptance criteria:
- Active contracts survive save/load.
- Contract state can be evaluated on arrival at destination.

### C2. Jobs Board UI

- Priority: P0
- Effort: M
- Dependencies: Screen flow, contract data model
- Why: Players need a clear place to discover and accept work
- Tasks:
- Add a jobs board or contracts panel accessible from ports.
- Show reward, destination, and cargo requirements clearly.
- Allow accepting and rejecting available contracts.
- Acceptance criteria:
- Players can browse and accept contracts from at least one port-facing UI path.
- Contract details are easy to compare.

### C3. Delivery Contract Resolution

- Priority: P0
- Effort: M
- Dependencies: Contract data model, travel flow, inventory/cargo logic
- Why: This is the first full loop that turns contracts into gameplay
- Tasks:
- Pay out on successful delivery.
- Handle basic failure states such as cargo loss, selling required goods, or missing a deadline if deadlines are included.
- Provide contract completion/failure feedback.
- Acceptance criteria:
- A player can accept, carry, and complete a delivery contract end to end.
- Failure handling is understandable and does not corrupt save state.

### C4. Optional Deadline Layer

- Priority: P2
- Effort: S
- Dependencies: Base contract system
- Why: Adds tension, but should not block the first usable contract release
- Tasks:
- Add turn/jump-based timing rules for bonus or failure.
- Surface deadline risk clearly in the UI.
- Acceptance criteria:
- Deadline mechanics are legible and do not feel arbitrary.

## Milestone D: Ship Upgrades

Goal: Give players a visible sense of progression and a meaningful credit sink.

### D1. Upgrade Data Model

- Priority: P1
- Effort: S
- Dependencies: Player data, save/load
- Why: Establishes the minimal foundation for progression
- Tasks:
- Add a basic upgrade inventory or ship stat model.
- Persist purchased upgrades.
- Acceptance criteria:
- Upgrade effects survive save/load.

### D2. Basic Upgrade Screen

- Priority: P1
- Effort: M
- Dependencies: Upgrade data model, UI framework
- Why: Makes progression tangible and easy to expand later
- Tasks:
- Add a simple ship services or upgrades screen.
- Present available upgrades and current ship stats.
- Acceptance criteria:
- Players can purchase upgrades from a clear UI flow.

### D3. First Upgrade Set

- Priority: P1
- Effort: M
- Dependencies: Upgrade screen, economy hooks
- Why: These directly strengthen the current loop without adding combat complexity
- Tasks:
- Add cargo expansion upgrade.
- Add travel efficiency or reduced travel-cost upgrade.
- Add event resistance or safer travel upgrade.
- Acceptance criteria:
- Each upgrade has a noticeable gameplay effect.
- Upgrades create useful spending decisions for mid-game credits.

## Supporting Work

These items should happen in support of feature delivery rather than as isolated refactors.

### S1. Data-Driven Content Cleanup

- Priority: P1
- Effort: M
- Dependencies: Existing content definitions
- Tasks:
- Standardize how ports, items, and events are defined.
- Reduce friction for adding new content safely.

### S2. Economy and Save/Load Tests

- Priority: P1
- Effort: M
- Dependencies: Test project setup if missing
- Tasks:
- Add tests for price calculation, travel costs, event effects, and serialization.
- Focus on systems most likely to regress during content expansion.

### S3. Documentation Refresh

- Priority: P2
- Effort: S
- Dependencies: Product direction alignment
- Tasks:
- Fix encoding issues in README.md and ROADMAP.md.
- Replace stale date-based milestones with versioned or milestone-based planning.
- Keep CLAUDE.md, README.md, ROADMAP.md, and this backlog aligned.

### S4. Layout Editor Post-MVP

- Priority: P2
- Effort: M
- Dependencies: Main menu layout editor MVP
- Tasks:
- Extend the editor beyond the main menu once the first JSON runtime path has proven stable.
- Add multi-screen layout selection and screen-specific layout files.
- Explore polygon or alpha-based hit masks for irregular artwork only after rectangle masks are reliable.
- Add optional animation timeline and transition metadata for future menu polish.
- Add audio cue metadata only after runtime audio ownership is clear.
- Consider content-pipeline management tools, but keep them separate from the runtime layout contract.
- Acceptance criteria:
- Post-MVP editor work continues to export repo-relative, runtime-loadable layout data.
- The MonoGame runtime remains free of Avalonia or editor-framework dependencies.

## Recommended Release Slices

### Slice 1: Polish Update

- A1 Invalid Action Feedback
- A2 Button Hover and Selection States
- A3 UI Transition Polish
- A4 Travel Presentation Upgrade

Target outcome: The current game feels noticeably better without changing its core structure.

### Slice 2: Content Update

- B1 Expand Port Roster
- B2 Expand Item Catalog
- B3 Expand Event Variety
- B5 Port Flavor Pass

Target outcome: The game supports more replayable short sessions and stronger world flavor.

### Slice 3: Contract Update

- C1 Contract Data Model
- C2 Jobs Board UI
- C3 Delivery Contract Resolution

Target outcome: Players now have directed goals beyond pure market optimization.

### Slice 4: Progression Update

- D1 Upgrade Data Model
- D2 Basic Upgrade Screen
- D3 First Upgrade Set

Target outcome: The player gains a clear growth path and a reason to invest earnings.

## Deprioritized For Now

These are still valuable, but should not be on the near-term critical path.

- Full combat system
- Deep faction warfare
- Branching narrative system
- Galaxy map overhaul
- Multiplayer and social systems
- Mobile and console ports
- Endgame and New Game+ systems

## Definition of Near-Term Success

The next practical target for Star Smuggler should be:

"A polished and replayable 20-30 minute session with stronger feedback, more content variety, and at least one directed progression layer."

If a task does not clearly help reach that target, it should usually come after Milestones A through D.
