# LokrLabApi — Cross-references

- **`LokrLab`** — the first-party suite. Depends on this plugin, registers
  Character, Ability Library, and Encounter, assigns Host / embed / fight delegates,
  and renders `Menus` / `CurrentSession` / dock placeholders. See
  [`../../LokrLab/docs/architecture.md`](../../LokrLab/docs/architecture.md),
  [`../../LokrLab/docs/character/overview.md`](../../LokrLab/docs/character/overview.md),
  [`../../LokrLab/docs/ability/overview.md`](../../LokrLab/docs/ability/overview.md).
- **Third-party editors** — depend on this plugin, not `LokrLab.dll`.
- **`LokrCharacterLoader`** — runtime content loading. Deliberately not
  a dependency of this plugin. Character's `CreateNew`/`Load` (in
  LokrLab) talk to `CharacterSession` / `HomeWorkstationScene`,
  which already depend on the loader.
- **`SimpleUI`** — not referenced. The shell uses it; this assembly
  stays widget-free so a project-type plugin can take only the contracts.
- **Editor redesign roadmap** —
  [`../../docs/roadmaps/started/editor-redesign.md`](../../docs/roadmaps/started/editor-redesign.md)
  §2 is the design this plugin implements.
