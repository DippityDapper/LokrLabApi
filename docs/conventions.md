# LokrLabApi — Conventions

- **Contracts only.** No Harmony, no disk IO, no UI widgets. IO
  (`project.json`) and chrome (`UiDockSpace`) live in LokrLab.
- **Facade + registration object.** Cross-cutting state
  (`CurrentSession`, menus) sits on `LokrLabApi`. Per-type registries sit
  on `ProjectTypeRegistration` so a type cannot register onto another
  type by accident.
- **`Transform`, not SimpleUI.** Any builder the shell will parent
  widgets under takes `UnityEngine.Transform`.
- **Kind is a string.** `LabNode.Kind` is not an enum. New node kinds
  are just new strings plus a factory/drawer registration.
- **Replace-by-id.** `RegisterProjectType`, `RegisterMenu`,
  `RegisterWorkspace`, and `RegisterMenuItem` replace an existing entry
  of the same id/name/label rather than stacking duplicates.
- **XML docs on every public/internal member.** Same rule as the rest of
  the solution: `/// <summary>` on every member; `<remarks>` for
  non-obvious why.
