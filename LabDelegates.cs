using System.Collections.Generic;
using UnityEngine;

namespace LokrLabApi
{
	/// <summary>Returns the top-level node(s) one contributor owns for the current session.</summary>
	public delegate IEnumerable<LabNode> NodeTreeContributor(ProjectSession session);

	/// <summary>Creates a new child node under parent in the given session.</summary>
	public delegate LabNode NodeFactory(LabNode parent, ProjectSession session);

	/// <summary>Builds inspector UI for a selected node into contentParent.</summary>
	/// <remarks>contentParent is a Transform rather than a SimpleUI type so this contracts plugin does not depend on SimpleUI.</remarks>
	public delegate void InspectorDrawer(LabNode node, ProjectSession session, Transform contentParent);

	/// <summary>Optional auto-focus filter for a bottom panel. Never hides the panel, only decides whether to focus it.</summary>
	public delegate bool BottomPanelIsRelevant(WorkspaceRegistration activeWorkspace, EditorSelection selection);
}
