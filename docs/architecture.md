# LokrLabApi — Architecture

## Why a separate plugin

`LokrLab` is the first-party suite (host + Character + Ability + Encounter). A
third-party editor must not have to depend
on that assembly. If the contracts lived on LokrLab, those editors would
take a suite dependency (and Character used to create a cycle through
InspectorDock). If they lived on `LokrCharacterLoader`, a runtime
content-loading plugin would own authoring-only types.

`LokrLabApi` is the third option: a passive shared library, same shape as
SimpleUI. LokrLab takes a `[BepInDependency]` on `LokrLabApiPlugin.Guid`;
a third-party editor does the same and does not reference `LokrLab.dll`.

## The facade

`LokrLabApi` (static class) is the only type other plugins need to call:

- `RegisterProjectType` / `GetProjectType` / `ProjectTypes`
- `CurrentSession` — null while the Project Browser is showing
- `Selection` — written by the Node Tree; `InspectorDock` reads it and
  dispatches `FindInspectorDrawer` / `FindInspectorSections`
- `RegisterMenu` / `RegisterMenuItem` / `Menus`
- `ProjectReferencePicker` / `PickProjectReference` — assigned by the
  shell; null until LokrLab boots
- `Host` — shell-assigned live lab context (`LabHost`); null while closed
- `StartEmbeddedScene` / `StopEmbeddedScene` / `IsEmbeddedSceneActive` /
  `GetEmbeddedSceneCamera` — LokrLab assigns these at plugin load (and
  copies them onto Host) so any project type can load an additive scene
  into a hole `RectTransform`
- `StartEmbeddedFight` / `StopEmbeddedFight` / `IsEmbeddedFightActive` —
  LokrLab assigns these at plugin load (and copies them onto Host) so a
  third-party type can start a fight. Fight start calls `StartEmbeddedScene`.
- `LabOpened` / `LabClosing` / `ShellShown` / `ScreenShown`
- `PromptLegacyImport` / `ImportLegacyFolder` / `RecentProjectFolders`

`FindWorkspace(name)` looks up a tab by name. LokrLab's shell calls
`BuildViewport` / `BuildToolbar` for in-shell workspaces (Properties,
Animator, Sandbox setup). `RequiresSceneTransition` is still on the
registration for a future workspace that must leave the shell; Sandbox
setup does not use it.

`ProjectTypeRegistration` then holds per-type registries (workspaces,
node contributors, inspector drawers, bottom panels). Those methods live
on the registration, not the facade, so a type cannot accidentally
register a drawer onto a different type. `BuildCreateSheet` /
`CommitCreateSheet` are optional New Project form callbacks; the shell
parents the sheet under its create wizard.

## No SimpleUI reference

`InspectorDrawer`, `WorkspaceRegistration.BuildToolbar` /
`BuildViewport`, and `BottomPanelRegistration.Builder` all take a
`UnityEngine.Transform`. The shell (LokrLab) parents SimpleUI widgets
under that transform. Keeping SimpleUI out of this assembly means a
future project-type plugin can depend on LokrLabApi without also taking
a SimpleUI dependency if it only needs the contracts.

## Session vs CharacterSession

`ProjectSession` is the project-type-agnostic handle the shell reads
(`Id`, `FolderPath`, `DisplayName`, `IsDirty`). The Character project
type's concrete subclass (`CharacterProjectSession` in LokrLab) wraps
the existing static `CharacterSession` so the pre-redesign Home /
Properties / Animator / Sandbox setup are in-shell workspaces; Timeline /
Checklist / History are registered bottom panels (Phase 6). File Tree
(Phase 7) reads `FolderPath` only — it does not change this contract.
Ability Library is a second `ProjectSession` subclass in the LokrLab
suite; jumps use `LokrLabApi.JumpToProject`. A third-party type uses the
same jump API without referencing `LokrLab.dll`.
