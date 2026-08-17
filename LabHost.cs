using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LokrLabApi
{
	/// <summary>Shell-assigned live lab context so project-type plugins never reference LokrLab.dll.</summary>
	/// <remarks>
	/// LokrLab fills this when the lab scene is built and clears it on close. Ability Lab and
	/// Character Lab call through <see cref="LokrLabApi.Host"/> the same way they already call
	/// <see cref="LokrLabApi.JumpToProject"/>.
	/// </remarks>
	public sealed class LabHost
	{
		/// <summary>Built-in Arial used by lab chrome.</summary>
		public Font DefaultFont { get; set; }

		/// <summary>The in-memory lab scene.</summary>
		public Scene LabScene { get; set; }

		/// <summary>Full-screen backdrop camera; viewport cameras render above it.</summary>
		public Camera BackdropCamera { get; set; }

		/// <summary>Root canvas transform.</summary>
		public Transform Canvas { get; set; }

		/// <summary>Real scene the lab was opened from.</summary>
		public string OriginScene { get; set; }

		/// <summary>True while the lab scene is showing.</summary>
		public Func<bool> IsOpen { get; set; }

		/// <summary>Active workspace name, or empty.</summary>
		public Func<string> GetActiveWorkspaceName { get; set; }

		/// <summary>Switches the in-shell workspace tab.</summary>
		public Action<string> ActivateWorkspace { get; set; }

		/// <summary>Focuses a bottom-dock tab by display name. Returns false if that tab is absent.</summary>
		public Func<string, bool> FocusBottomPanel { get; set; }

		/// <summary>Focuses a dock panel by id (e.g. file-tree).</summary>
		public Action<string> FocusPanel { get; set; }

		/// <summary>Sets the status-bar left text.</summary>
		public Action<string> SetStatus { get; set; }

		/// <summary>Selects a Node Tree row by id. Returns false if missing.</summary>
		public Func<string, bool> SelectNodeById { get; set; }

		/// <summary>Closes the lab into a real Build-Settings scene.</summary>
		public Action<string> CloseTo { get; set; }

		/// <summary>Rebuilds the lab after a sandbox fight, landing on the given workspace when a project is open.</summary>
		public Action<string, string> ReopenAfterFight { get; set; }

		/// <summary>Closes the open project and returns to the Project Browser.</summary>
		public Action CloseProject { get; set; }

		/// <summary>Closes the lab and returns to the origin scene.</summary>
		public Action CloseLab { get; set; }

		/// <summary>Shows the dockable shell (Home is retired).</summary>
		public Action SwitchToHome { get; set; }

		/// <summary>Shows the Project Browser.</summary>
		public Action SwitchToLoad { get; set; }

		/// <summary>Shows the dockable shell for the current project.</summary>
		public Action SwitchToShell { get; set; }

		/// <summary>Shows a named screen root (Home, Load, or a legacy workstation id).</summary>
		public Action<string> ShowScreen { get; set; }

		/// <summary>Screen root for a named lab screen, creating it if needed.</summary>
		public Func<string, Transform> GetScreenRoot { get; set; }

		/// <summary>Legacy workstation content frame inside a screen root.</summary>
		public Func<Transform, Transform> GetWorkstationContentRoot { get; set; }

		/// <summary>Shows the Help → About modal.</summary>
		public Action ShowAbout { get; set; }

		/// <summary>Forces the inspector to rebuild on the next refresh.</summary>
		public Action InvalidateInspector { get; set; }

		/// <summary>Starts an additive scene and crops its camera to <see cref="EmbeddedSceneRequest.Hole"/>. Returns an immediate error, or null if the load started.</summary>
		public Func<EmbeddedSceneRequest, string> StartEmbeddedScene { get; set; }

		/// <summary>Unloads the embedded scene. No-op if none is running.</summary>
		public Action StopEmbeddedScene { get; set; }

		/// <summary>True while an additive scene is loaded beside the lab.</summary>
		public Func<bool> IsEmbeddedSceneActive { get; set; }

		/// <summary>Gameplay camera of the current embed, or null.</summary>
		public Func<Camera> GetEmbeddedSceneCamera { get; set; }

		/// <summary>Starts a real fight scene additively and binds its camera into a Stage hole. Returns an immediate error, or null if the load started.</summary>
		public Func<EmbeddedFightRequest, string> StartEmbeddedFight { get; set; }

		/// <summary>Unloads an embedded fight and restores Stage singletons. No-op if none is running.</summary>
		public Action StopEmbeddedFight { get; set; }

		/// <summary>True while an embedded fight scene is loaded beside the lab.</summary>
		public Func<bool> IsEmbeddedFightActive { get; set; }
	}
}
