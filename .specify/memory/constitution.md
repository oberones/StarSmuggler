<!--
Sync Impact Report
Version change: 0.0.0 -> 1.0.0
Modified principles:
- Template placeholder -> I. Code Quality and Maintainability
- Template placeholder -> II. Test-Driven Development
- Template placeholder -> III. Testing Standards
- Template placeholder -> IV. User Experience Consistency
- Template placeholder -> V. Performance and Resource Discipline
Added sections:
- Development Standards
- Workflow and Quality Gates
Removed sections:
- Placeholder sections from the generated template
Templates requiring updates:
- ✅ .specify/templates/plan-template.md
- ✅ .specify/templates/spec-template.md
- ✅ .specify/templates/tasks-template.md
- ⚠ .specify/templates/commands/*.md (directory not present in this checkout)
- ✅ README.md
- ✅ CLAUDE.md
Follow-up TODOs:
- None
-->
# Star Smuggler Constitution

## Core Principles

### I. Code Quality and Maintainability

Star Smuggler code MUST favor clear, focused, maintainable implementations over cleverness.
Gameplay rules, screen transitions, persistence behavior, and content-pipeline assumptions MUST be
kept cohesive and easy to audit. New abstractions MUST remove real duplication or isolate a clear
domain concept such as economy, travel, save/load, events, UI components, or audio.

Comments MUST be used liberally to document libraries, public classes, public methods, non-obvious
gameplay formulas, state transitions, asset/content-pipeline dependencies, and save-schema behavior.
Comments MUST explain intent, invariants, and constraints; they MUST be updated when behavior
changes and removed when they become misleading.

Rationale: The project is a growing game codebase where small state or economy mistakes can create
unwinnable runs, broken saves, or confusing UI. Clear code and generous intent-level comments make
future feature work safer.

### II. Test-Driven Development

Behavior changes MUST begin with a test or explicit manual reproduction that fails before the fix
or feature is implemented. Bug fixes MUST include a regression test when the behavior is practical
to automate. If a scenario cannot reasonably be automated because it depends on MonoGame rendering,
audio, or manual input, the implementation plan MUST record the manual red/green validation path.

Code MUST follow a red-green-refactor workflow: define the expected behavior, observe failure,
implement the narrowest fix, then refactor only after the behavior passes. Test gaps are allowed
only when documented with the reason and a concrete manual verification.

Rationale: Trading, travel, random events, and save/load logic interact in ways that are easy to
regress. TDD keeps game-state fixes honest and makes future balancing work safer.

### III. Testing Standards

Every production behavior change MUST pass the project build and relevant focused tests before it
is considered complete. Core logic touching economy calculations, cargo handling, save/load,
random events, game-over rules, and travel costs MUST have automated unit or integration coverage
when practical. UI and audiovisual changes MUST include documented manual playtest scenarios that
cover the affected screen states.

Tests MUST be deterministic wherever possible. Randomness-dependent behavior MUST be isolated,
seeded, or asserted through bounded outcomes. Manual validation MUST include the exact command or
interaction path used, plus any remaining risk.

Rationale: The game currently relies on a playable loop and JSON persistence. Reliable tests and
clear manual scenarios protect the loop while the project adds content and progression systems.

### IV. User Experience Consistency

User-facing behavior MUST be consistent across screens. Buttons, disabled states, invalid-action
feedback, navigation, save/load outcomes, and game-over routing MUST follow one predictable pattern
throughout the main loop. Terminal-inspired UI styling, copy tone, and interaction feedback MUST
remain coherent with the retro-futuristic game identity.

Features MUST define user-visible acceptance criteria before implementation. Any change that can
strand the player, hide state, silently ignore input, or alter save/load outcomes MUST explicitly
state what the player sees and how they recover or continue.

Rationale: Star Smuggler depends on atmosphere and readable decision-making. Consistent feedback
keeps the game feeling intentional instead of brittle.

### V. Performance and Resource Discipline

Runtime changes MUST preserve responsive gameplay on the target MonoGame desktop profile.
Rendering code MUST avoid unnecessary per-frame allocations, asset loads, excessive logging, and
avoidable SpriteBatch state churn. Assets MUST be loaded through the content pipeline at appropriate
screen lifecycle points and reused rather than repeatedly recreated during draw/update loops.

Performance-sensitive changes MUST document expected impact and validation. Visual polish,
animation, audio, and content expansion MUST not degrade input responsiveness or introduce obvious
frame hitches in the main loop.

Rationale: A trading game still needs crisp input, readable screens, and smooth travel presentation.
Performance discipline keeps polish work from making the core loop feel worse.

## Development Standards

Star Smuggler is a .NET 8 and MonoGame project. Features MUST fit the existing screen-based
architecture, reusable UI component approach, JSON save/load model, and content pipeline unless a
plan explicitly justifies a change. Public behavior MUST be expressed in project terms: ports,
zones, cargo, travel, events, saves, screens, audio, and player outcomes.

All feature specs and plans MUST identify affected gameplay state, save/load implications,
test-first strategy, UX acceptance criteria, comment/documentation needs, and performance risks.
New dependencies MUST be justified by concrete value and MUST not replace simple local code without
a maintainability or correctness reason.

## Workflow and Quality Gates

Before implementation, plans MUST pass a constitution check covering code quality, comments,
TDD, testing standards, UX consistency, and performance. During implementation, tests or manual
reproductions MUST be created before production changes whenever practical. Before review, changes
MUST pass the project build, relevant tests, `git diff --check`, and documented manual playtests
for UI or audiovisual behavior.

Pull requests MUST describe behavior changes, validation performed, known test gaps, and any
save/load compatibility effects. Reviewers MUST reject changes that bypass game-over rules,
silently revive invalid saves, introduce inconsistent UI feedback, or leave complex behavior
without comments.

## Governance

This constitution supersedes conflicting local practice for Star Smuggler development. Amendments
MUST update this file, include a Sync Impact Report, and propagate changed requirements to Spec Kit
templates and runtime guidance docs in the same change.

Versioning follows semantic governance:
- MAJOR: incompatible principle removals or redefinitions.
- MINOR: new principles, new mandatory gates, or materially expanded governance.
- PATCH: clarifications, wording fixes, and non-semantic refinements.

Compliance is reviewed during planning and pull request review. Any approved exception MUST be
documented in the feature plan with rationale, rejected alternatives, and a follow-up mitigation
when needed.

**Version**: 1.0.0 | **Ratified**: 2026-05-21 | **Last Amended**: 2026-05-21
