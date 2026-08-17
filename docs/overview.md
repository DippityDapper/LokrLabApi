# LokrLabApi — Overview

Contracts-only plugin for the LokrLab editor shell. Other plugins register
project types, workspaces, node-tree contributors, inspector drawers, and
shell menus here. LokrLab (the first-party suite) and any third-party
editor depend on it so editor contracts do not live in the runtime
content-loading plugin. Phase 9 adds `JumpToProject` /
`ReturnToPreviousProject` / `RequestRefresh` (shell-assigned). Phase 9.5
adds `Host`, lab-scene events, persistent inspectors, Project Browser
hooks, and embed delegates (`StartEmbeddedScene` / `StartEmbeddedFight`
and camera / stop / is-active) so a third-party type can start a fight
without owning embed Harmony. `RegisterMenuItem` takes
`isVisible` so items appear only in their session / workspace.

No Harmony patches and no rendering. Inspector/toolbar builders take a
`Transform`, not a SimpleUI type, so this assembly does not reference
SimpleUI.

## In this folder

- [`overview.md`](overview.md) — this file
- [`layout.md`](layout.md) — file structure and namespace
- [`architecture.md`](architecture.md) — facade, session, why a separate plugin
- [`classes.md`](classes.md) — every public type
- [`conventions.md`](conventions.md) — naming and structural patterns
- [`cross-references.md`](cross-references.md) — neighboring plugins

## Plugin metadata

`LokrLabApiPlugin.cs`: `Guid = "com.lokrmodding.labapi"`,
`Name = "LoKR Lab API"`, `Version = "1.5.3"`. No `[BepInDependency]` — a
library plugin. `Awake()` logs plugin load and caches the logger.

## Quick example

```csharp
ProjectTypeRegistration type = LokrLabApi.RegisterProjectType(
    "character", "Character", "character", charactersRoot);
type.CreateNew = () => /* return a ProjectSession or null if cancelled */;
type.BuildCreateSheet = parent => /* optional New Project fields */;
type.CommitCreateSheet = () => /* error string, or null to create */;
type.Load = folder => /* load folder into a ProjectSession */;

LokrLabApi.RegisterMenu("File");
LokrLabApi.RegisterMenuItem("File", "Close Project", OnCloseProject);
type.RegisterBottomPanel("Timeline", "Timeline", parent => /* build */, isRelevant);

type.RegisterInspectorDrawer("Part", (node, session, parent) => { /* build UI */ });
type.RegisterInspectorSection("Part", (node, session, parent) => { /* extra block */ });
```
