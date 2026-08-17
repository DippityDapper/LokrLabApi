using System;
using System.Collections.Generic;
using UnityEngine;

namespace LokrLabApi
{
	/// <summary>One kind of project the shell can host (Character, Ability Library, Encounter, …).</summary>
	public sealed class ProjectTypeRegistration
	{
		/// <summary>Stable type id (e.g. "character").</summary>
		public string Id { get; }

		/// <summary>Player-facing type name.</summary>
		public string DisplayName { get; }

		/// <summary>Optional icon key for the Project Browser.</summary>
		public string IconKey { get; }

		/// <summary>Default on-disk root where new projects of this type are written.</summary>
		public string FolderRoot { get; }

		/// <summary>True for singleton types (Ability Library): one project, not many.</summary>
		public bool IsSingleton { get; set; }

		/// <summary>Other project type ids this type may cross-reference.</summary>
		public string[] ReferenceableProjectTypes { get; set; } = new string[0];

		/// <summary>Optional ModAPI category name to scan in addition to <see cref="FolderRoot"/> (e.g. "Characters").</summary>
		public string ScanCategory { get; set; }

		/// <summary>Optional folder → display name (e.g. Character profile Name). Null or empty keeps the folder name.</summary>
		public Func<string, string> ResolveDisplayName { get; set; }

		/// <summary>Optional selection sync (e.g. Character Animator viewport tint). The shell calls this from ShowSelection.</summary>
		public Action<IReadOnlyList<LabNode>> OnSelectionChanged { get; set; }

		/// <summary>Optional double-click / activate handler (e.g. jump to a referenced Ability).</summary>
		public Action<LabNode> OnNodeActivated { get; set; }

		/// <summary>Type-specific "New Project" flow. Returns the new session, or null if cancelled.</summary>
		public Func<ProjectSession> CreateNew { get; set; }

		/// <summary>Optional New Project form. The shell parents this under the create wizard so the type can collect name and other init fields.</summary>
		public Action<Transform> BuildCreateSheet { get; set; }

		/// <summary>Validates the create form. Returns an error message, or null to proceed to <see cref="CreateNew"/>.</summary>
		public Func<string> CommitCreateSheet { get; set; }

		/// <summary>Loads a project folder into a session.</summary>
		public Func<string, ProjectSession> Load { get; set; }

		/// <summary>Optional delete. Receives the project folder and returns an error, or null on success. The shell confirms first and skips singleton types.</summary>
		public Func<string, string> Delete { get; set; }

		/// <summary>Optional post-delete hook, called with the deleted folder after a successful <see cref="Delete"/>. Lets the type push the removal into runtime caches (e.g. reload merged content so a vanilla override reverts immediately).</summary>
		public Action<string> OnDeleted { get; set; }

		private readonly List<(WorkspaceRegistration Workspace, int Order)> workspaces = new List<(WorkspaceRegistration, int)>();
		private readonly List<(NodeTreeContributor Contributor, int Priority)> nodeContributors = new List<(NodeTreeContributor, int)>();
		private readonly List<(string Kind, string[] ValidParents, NodeFactory Factory)> nodeFactories = new List<(string, string[], NodeFactory)>();
		private readonly List<(string Kind, InspectorDrawer Drawer, int Priority)> inspectorDrawers = new List<(string, InspectorDrawer, int)>();
		private readonly List<(string Kind, InspectorDrawer Section, int Priority)> inspectorSections = new List<(string, InspectorDrawer, int)>();
		private readonly List<BottomPanelRegistration> bottomPanels = new List<BottomPanelRegistration>();
		private readonly List<PersistentInspectorRegistration> persistentInspectors = new List<PersistentInspectorRegistration>();

		/// <summary>Creates a registration. Callers go through <see cref="LokrLabApi.RegisterProjectType"/>.</summary>
		internal ProjectTypeRegistration(string id, string displayName, string iconKey, string folderRoot)
		{
			Id = id;
			DisplayName = displayName;
			IconKey = iconKey;
			FolderRoot = folderRoot;
		}

		/// <summary>Workspaces registered on this type, lowest priority first.</summary>
		public IReadOnlyList<WorkspaceRegistration> Workspaces
		{
			get
			{
				List<WorkspaceRegistration> result = new List<WorkspaceRegistration>(workspaces.Count);
				workspaces.Sort((a, b) => a.Order.CompareTo(b.Order));
				foreach ((WorkspaceRegistration workspace, int _) in workspaces)
				{
					result.Add(workspace);
				}
				return result;
			}
		}

		/// <summary>Node-tree contributors, lowest priority first.</summary>
		public IReadOnlyList<NodeTreeContributor> NodeTreeContributors
		{
			get
			{
				nodeContributors.Sort((a, b) => a.Priority.CompareTo(b.Priority));
				List<NodeTreeContributor> result = new List<NodeTreeContributor>(nodeContributors.Count);
				foreach ((NodeTreeContributor contributor, int _) in nodeContributors)
				{
					result.Add(contributor);
				}
				return result;
			}
		}

		/// <summary>Bottom panels registered on this type.</summary>
		public IReadOnlyList<BottomPanelRegistration> BottomPanels => bottomPanels;

		/// <summary>Persistent inspector hosts registered on this type.</summary>
		public IReadOnlyList<PersistentInspectorRegistration> PersistentInspectors => persistentInspectors;

		/// <summary>Registers a workspace tab on this project type.</summary>
		public void RegisterWorkspace(WorkspaceRegistration workspace)
		{
			if (workspace == null || string.IsNullOrEmpty(workspace.Name))
			{
				throw new ArgumentException("Workspace must have a name.", nameof(workspace));
			}
			workspaces.RemoveAll(entry => entry.Workspace.Name == workspace.Name);
			workspaces.Add((workspace, workspace.Priority));
		}

		/// <summary>Looks up a workspace by name, or null if none is registered.</summary>
		public WorkspaceRegistration FindWorkspace(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}

			foreach ((WorkspaceRegistration workspace, int _) in workspaces)
			{
				if (workspace.Name == name)
				{
					return workspace;
				}
			}

			return null;
		}

		/// <summary>Registers a Node Tree contributor for this project type.</summary>
		public void RegisterNodeTreeContributor(NodeTreeContributor contributor, int priority = 0)
		{
			if (contributor == null)
			{
				throw new ArgumentNullException(nameof(contributor));
			}
			nodeContributors.Add((contributor, priority));
		}

		/// <summary>Registers a factory that can create a node of the given kind under the listed parent kinds.</summary>
		public void RegisterNodeFactory(string kind, string[] validParentKinds, NodeFactory factory)
		{
			if (string.IsNullOrEmpty(kind))
			{
				throw new ArgumentException("Node kind must be non-empty.", nameof(kind));
			}
			if (factory == null)
			{
				throw new ArgumentNullException(nameof(factory));
			}
			nodeFactories.RemoveAll(entry => entry.Kind == kind);
			nodeFactories.Add((kind, validParentKinds ?? new string[0], factory));
		}

		/// <summary>Factories whose valid-parent list includes parentKind. Empty parentKind matches factories registered for the tree root.</summary>
		public IReadOnlyList<(string Kind, NodeFactory Factory)> FactoriesForParent(string parentKind)
		{
			List<(string Kind, NodeFactory Factory)> result = new List<(string, NodeFactory)>();
			foreach ((string kind, string[] parents, NodeFactory factory) in nodeFactories)
			{
				if (IsValidParent(parents, parentKind))
				{
					result.Add((kind, factory));
				}
			}
			return result;
		}

		private static bool IsValidParent(string[] validParents, string parentKind)
		{
			if (validParents == null || validParents.Length == 0)
			{
				return string.IsNullOrEmpty(parentKind);
			}

			if (string.IsNullOrEmpty(parentKind))
			{
				return false;
			}

			foreach (string parent in validParents)
			{
				if (parent == parentKind)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>Looks up the factory for a node kind, or null if none is registered.</summary>
		public bool TryGetNodeFactory(string kind, out NodeFactory factory, out string[] validParentKinds)
		{
			foreach ((string registeredKind, string[] parents, NodeFactory registered) in nodeFactories)
			{
				if (registeredKind == kind)
				{
					factory = registered;
					validParentKinds = parents;
					return true;
				}
			}
			factory = null;
			validParentKinds = null;
			return false;
		}

		/// <summary>Registers the primary inspector drawer for a node kind. Highest priority wins.</summary>
		public void RegisterInspectorDrawer(string kind, InspectorDrawer drawer, int priority = 0)
		{
			if (string.IsNullOrEmpty(kind) || drawer == null)
			{
				throw new ArgumentException("Inspector drawer needs a kind and a callback.");
			}
			inspectorDrawers.Add((kind, drawer, priority));
		}

		/// <summary>Registers an extra inspector section stacked under the primary drawer for a node kind.</summary>
		public void RegisterInspectorSection(string kind, InspectorDrawer section, int priority = 0)
		{
			if (string.IsNullOrEmpty(kind) || section == null)
			{
				throw new ArgumentException("Inspector section needs a kind and a callback.");
			}
			inspectorSections.Add((kind, section, priority));
		}

		/// <summary>Returns the highest-priority primary drawer for a kind, or null.</summary>
		public InspectorDrawer FindInspectorDrawer(string kind)
		{
			InspectorDrawer best = null;
			int bestPriority = int.MinValue;
			foreach ((string registeredKind, InspectorDrawer drawer, int priority) in inspectorDrawers)
			{
				if (registeredKind == kind && priority >= bestPriority)
				{
					best = drawer;
					bestPriority = priority;
				}
			}
			return best;
		}

		/// <summary>Returns every contributed inspector section for a kind, lowest priority first.</summary>
		public IReadOnlyList<InspectorDrawer> FindInspectorSections(string kind)
		{
			List<(InspectorDrawer Section, int Priority)> matched = new List<(InspectorDrawer, int)>();
			foreach ((string registeredKind, InspectorDrawer section, int priority) in inspectorSections)
			{
				if (registeredKind == kind)
				{
					matched.Add((section, priority));
				}
			}
			matched.Sort((a, b) => a.Priority.CompareTo(b.Priority));
			List<InspectorDrawer> result = new List<InspectorDrawer>(matched.Count);
			foreach ((InspectorDrawer section, int _) in matched)
			{
				result.Add(section);
			}
			return result;
		}

		/// <summary>Registers a bottom panel on this project type.</summary>
		public void RegisterBottomPanel(string name, string iconKey, Func<Transform, GameObject> builder, BottomPanelIsRelevant isRelevant = null, Action refresh = null, Action unbind = null)
		{
			if (string.IsNullOrEmpty(name) || builder == null)
			{
				throw new ArgumentException("Bottom panel needs a name and a builder.");
			}
			bottomPanels.RemoveAll(panel => panel.Name == name);
			bottomPanels.Add(new BottomPanelRegistration(name, iconKey, builder, isRelevant, refresh, unbind));
		}

		/// <summary>Registers a persistent inspector host that the shell must not rebuild on every selection change.</summary>
		public void RegisterPersistentInspector(PersistentInspectorRegistration registration)
		{
			if (registration == null || string.IsNullOrEmpty(registration.Id) || registration.Matches == null)
			{
				throw new ArgumentException("Persistent inspector needs an id and a Matches callback.");
			}

			persistentInspectors.RemoveAll(entry => entry.Id == registration.Id);
			persistentInspectors.Add(registration);
		}
	}

	/// <summary>One bottom-panel registration on a project type.</summary>
	public sealed class BottomPanelRegistration
	{
		/// <summary>Panel name shown on the tab.</summary>
		public string Name { get; }

		/// <summary>Optional icon key.</summary>
		public string IconKey { get; }

		/// <summary>Builds the panel's content under the given parent.</summary>
		public Func<Transform, GameObject> Builder { get; }

		/// <summary>Optional auto-focus predicate; never hides the panel.</summary>
		public BottomPanelIsRelevant IsRelevant { get; }

		/// <summary>Optional refresh when the shell rebuilds or the tab is focused.</summary>
		public Action Refresh { get; }

		/// <summary>Optional cleanup before the shell destroys this panel.</summary>
		public Action Unbind { get; }

		/// <summary>Creates a bottom-panel registration.</summary>
		internal BottomPanelRegistration(string name, string iconKey, Func<Transform, GameObject> builder, BottomPanelIsRelevant isRelevant, Action refresh, Action unbind)
		{
			Name = name;
			IconKey = iconKey;
			Builder = builder;
			IsRelevant = isRelevant;
			Refresh = refresh;
			Unbind = unbind;
		}
	}
}
