# LokrLabApi — Layout

```
LokrLabApi/
├── LokrLabApiPlugin.cs
├── LokrLabApi.cs                      (static facade)
├── ProjectSession.cs
├── ProjectReference.cs
├── ProjectTypeRegistration.cs         (plus BottomPanelRegistration)
├── WorkspaceRegistration.cs
├── MenuRegistration.cs                (plus MenuItemRegistration)
├── LabNode.cs
├── EditorSelection.cs
├── LabDelegates.cs                    (NodeTreeContributor, NodeFactory, …)
├── LabHost.cs
├── EmbeddedSceneRequest.cs        (additive scene-in-hole start args)
├── EmbeddedFightRequest.cs        (Stage-hole fight start args)
├── LabSceneContext.cs
└── PersistentInspectorRegistration.cs
```

Everything lives in the root `LokrLabApi` namespace. The static facade
class is also named `LokrLabApi` (same pattern as `CharacterAPI` in
`LokrCharacterLoader`): after `using LokrLabApi;`, callers write
`LokrLabApi.RegisterProjectType(...)`.
