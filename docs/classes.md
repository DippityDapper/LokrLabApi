# LokrLabApi — Classes

## `LokrLabApiPlugin` (`LokrLabApiPlugin.cs`)

```csharp
[BepInPlugin(Guid, Name, Version)]
public class LokrLabApiPlugin : BaseUnityPlugin
```

Passive library plugin. `Awake()` caches `Log` and writes a load line.
No Harmony patches.

## `LokrLabApi` (`LokrLabApi.cs`)

Static facade. See [`architecture.md`](architecture.md).

```csharp
public const string CharacterTypeId = "character";
public const string AbilityLibraryTypeId = "ability-library";
public const string EncounterTypeId = "encounter";

public static ProjectSession CurrentSession { get; set; }
public static EditorSelection Selection { get; }
public static Func<string, ProjectReference> ProjectReferencePicker { get; set; }
public static Action<string, string, string> OpenProject { get; set; }
public static Action ReturnToPreviousProject { get; set; }
public static Func<bool> CanReturnToPreviousProject { get; set; }
public static Action RefreshShell { get; set; }
public static LabHost Host { get; set; }
public static Func<EmbeddedSceneRequest, string> StartEmbeddedScene { get; set; }
public static Action StopEmbeddedScene { get; set; }
public static Func<bool> IsEmbeddedSceneActive { get; set; }
public static Func<Camera> GetEmbeddedSceneCamera { get; set; }
public static Func<EmbeddedFightRequest, string> StartEmbeddedFight { get; set; }
public static Action StopEmbeddedFight { get; set; }
public static Func<bool> IsEmbeddedFightActive { get; set; }
public static Action<string> ImportLegacyFolder { get; set; }
public static Action PromptLegacyImport { get; set; }
public static Func<IReadOnlyList<string>> RecentProjectFolders { get; set; }
public static event Action<LabSceneContext> LabOpened;
public static event Action LabClosing;
public static event Action ShellShown;
public static event Action<string> ScreenShown;
public static IReadOnlyCollection<ProjectTypeRegistration> ProjectTypes { get; }
public static IReadOnlyList<MenuRegistration> Menus { get; }

public static ProjectTypeRegistration RegisterProjectType(string id, string displayName, string iconKey, string folderRoot)
public static ProjectTypeRegistration GetProjectType(string id)
public static ProjectReference PickProjectReference(string projectTypeId)
public static void JumpToProject(string projectTypeId, string folder = null, string selectNodeId = null)
public static void RequestRefresh()
public static void RegisterMenu(string name, int priority = 0)
public static void RegisterMenuItem(string menuName, string label, Action onClick, int priority = 0, Func<bool> isEnabled = null, Func<bool> isVisible = null)
```

`EncounterTypeId` is the many-projects Encounter type. See
[Encounter editor-design](../../LokrLab/docs/encounter/editor-design.md).

`RegisterProjectType` replaces any existing type with the same id.
`RegisterMenu` is idempotent (existing menus keep their items).
`RegisterMenuItem` creates the menu if needed, then replaces by label.
`isEnabled` greys an item; `isVisible` omits it. A top-level menu button
is hidden when none of its items are visible.

## `ProjectSession` (`ProjectSession.cs`)

Abstract handle for the currently open project. Concrete subclasses live
in the plugin that owns the project type (e.g. `CharacterProjectSession`
in LokrLab).

## `ProjectTypeRegistration` (`ProjectTypeRegistration.cs`)

One kind of project the shell can host. Created only through
`LokrLabApi.RegisterProjectType`. Holds `CreateNew` / `Load` plus the
per-type registries (`RegisterWorkspace`, `RegisterNodeTreeContributor`,
`RegisterNodeFactory`, `RegisterInspectorDrawer`,
`RegisterInspectorSection`, `RegisterBottomPanel`). Optional
`BuildCreateSheet` / `CommitCreateSheet` let a type supply its own New
Project fill-out form; the shell parents the sheet under the create wizard.
`Delete` removes a project folder after the shell confirms; singleton types
are not offered a delete button.

`IsSingleton` is for types like Ability Library: one shared project,
always listed as a single row. New Project still offers it so a type can
run its create sheet (a new ability id) and then open the library.

`FindWorkspace(name)` returns the registered workspace or null.

`FactoriesForParent(parentKind)` returns every `RegisterNodeFactory`
entry whose valid-parent list includes that kind (empty kind = tree root).

`FindInspectorDrawer(kind)` returns the highest-priority primary drawer
(or null). `FindInspectorSections(kind)` returns every extra section,
lowest priority first. LokrLab's `InspectorDock` calls both when
selection identity changes; the Animator's `InspectorPanel` stacks
sections only when kind+id changes so playback-tick refresh does not
rebuild them.

## `WorkspaceRegistration` (`WorkspaceRegistration.cs`)

One top-tab workspace on a project type. Replaces today's workstation
screens over Phases 5–8. `BuildToolbar` / `BuildViewport` take a
`Transform`. `RequiresSceneTransition` is reserved for a workspace that
must leave the shell; Sandbox and Ability Lab Stage both embed the fight
in a workspace hole (`StartEmbeddedFight`).

## `MenuRegistration` / `MenuItemRegistration` (`MenuRegistration.cs`)

Top-level shell menus (File / Edit / View / Help) and their items.
LokrLab's `LabMenuBar` renders whatever is registered here. Each item
may supply `IsEnabled` and `IsVisible`; hidden items are omitted from
the dropdown rather than shown greyed out.

## `LabNode` (`LabNode.cs`)

One Node Tree row. `Kind` is an extensible string, not a closed enum.
Presentation-only: contributors rebuild the tree from on-disk files.

## `EditorSelection` (`EditorSelection.cs`)

Shell-level selection. `Primary` is always a member of `All`.
`Set(nodes)` / `Set(node)` / `Clear()` are the only writers — properties
are privately set so callers cannot break the invariant.

## `ProjectReference` (`ProjectReference.cs`)

Read-only cross-project pointer (`projectTypeId` + `projectId`). Encounter
Creator is the first real consumer — see
[encounter-creator.md](../../docs/roadmaps/started/encounter-creator.md).

## Delegates (`LabDelegates.cs`)

`NodeTreeContributor`, `NodeFactory`, `InspectorDrawer`,
`BottomPanelIsRelevant`. `InspectorDrawer` takes a `Transform` so this
plugin does not depend on SimpleUI.

## `BottomPanelRegistration` (`ProjectTypeRegistration.cs`)

One bottom-panel tab on a project type. LokrLab hosts every registered
panel in the bottom dock. `IsRelevant` may auto-focus the tab on
workspace switch; it never hides it. Optional `Refresh` / `Unbind` run
when the shell rebuilds or focuses that tab.

## `LabHost` (`LabHost.cs`)

Shell-assigned live lab context. Null while the lab is closed. Project
types call through `LokrLabApi.Host` instead of referencing `LokrLab.dll`.
`StartEmbeddedScene` / `StopEmbeddedScene` / `IsEmbeddedSceneActive` /
`GetEmbeddedSceneCamera` also live on the static facade (LokrLab assigns
them at plugin load). `StartEmbeddedFight` / `StopEmbeddedFight` /
`IsEmbeddedFightActive` are Character Lab's fight convenience on top of
that. A third-party editor depends on LabApi, not the suite DLL. Start
returns an immediate error string, or null if the additive load began.

## `EmbeddedSceneRequest` (`EmbeddedSceneRequest.cs`)

`BundleId` / `SceneName`, required `Hole` (`RectTransform` with real
height — `Grow()`, `minHeight`, no `ContentSizeFitter`), `FitHud`,
`DisableExtraCameras`, `OnCamera` / `OnReady` / `OnFailed` / `OnEnded`.
LokrLab loads the scene additively and crops `Camera.rect` to the hole.

## `EmbeddedFightRequest` (`EmbeddedFightRequest.cs`)

Caster / optional enemy unit ids, optional `CasterLevel` (1-based rank;
walks `nextLevelArchetype`), required `Hole`, optional
`BindCamera`, `OnReady` / `OnFailed` / `OnEnded`. Character Lab
sets up the ephemeral quest and Sandbox roster spawn, then calls
`StartEmbeddedScene`.

## `LabSceneContext` (`LabSceneContext.cs`)

Passed to `LabOpened`: scene, canvas, font, backdrop camera, `GetScreenRoot`.

## `PersistentInspectorRegistration` (`PersistentInspectorRegistration.cs`)

A form the shell must not rebuild on every selection identity change.
One Grow() host per `Id`. `Matches` / `EnsureBuilt` / `Show` /
`Hide` / optional `Refresh` (same-identity tick). `Scrollable` defaults
true; set false when the inner form owns the only `ScrollRect`.
