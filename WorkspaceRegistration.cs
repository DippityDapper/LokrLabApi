using System;
using UnityEngine;

namespace LokrLabApi
{
	/// <summary>One top-tab workspace registered on a project type (replaces today's workstation screens over Phases 5–8).</summary>
	public sealed class WorkspaceRegistration
	{
		/// <summary>Stable workspace id (e.g. "animator", "properties").</summary>
		public string Name { get; set; }

		/// <summary>Optional icon key for the workspace tab.</summary>
		public string IconKey { get; set; }

		/// <summary>Lower values appear first in the workspace tab strip.</summary>
		public int Priority { get; set; }

		/// <summary>When true, the workspace is unavailable until a project is open.</summary>
		public bool RequiresProjectOpen { get; set; } = true;

		/// <summary>Builds this workspace's toolbar into the given parent. Optional.</summary>
		public Func<Transform, GameObject> BuildToolbar { get; set; }

		/// <summary>Builds this workspace's center-viewport content. Optional.</summary>
		public Func<Transform, GameObject> BuildViewport { get; set; }

		/// <summary>When this returns true, selecting the workspace drives a real scene transition instead of in-shell content. Unused by Sandbox setup (that tab is in-shell); the fight itself still uses <c>CloseTo</c>.</summary>
		public Func<ProjectSession, bool> RequiresSceneTransition { get; set; }

		/// <summary>Optional enter callback when <see cref="RequiresSceneTransition"/> is true.</summary>
		public Action<ProjectSession> EnterViaSceneTransition { get; set; }

		/// <summary>Fired after this workspace's viewport is built.</summary>
		public Action OnActivated { get; set; }

		/// <summary>Fired when leaving this workspace (tab switch or project-type change).</summary>
		public Action OnDeactivated { get; set; }
	}
}
