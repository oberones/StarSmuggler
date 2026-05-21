# Feature Specification: Main Menu Layout Editor

**Feature Branch**: `001-menu-layout-editor`
**Created**: 2026-05-21
**Status**: Draft
**Input**: User description: "Create a feature specification for a standalone Star Smuggler menu layout editor focused on the main menu image, editable text boxes, invisible button hit masks, JSON layout export, runtime loading by MainMenuScreen, hardcoded fallback when the JSON is missing or invalid, and no npm or web stack requirement."

## Clarifications

### Session 2026-05-21

- Q: How should exported 1536x1024 layout coordinates behave if the game viewport differs from the design canvas? → A: Independently scale X and Y coordinates from 1536x1024 to the current viewport.
- Q: How should the editor handle text boxes or button masks dragged or resized beyond the 1536x1024 canvas? → A: Clamp element bounds to the design canvas while editing.
- Q: What uniqueness rule should apply to text box and button mask ids? → A: Element ids must be globally unique across the entire layout.
- Q: How should optional button mask labels behave at runtime? → A: Button mask labels are editor-only annotations and are not rendered by the game.
- Q: How should overlapping enabled button masks be handled? → A: Reject overlapping enabled button masks; editor validation blocks save/export until fixed.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Open Main Menu Artwork (Priority: P1)

A Star Smuggler developer/designer opens the existing main menu artwork in a standalone editor and sees it positioned on the same design canvas used by the game.

**Why this priority**: The editor has no value unless the current menu image can be opened and viewed against the authoritative 1536x1024 coordinate space without changing game code.

**Independent Test**: Can be tested by opening a repo-local main menu image and confirming the image appears on a 1536x1024 design canvas with coordinate readouts available for layout work.

**Acceptance Scenarios**:

1. **Given** a repo-local menu image such as `Content/UI/MainMenu.png`, **When** the user opens it in the editor, **Then** it appears on a 1536x1024 design canvas with no manual code changes.
2. **Given** the user opens an unsupported, missing, or unreadable image, **When** the editor cannot load it, **Then** the editor shows clear invalid-file feedback and leaves the current layout unchanged.

---

### User Story 2 - Author Text and Button Regions (Priority: P1)

A developer/designer creates and adjusts menu text boxes and invisible button hit masks directly over the menu artwork, then saves those positions for the game.

**Why this priority**: The central workflow is removing hardcoded C# coordinate edits for main menu text and button alignment.

**Independent Test**: Can be tested by adding, selecting, moving, resizing, deleting, saving, reopening, and verifying a text box and a button mask without launching the game.

**Acceptance Scenarios**:

1. **Given** an open layout, **When** the user adds a button mask over the "New Game" area and assigns `NewGame`, **Then** the exported JSON contains that region and action.
2. **Given** a selected element in the editor, **When** the user drags or resizes it, **Then** its coordinates update visibly and are preserved after save and reopen.
3. **Given** a selected text box, **When** the user edits its text, font key, size or scale, color, and horizontal alignment, **Then** the editor stores the edited values in the layout.
4. **Given** a selected button mask, **When** the user edits its id, action, optional editor-only label, enabled state, and bounds, **Then** the editor stores those values and visibly outlines the mask while editing.
5. **Given** a selected text box or button mask, **When** the user drags or resizes it past the canvas edge, **Then** the editor clamps the element bounds within the 1536x1024 canvas.
6. **Given** two enabled button masks overlap, **When** the user attempts to save or export the layout, **Then** the editor shows a validation error and blocks the save or export until the overlap is fixed.

---

### User Story 3 - Load Layout in the Game (Priority: P1)

A player sees the main menu rendered from the exported layout while existing menu actions continue to behave as before.

**Why this priority**: The editor must produce a runtime artifact that the game can actually use; otherwise the alignment work still has to be duplicated in code.

**Independent Test**: Can be tested by exporting a valid main-menu layout, starting the game, confirming the background/text render from the layout, and clicking each button mask.

**Acceptance Scenarios**:

1. **Given** an exported main-menu layout, **When** the game starts, **Then** `MainMenuScreen` renders the configured background and text elements.
2. **Given** a `NewGame`, `LoadGame`, `SaveGame`, or `Quit` button mask in the exported layout, **When** the player clicks the mask, **Then** the game triggers the existing mapped behavior for that action.
3. **Given** a button mask marked disabled, **When** the player clicks inside that mask, **Then** the game ignores the click and preserves the current menu state.
4. **Given** the runtime viewport differs from 1536x1024, **When** the game renders and hit-tests a valid layout, **Then** the layout's X and Y coordinates scale independently to match the current full-window background.
5. **Given** a button mask has an optional label, **When** the game renders the main menu, **Then** the game does not render that label as visible text.

---

### User Story 4 - Preserve Safe Fallback Behavior (Priority: P2)

A developer can remove, corrupt, or invalidate the layout file without breaking the game menu.

**Why this priority**: The menu is the game entry point; a bad layout file must not strand development or players.

**Independent Test**: Can be tested by running the game with the layout file missing and again with invalid JSON, then confirming the existing hardcoded menu still works.

**Acceptance Scenarios**:

1. **Given** missing layout JSON, **When** the game starts, **Then** the existing hardcoded menu still works and a clear warning is logged.
2. **Given** invalid layout JSON, **When** the game starts, **Then** the existing hardcoded menu still works and a clear warning is logged.
3. **Given** a layout with an unsupported button action, **When** the game loads the layout, **Then** the game treats the layout as invalid for runtime use and falls back with a clear warning.
4. **Given** a layout with overlapping enabled button masks, **When** the game loads the layout, **Then** the game treats the layout as invalid for runtime use and falls back with a clear warning.

---

### User Story 5 - Export a Stable Main Menu Layout Contract (Priority: P2)

A developer can inspect the exported JSON and understand which background, canvas, text boxes, and button masks the game will load.

**Why this priority**: A stable contract lets the editor and runtime evolve without hidden local-machine paths or fragile coordinate assumptions.

**Independent Test**: Can be tested by exporting a layout and validating that it contains only repo-relative paths or content asset keys, a layout version, canvas dimensions, background asset key, and recognized element records.

**Acceptance Scenarios**:

1. **Given** a saved main-menu layout, **When** the user inspects the JSON, **Then** it stores a layout version, canvas width and height, background asset key, and a list of text and button mask elements.
2. **Given** the user exports the layout, **When** the file is written, **Then** the editor saves it under a runtime-loadable repo/content path such as `Content/UI/MenuLayouts/main-menu.json`.
3. **Given** an image opened from a local absolute path, **When** the layout is exported, **Then** the exported layout stores a game content asset key or repo-relative path only.

### Edge Cases

- The editor opens a layout whose background image is missing or no longer matches the stored asset key.
- The editor opens a layout with duplicate element ids.
- The user tries to export a button mask without an id or without one of the four MVP actions.
- The user tries to export a text box without an id, bounds, or display text.
- The user resizes an element to zero or negative width or height.
- The user drags or resizes an element beyond the 1536x1024 canvas edge.
- The game loads a layout whose canvas size differs from 1536x1024.
- The game viewport differs from 1536x1024 while the loaded layout was authored on a 1536x1024 canvas.
- The game loads a layout that contains unknown element types from a future version.
- The game loads a layout with disabled button masks or empty optional labels.
- The editor or game encounters overlapping enabled button masks.
- The editor preview font differs from runtime rendering.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The editor MUST be a standalone desktop application separate from the game runtime.
- **FR-002**: The MVP MUST target the Star Smuggler main menu layout only.
- **FR-003**: The editor MUST allow the user to open or import a repo-local main menu image such as `Content/UI/MainMenu.png`.
- **FR-004**: The editor MUST present the menu image on a 1536x1024 design canvas matching the current game window coordinate space.
- **FR-005**: The editor MUST allow users to add, select, move, resize, and delete text boxes.
- **FR-006**: Text boxes MUST store id, text, x, y, width, height, font key, font size or scale, color, and horizontal alignment.
- **FR-007**: The editor MUST allow users to add, select, move, resize, and delete button masks.
- **FR-008**: Button masks MUST store id, action, x, y, width, height, optional editor-only label, and enabled or disabled state.
- **FR-009**: Button mask actions for the MVP MUST be exactly `NewGame`, `LoadGame`, `SaveGame`, and `Quit`.
- **FR-010**: Button masks MUST be visibly outlined in the editor and invisible in the game by default.
- **FR-011**: The editor MUST expose property editing for the selected element and update the design canvas when properties change.
- **FR-012**: The editor MUST provide clear visual selection, dragging, resizing, invalid-file feedback, validation feedback, and save/export success or failure feedback.
- **FR-013**: The editor MUST save and reopen layouts without losing element ids, bounds, styling properties, actions, labels, enabled states, or background references.
- **FR-014**: The editor MUST export the main menu layout to a JSON file under a runtime-loadable repo/content path such as `Content/UI/MenuLayouts/main-menu.json`.
- **FR-015**: Exported JSON MUST store a layout version, canvas width, canvas height, background image content asset key, and layout elements as text and button mask records.
- **FR-016**: Exported JSON MUST use repo-relative paths or game content asset keys only and MUST NOT store local absolute paths.
- **FR-017**: The game MUST load the exported layout for `MainMenuScreen` when a valid layout exists.
- **FR-018**: The game MUST map loaded button mask actions to the existing main menu behavior for new game, load game, save game, and quit.
- **FR-019**: If layout JSON is missing, unreadable, malformed, semantically invalid, or contains unsupported MVP actions, the game MUST fall back to the existing hardcoded main menu behavior.
- **FR-020**: Runtime fallback MUST log a clear warning that distinguishes missing layout, invalid JSON, invalid schema/content, and unsupported action where possible.
- **FR-021**: The editor preview MAY approximate runtime font rendering, but the feature MUST document that runtime output is authoritative.
- **FR-022**: The MVP MUST NOT require npm or a web stack.
- **FR-023**: The MVP MUST NOT include every game screen, polygon or alpha-based masks, animation timelines, audio editing, full content-pipeline management, or action-to-game-logic wiring beyond the four main menu actions.
- **FR-024**: Runtime rendering and hit-testing MUST convert layout bounds from the stored design canvas to the current viewport by scaling X and Y coordinates independently.
- **FR-025**: The editor MUST clamp text box and button mask bounds within the stored design canvas during drag and resize interactions.
- **FR-026**: Text box and button mask ids MUST be globally unique across the entire layout.
- **FR-027**: The game MUST NOT render button mask labels; visible runtime text MUST come from text boxes or menu artwork.
- **FR-028**: Enabled button masks MUST NOT overlap; editor validation MUST block save/export for overlapping enabled masks, and runtime validation MUST treat such layouts as invalid.

### Constitution Alignment *(mandatory)*

- **Code Quality & Comments**: The implementation plan must call out comments for the JSON schema/contract, runtime loader fallback paths, design-canvas coordinate conversion, and editor design-surface selection/drag/resize logic. Runtime menu loading must remain cohesive with existing menu behavior and must not make the editor UI framework a game dependency.
- **TDD Signal**: Before production behavior changes, define failing JSON contract tests, runtime loader fallback tests, and a manual editor smoke reproduction that initially fails because no editor/exported layout path exists.
- **Testing Scope**: Required validation includes build, JSON contract tests, runtime loader tests for valid/missing/invalid layouts, action mapping tests for all four actions where practical, and documented manual editor smoke tests for open image, add element, edit properties, drag, resize, delete, save, reopen, and export.
- **UX Consistency**: The editor must provide predictable selection, drag handles, resize handles, property editing, invalid file feedback, validation errors, save/export feedback, and disabled-state editing. The game menu must preserve existing main menu action outcomes and player-facing behavior.
- **Performance Requirement**: The editor must not reload the menu image during drag or resize operations. The game runtime must avoid unnecessary per-frame allocations while drawing or hit-testing the loaded layout and must not depend on the editor UI framework at runtime.
- **Coordinate Conversion**: Runtime coordinate conversion must use the stored canvas size as the source coordinate space and independently scale X and Y values to the current viewport so hit masks remain aligned with the full-window background.
- **Design-Surface Bounds**: Editor drag and resize behavior must clamp element bounds to the stored design canvas so saved layouts contain no off-canvas element rectangles.
- **Hit-Region Ambiguity**: Enabled button masks must not overlap so runtime clicks never depend on hidden ordering rules.

### Key Entities *(include if feature involves data)*

- **Menu Layout**: A versioned main-menu layout document with canvas width, canvas height, background asset key, and ordered layout elements.
- **Text Box Element**: A rectangular text element with id, text, x/y position, width, height, font key, font size or scale, color, and horizontal alignment.
- **Button Mask Element**: A rectangular invisible runtime hit region with id, action, x/y position, width, height, optional editor-only label, and enabled state.
- **Button Action**: One of the four allowed MVP actions: `NewGame`, `LoadGame`, `SaveGame`, or `Quit`.
- **Background Asset Reference**: A repo-relative image reference or game content asset key such as `UI/MainMenu`; it must be portable across machines and must not be an absolute local path.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A developer/designer can align the four main-menu hit regions, assign actions, export the layout, and reopen it without editing C# coordinates.
- **SC-002**: A valid exported layout preserves 100% of required text box and button mask fields after save and reopen in the editor.
- **SC-003**: The game can start with a valid exported layout and all four supported button actions continue to trigger their existing outcomes.
- **SC-004**: The game can start with missing and invalid layout files, continue using the existing hardcoded menu, and emit a clear warning for each case.
- **SC-005**: Manual editor smoke testing confirms image open, add, select, move, resize, delete, edit properties, save, reopen, and export flows complete without restarting the editor.
- **SC-006**: Runtime menu drawing and hit-testing remain responsive enough that users do not perceive input lag or frame hitches on the target desktop profile.

## Assumptions

- The main menu canvas remains 1536x1024 for the MVP, even if the editor window itself can scale the preview.
- Runtime coordinates are authored in the stored canvas coordinate space and independently scaled to the current viewport during rendering and hit-testing.
- Text boxes and button masks are always saved with bounds inside the stored design canvas.
- Element ids are unique across all text boxes and button masks in a layout.
- Button mask labels are editor annotations only; runtime-visible text is supplied by text boxes or menu artwork.
- Enabled button masks do not overlap within a valid layout.
- `UI/MainMenu` is the default game content asset key for the menu background.
- `Content/UI/MenuLayouts/main-menu.json` is the preferred exported layout path unless planning identifies an equally runtime-appropriate content path.
- The current hardcoded menu in `MainMenuScreen` remains the fallback behavior and source of existing action semantics.
- Editor users are trusted project contributors working with repo-local assets.
- The first version only needs rectangular text boxes and rectangular button masks.
- Runtime text rendering is authoritative; editor preview differences are acceptable when documented.
