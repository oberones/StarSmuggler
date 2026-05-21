# Tasks: Main Menu Layout Editor

**Input**: Design documents from `specs/001-menu-layout-editor/`
**Prerequisites**: [plan.md](./plan.md), [spec.md](./spec.md), [research.md](./research.md), [data-model.md](./data-model.md), [contracts/main-menu-layout.schema.json](./contracts/main-menu-layout.schema.json), [quickstart.md](./quickstart.md)

**Tests**: Required by the feature specification and constitution. Automated tests should be written first where practical; manual red/green smoke tasks cover Avalonia UI interactions and MonoGame runtime clicks.

**Organization**: Tasks are grouped by user story so each story can be implemented and validated independently after shared setup/foundation.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel with other [P] tasks in the same phase because it touches different files and has no dependency on incomplete tasks.
- **[Story]**: Maps to a user story from [spec.md](./spec.md); setup, foundational, and polish tasks intentionally omit story labels.
- Every task includes at least one exact file path.

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create the solution/project structure needed by all stories.

- [X] T001 Create `StarSmuggler.MenuLayouts/StarSmuggler.MenuLayouts.csproj` as a .NET 8 class library.
- [X] T002 Create `StarSmuggler.Editor/StarSmuggler.Editor.csproj` as a .NET 8 Avalonia desktop app.
- [X] T003 Create `StarSmuggler.Tests/StarSmuggler.Tests.csproj` as a .NET 8 xUnit test project.
- [X] T004 Add `StarSmuggler.MenuLayouts/StarSmuggler.MenuLayouts.csproj`, `StarSmuggler.Editor/StarSmuggler.Editor.csproj`, and `StarSmuggler.Tests/StarSmuggler.Tests.csproj` to `StarSmuggler.sln`.
- [X] T005 Configure project references so `StarSmuggler.csproj`, `StarSmuggler.Editor/StarSmuggler.Editor.csproj`, and `StarSmuggler.Tests/StarSmuggler.Tests.csproj` reference `StarSmuggler.MenuLayouts/StarSmuggler.MenuLayouts.csproj`.
- [X] T006 Configure `StarSmuggler.Editor/StarSmuggler.Editor.csproj` with Avalonia package references and confirm no Avalonia references are added to `StarSmuggler.csproj`.
- [X] T007 Create `Content/UI/MenuLayouts/.gitkeep` so the runtime layout export directory exists before the first JSON export.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish shared DTOs, validation primitives, serialization primitives, and runtime content-copy behavior required by all user stories.

**Critical**: No user story work should begin until this phase is complete.

- [X] T008 [P] Add `MenuLayoutElementType`, `MenuButtonAction`, and `HorizontalTextAlignment` enums in `StarSmuggler.MenuLayouts/MenuLayoutEnums.cs`.
- [X] T009 [P] Add shared rectangle value type for source-canvas pixel bounds in `StarSmuggler.MenuLayouts/MenuLayoutRect.cs`.
- [X] T010 [P] Add `MenuLayoutDocument`, `MenuLayoutElement`, `TextElement`, and `ButtonMaskElement` DTOs in `StarSmuggler.MenuLayouts/MenuLayoutDocument.cs`.
- [X] T011 [P] Add `MenuLayoutValidationIssue` and `MenuLayoutValidationResult` models in `StarSmuggler.MenuLayouts/MenuLayoutValidationResult.cs`.
- [X] T012 [P] Add `MenuLayoutLoadResult` and `MenuLayoutFallbackReason` models in `StarSmuggler.MenuLayouts/MenuLayoutLoadResult.cs`.
- [X] T013 Add deterministic `System.Text.Json` options, polymorphic element serialization, and indented write helpers in `StarSmuggler.MenuLayouts/MenuLayoutJson.cs`.
- [X] T014 Add the first-pass `MenuLayoutValidator` shell with version, canvas, path, id, bounds, action, and overlap validation entry points in `StarSmuggler.MenuLayouts/MenuLayoutValidator.cs`.
- [X] T015 Add file load/save helpers that return load results without throwing for common fallback cases in `StarSmuggler.MenuLayouts/MenuLayoutLoader.cs`.
- [X] T016 Add coordinate scaling helpers for source-canvas to viewport rectangles in `StarSmuggler.MenuLayouts/CoordinateScaler.cs`.
- [X] T017 Add intent-level comments for schema versioning, JSON naming, source-canvas coordinates, and fallback result semantics in `StarSmuggler.MenuLayouts/MenuLayoutDocument.cs`, `StarSmuggler.MenuLayouts/MenuLayoutJson.cs`, and `StarSmuggler.MenuLayouts/MenuLayoutLoader.cs`.
- [X] T018 Configure `StarSmuggler.csproj` to copy `Content/UI/MenuLayouts/*.json` to output without adding those JSON files to `Content/Content.mgcb`.
- [X] T019 Add shared test builders and fixture helpers for valid main-menu layouts in `StarSmuggler.Tests/MenuLayouts/MenuLayoutTestData.cs`.

**Checkpoint**: The solution has all projects, references, shared contract skeletons, JSON copy behavior, and test helpers ready for user-story implementation.

---

## Phase 3: User Story 1 - Open Main Menu Artwork (Priority: P1)

**Goal**: A developer/designer can open repo-local main menu art in the standalone editor and view it on a 1536x1024 design canvas.

**Independent Test**: Start the editor, open `Content/UI/MainMenu.png`, associate `UI/MainMenu`, and verify the image appears on the 1536x1024 canvas with invalid-file feedback for a missing image.

### Tests for User Story 1 (write first)

- [X] T020 [P] [US1] Add failing service tests for content asset key normalization and absolute-path rejection in `StarSmuggler.Tests/MenuLayouts/MenuLayoutPathTests.cs`.
- [X] T021 [P] [US1] Document the manual red/green open-image smoke path in `specs/001-menu-layout-editor/quickstart.md`.

### Implementation for User Story 1

- [X] T022 [P] [US1] Create Avalonia application entry files in `StarSmuggler.Editor/Program.cs`, `StarSmuggler.Editor/App.axaml`, and `StarSmuggler.Editor/App.axaml.cs`.
- [X] T023 [P] [US1] Create the main editor shell view in `StarSmuggler.Editor/Views/MainWindow.axaml` and `StarSmuggler.Editor/Views/MainWindow.axaml.cs`.
- [X] T024 [P] [US1] Create `MainWindowViewModel` with document state, selected background path, background asset key, and validation messages in `StarSmuggler.Editor/ViewModels/MainWindowViewModel.cs`.
- [X] T025 [P] [US1] Implement `ImageImportService` to open repo-local images and normalize `Content/UI/MainMenu.png` to `UI/MainMenu` in `StarSmuggler.Editor/Services/ImageImportService.cs`.
- [X] T026 [US1] Implement toolbar commands for open image and open layout in `StarSmuggler.Editor/ViewModels/MainWindowViewModel.cs`.
- [X] T027 [US1] Implement the 1536x1024 image design canvas in `StarSmuggler.Editor/Views/DesignSurface.axaml` and `StarSmuggler.Editor/Views/DesignSurface.axaml.cs`.
- [X] T028 [US1] Show invalid, missing, or unreadable image feedback without replacing the current draft in `StarSmuggler.Editor/ViewModels/MainWindowViewModel.cs`.
- [X] T029 [US1] Add intent-level comments for image import assumptions and design-surface source-canvas behavior in `StarSmuggler.Editor/Services/ImageImportService.cs` and `StarSmuggler.Editor/Views/DesignSurface.axaml.cs`.
- [ ] T030 [US1] Run the US1 manual open-image smoke test from `specs/001-menu-layout-editor/quickstart.md`.

**Checkpoint**: User Story 1 works independently: editor opens the menu image on the correct canvas and reports invalid image errors.

---

## Phase 4: User Story 2 - Author Text and Button Regions (Priority: P1)

**Goal**: A developer/designer can add, select, move, resize, edit, delete, validate, save, and reopen text boxes and button masks.

**Independent Test**: Create one text box and one `NewGame` button mask, drag/resize them, confirm clamping and overlap validation, save, reopen, and verify all fields persisted.

### Tests for User Story 2 (write first)

- [X] T031 [P] [US2] Add failing validation tests for missing ids, duplicate ids, zero/negative sizes, out-of-canvas bounds, unsupported actions, invalid text font scale/color/alignment, and overlapping enabled masks in `StarSmuggler.Tests/MenuLayouts/MenuLayoutValidationTests.cs`.
- [X] T032 [P] [US2] Add failing JSON round-trip tests for text boxes and button masks in `StarSmuggler.Tests/MenuLayouts/MenuLayoutJsonTests.cs`.
- [X] T033 [P] [US2] Document the manual red/green author-save-reopen smoke path in `specs/001-menu-layout-editor/quickstart.md`.

### Implementation for User Story 2

- [X] T034 [US2] Complete validation rules in `StarSmuggler.MenuLayouts/MenuLayoutValidator.cs`.
- [X] T035 [US2] Complete polymorphic JSON read/write for `Text` and `ButtonMask` elements in `StarSmuggler.MenuLayouts/MenuLayoutJson.cs`.
- [X] T036 [P] [US2] Add layout file save/open services for `Content/UI/MenuLayouts/main-menu.json` in `StarSmuggler.Editor/Services/LayoutFileService.cs`.
- [X] T037 [P] [US2] Add editable element view models in `StarSmuggler.Editor/ViewModels/LayoutElementViewModel.cs`.
- [X] T038 [P] [US2] Add validation message view models in `StarSmuggler.Editor/ViewModels/ValidationMessageViewModel.cs`.
- [X] T039 [US2] Implement add text, add button mask, delete selected, save layout, and export layout commands in `StarSmuggler.Editor/ViewModels/MainWindowViewModel.cs`.
- [X] T040 [US2] Implement visible outlines, selection state, and selected-element adorners in `StarSmuggler.Editor/Views/DesignSurface.axaml` and `StarSmuggler.Editor/Views/DesignSurface.axaml.cs`.
- [X] T041 [US2] Implement drag-to-move and resize handles with canvas-bound clamping in `StarSmuggler.Editor/Views/DesignSurface.axaml.cs`.
- [X] T042 [US2] Implement selected-element property editing for text boxes and button masks in `StarSmuggler.Editor/Views/PropertyPanel.axaml`.
- [X] T043 [US2] Block save/export and show validation errors when required properties are missing, ids duplicate, bounds are invalid, actions are unsupported, or enabled masks overlap in `StarSmuggler.Editor/ViewModels/MainWindowViewModel.cs`.
- [X] T044 [US2] Add intent-level comments for selection, drag, resize, clamp, and validation feedback behavior in `StarSmuggler.Editor/Views/DesignSurface.axaml.cs` and `StarSmuggler.Editor/ViewModels/MainWindowViewModel.cs`.
- [ ] T045 [US2] Run the US2 manual author-save-reopen smoke test from `specs/001-menu-layout-editor/quickstart.md`.

**Checkpoint**: User Story 2 works independently: the editor can author valid layouts and blocks invalid exports before runtime is involved.

---

## Phase 5: User Story 3 - Load Layout in the Game (Priority: P1)

**Goal**: `MainMenuScreen` loads a valid exported layout, renders configured text, scales rectangles to the current viewport, hit-tests invisible masks, and dispatches existing actions.

**Independent Test**: Place a valid `main-menu.json`, run the game, verify layout rendering and all four action hit regions.

### Tests for User Story 3 (write first)

- [X] T046 [P] [US3] Add failing valid-layout loader tests, including disabled button masks being ignored during hit testing, in `StarSmuggler.Tests/Runtime/MainMenuLayoutLoaderTests.cs`.
- [X] T047 [P] [US3] Add failing coordinate-scaling tests from 1536x1024 to an alternate viewport in `StarSmuggler.Tests/MenuLayouts/CoordinateScalerTests.cs`.
- [X] T048 [P] [US3] Document the manual red/green runtime action-dispatch smoke path in `specs/001-menu-layout-editor/quickstart.md`.

### Implementation for User Story 3

- [X] T049 [US3] Complete source-canvas to viewport scaling implementation in `StarSmuggler.MenuLayouts/CoordinateScaler.cs`.
- [X] T050 [US3] Add runtime-facing layout load helpers for valid layouts in `StarSmuggler.MenuLayouts/MenuLayoutLoader.cs`.
- [X] T051 [US3] Refactor `Screens/MainMenuScreen.cs` to load and cache a valid `Content/UI/MenuLayouts/main-menu.json` layout during `LoadContent` or `Refresh`.
- [X] T052 [US3] Render layout background and text elements with existing SpriteFont assets while applying font scale, color, and horizontal alignment in `Screens/MainMenuScreen.cs`.
- [X] T053 [US3] Convert enabled button mask rectangles to current viewport rectangles and use only enabled masks for hit testing in `Screens/MainMenuScreen.cs`.
- [X] T054 [US3] Map `NewGame`, `LoadGame`, `SaveGame`, and `Quit` layout actions to the existing main menu behavior in `Screens/MainMenuScreen.cs`.
- [X] T055 [US3] Cache layout data, font lookups, scaled rectangles, and viewport dimensions so `Screens/MainMenuScreen.cs` does not reload JSON or assets per frame.
- [X] T056 [US3] Add intent-level comments for runtime loader use, coordinate scaling, and action dispatch in `Screens/MainMenuScreen.cs`.
- [ ] T057 [US3] Run the US3 manual runtime hit-region smoke test from `specs/001-menu-layout-editor/quickstart.md`.

**Checkpoint**: User Story 3 works independently with a valid layout file and preserves all existing menu actions.

---

## Phase 6: User Story 4 - Preserve Safe Fallback Behavior (Priority: P2)

**Goal**: Missing, malformed, unsupported, or invalid layout JSON falls back to the existing hardcoded menu and logs a clear warning.

**Independent Test**: Run the game with missing and invalid `main-menu.json`; the old hardcoded menu remains playable and logs the fallback reason.

### Tests for User Story 4 (write first)

- [X] T058 [P] [US4] Add failing missing-layout fallback tests in `StarSmuggler.Tests/Runtime/MainMenuLayoutLoaderTests.cs`.
- [X] T059 [P] [US4] Add failing invalid-layout fallback tests for malformed JSON, unsupported version, unsupported action, unknown element type, duplicate ids, and overlapping enabled masks in `StarSmuggler.Tests/Runtime/MainMenuLayoutLoaderTests.cs`.
- [X] T060 [P] [US4] Document the manual red/green fallback smoke path in `specs/001-menu-layout-editor/quickstart.md`.

### Implementation for User Story 4

- [X] T061 [US4] Complete fallback reason classification and warning messages in `StarSmuggler.MenuLayouts/MenuLayoutLoader.cs`.
- [X] T062 [US4] Preserve the existing hardcoded button/label layout as fallback state in `Screens/MainMenuScreen.cs`.
- [X] T063 [US4] Log clear fallback warnings for missing files, invalid JSON, invalid schema/content, unsupported actions, unsupported versions, and overlapping masks in `Screens/MainMenuScreen.cs`.
- [X] T064 [US4] Ensure fallback save behavior still refuses stranded games and preserves existing active-game checks in `Screens/MainMenuScreen.cs`.
- [X] T065 [US4] Add intent-level comments for hardcoded fallback invariants in `Screens/MainMenuScreen.cs`.
- [ ] T066 [US4] Run the US4 missing/invalid-layout manual smoke test from `specs/001-menu-layout-editor/quickstart.md`.

**Checkpoint**: User Story 4 works independently: bad layout files cannot break the playable main menu.

---

## Phase 7: User Story 5 - Export a Stable Main Menu Layout Contract (Priority: P2)

**Goal**: The exported JSON contract is stable, inspectable, deterministic, repo-relative, and documented for editor/runtime compatibility.

**Independent Test**: Export a layout, inspect the JSON for the required properties and no absolute paths, validate it against shared rules, and confirm the schema/documentation match the generated output.

### Tests for User Story 5 (write first)

- [X] T067 [P] [US5] Add failing deterministic JSON formatting and PascalCase property-name tests in `StarSmuggler.Tests/MenuLayouts/MenuLayoutJsonTests.cs`.
- [X] T068 [P] [US5] Add failing no-absolute-path export tests in `StarSmuggler.Tests/MenuLayouts/MenuLayoutPathTests.cs`.
- [X] T069 [P] [US5] Add failing schema example compatibility tests against `specs/001-menu-layout-editor/contracts/main-menu-layout.schema.json` in `StarSmuggler.Tests/MenuLayouts/MenuLayoutSchemaTests.cs`.

### Implementation for User Story 5

- [X] T070 [US5] Add or update the canonical sample exported layout in `Content/UI/MenuLayouts/main-menu.json`.
- [X] T071 [US5] Ensure `MenuLayoutJson` writes indented deterministic JSON with PascalCase properties in `StarSmuggler.MenuLayouts/MenuLayoutJson.cs`.
- [X] T072 [US5] Ensure editor export writes only repo-relative paths or content asset keys in `StarSmuggler.Editor/Services/LayoutFileService.cs`.
- [X] T073 [US5] Update schema details to match DTO and serializer behavior in `specs/001-menu-layout-editor/contracts/main-menu-layout.schema.json`.
- [X] T074 [US5] Add schema/versioning and coordinate-contract comments in `StarSmuggler.MenuLayouts/MenuLayoutDocument.cs` and `StarSmuggler.MenuLayouts/MenuLayoutJson.cs`.
- [ ] T075 [US5] Run the US5 export-contract inspection steps from `specs/001-menu-layout-editor/quickstart.md`.

**Checkpoint**: User Story 5 works independently: exported JSON is stable, portable, documented, and accepted by both editor and runtime validation.

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Final validation, cleanup, documentation, and project-wide quality checks.

- [X] T076 [P] Update `docs/NOTES.md` with the editor workflow, runtime layout path, fallback behavior, and the rule that editor font preview is approximate while runtime rendering is authoritative.
- [X] T077 [P] Update `docs/BACKLOG.md` with post-MVP layout editor ideas such as multi-screen editing, polygon masks, audio, and animation timelines.
- [X] T078 [P] Review comments for accuracy in `StarSmuggler.MenuLayouts/MenuLayoutDocument.cs`, `StarSmuggler.MenuLayouts/MenuLayoutValidator.cs`, `StarSmuggler.MenuLayouts/MenuLayoutLoader.cs`, `StarSmuggler.Editor/Views/DesignSurface.axaml.cs`, and `Screens/MainMenuScreen.cs`.
- [X] T079 Run `dotnet build StarSmuggler.sln` from `/Users/oberon/Projects/coding/monogame/StarSmuggler/StarSmuggler.sln`.
- [X] T080 Run `dotnet test StarSmuggler.Tests/StarSmuggler.Tests.csproj` from `/Users/oberon/Projects/coding/monogame/StarSmuggler/StarSmuggler.Tests/StarSmuggler.Tests.csproj`.
- [ ] T081 Run final manual smoke validation from `specs/001-menu-layout-editor/quickstart.md`.
- [X] T082 Run `git diff --check` from `/Users/oberon/Projects/coding/monogame/StarSmuggler`.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 Setup**: No dependencies.
- **Phase 2 Foundational**: Depends on Phase 1 and blocks all user stories.
- **Phase 3 US1**: Depends on Phase 2.
- **Phase 4 US2**: Depends on Phase 2 and benefits from US1 editor shell, but can be tested with synthetic documents.
- **Phase 5 US3**: Depends on Phase 2 and a valid layout fixture; can be implemented before the editor is complete by using test JSON.
- **Phase 6 US4**: Depends on Phase 2 and the runtime loader shape from US3.
- **Phase 7 US5**: Depends on Phase 2 and serializer/export behavior from US2.
- **Phase 8 Polish**: Depends on whichever user stories are targeted for the delivery checkpoint.

### User Story Dependencies

- **US1 Open Main Menu Artwork**: No other user-story dependency after foundational setup.
- **US2 Author Text and Button Regions**: Uses the editor shell from US1 for the natural product path; shared validation tests can start after foundational setup.
- **US3 Load Layout in the Game**: Does not require the editor UI if tests provide valid JSON fixtures.
- **US4 Preserve Safe Fallback Behavior**: Requires the runtime loader integration from US3.
- **US5 Export Stable Contract**: Requires shared serialization and editor save/export services from US2.

### Within Each User Story

- Write automated tests or manual red/green smoke notes first.
- Implement shared or view-model logic before UI wiring where practical.
- Add intent-level comments with the behavior they explain.
- Validate the independent test before moving to the next checkpoint.

---

## Parallel Opportunities

- Setup tasks T001, T002, and T003 can be split across projects, then T004-T007 integrate them.
- Foundational DTO/model tasks T008-T012 can run in parallel before serializer/validator tasks T013-T016.
- US1 tasks T022-T025 can run in parallel after the editor project exists.
- US2 test tasks T031-T033 can run in parallel; view model, service, and view tasks T036-T038 can also run in parallel.
- US3 tests T046-T048 can run in parallel before runtime implementation.
- US4 fallback tests T058-T060 can run in parallel.
- US5 contract tests T067-T069 can run in parallel.
- Polish docs tasks T076 and T077 can run in parallel with comment review T078.

---

## Parallel Example: User Story 2

```text
Task: T031 Add failing validation tests in StarSmuggler.Tests/MenuLayouts/MenuLayoutValidationTests.cs
Task: T032 Add failing JSON round-trip tests in StarSmuggler.Tests/MenuLayouts/MenuLayoutJsonTests.cs
Task: T033 Document manual author-save-reopen smoke path in specs/001-menu-layout-editor/quickstart.md
```

```text
Task: T036 Add layout file save/open services in StarSmuggler.Editor/Services/LayoutFileService.cs
Task: T037 Add editable element view models in StarSmuggler.Editor/ViewModels/LayoutElementViewModel.cs
Task: T038 Add validation message view models in StarSmuggler.Editor/ViewModels/ValidationMessageViewModel.cs
```

## Parallel Example: User Story 3

```text
Task: T046 Add valid-layout loader tests in StarSmuggler.Tests/Runtime/MainMenuLayoutLoaderTests.cs
Task: T047 Add coordinate-scaling tests in StarSmuggler.Tests/MenuLayouts/CoordinateScalerTests.cs
Task: T048 Document runtime action-dispatch smoke path in specs/001-menu-layout-editor/quickstart.md
```

---

## Implementation Strategy

### MVP First

1. Complete Phase 1 and Phase 2.
2. Complete US1 to prove the standalone editor can open menu art.
3. For a practical authoring MVP, continue through US2 so the editor can save/reopen text boxes and button masks.
4. Stop and validate US1/US2 independently with the quickstart smoke paths.

### Runtime Integration Increment

1. Complete US3 with test JSON fixtures before relying on editor-authored output.
2. Complete US4 so invalid or missing layouts never break the main menu.
3. Validate all four runtime actions manually.

### Contract Hardening Increment

1. Complete US5 once editor export and runtime loading exist.
2. Re-run JSON, validation, schema, loader, and coordinate-scaling tests.
3. Finish Phase 8 validation before review.

## Notes

- Tests and manual red/green notes are intentionally first inside each user-story phase.
- [P] tasks use different files and can be assigned independently after their phase dependencies are satisfied.
- Keep Avalonia references inside `StarSmuggler.Editor/StarSmuggler.Editor.csproj` only.
- Keep layout JSON outside `Content/Content.mgcb`; copy it with `StarSmuggler.csproj` content items.
