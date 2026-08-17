using System;
using System.Collections.Generic;
using UnityEngine;

namespace LokrLabApi
{
	/// <summary>Project-type-agnostic editor contracts: register types, read the current session/selection, contribute shell menus.</summary>
	/// <remarks>
	/// Lives in its own plugin so LokrLab (the shell) and third-party editors
	/// can all depend on it as peers. No rendering, no Harmony patches.
	/// </remarks>
	public static class LokrLabApi
	{
		private static readonly Dictionary<string, ProjectTypeRegistration> projectTypes = new Dictionary<string, ProjectTypeRegistration>();
		private static readonly List<MenuRegistration> menus = new List<MenuRegistration>();
		private static readonly EditorSelection selection = new EditorSelection();

		/// <summary>Stable id for the Character project type.</summary>
		public const string CharacterTypeId = "character";

		/// <summary>Stable id for the Ability Library project type.</summary>
		public const string AbilityLibraryTypeId = "ability-library";

		/// <summary>Stable id for the Encounter project type.</summary>
		public const string EncounterTypeId = "encounter";

		/// <summary>The currently open project, or null when the Project Browser is showing.</summary>
		public static ProjectSession CurrentSession { get; set; }

		/// <summary>Shell-level selection. The Node Tree writes this; Inspector / other panels read it.</summary>
		public static EditorSelection Selection => selection;

		/// <summary>Optional picker implementation assigned by the shell. Null until LokrLab boots.</summary>
		public static Func<string, ProjectReference> ProjectReferencePicker { get; set; }

		/// <summary>Shell-assigned: open a project (folder null = type FolderRoot) and optionally select a node. Opens the lab if needed.</summary>
		public static Action<string, string, string> OpenProject { get; set; }

		/// <summary>Shell-assigned: return to the project that was open before the last JumpToProject.</summary>
		public static Action ReturnToPreviousProject { get; set; }

		/// <summary>Shell-assigned: true while a cross-project jump can be undone.</summary>
		public static Func<bool> CanReturnToPreviousProject { get; set; }

		/// <summary>Shell-assigned: rebuild Node Tree / File Tree / inspector after an external edit.</summary>
		public static Action RefreshShell { get; set; }

		/// <summary>Shell-assigned live lab context. Null while the lab is closed.</summary>
		public static LabHost Host { get; set; }

		/// <summary>LokrLab assigns this when the shell boots. Project types call it even if Host was rebuilt.</summary>
		public static Func<EmbeddedSceneRequest, string> StartEmbeddedScene { get; set; }

		/// <summary>Unloads an embedded scene. Assigned with <see cref="StartEmbeddedScene"/>.</summary>
		public static Action StopEmbeddedScene { get; set; }

		/// <summary>True while an additive scene is loaded beside the lab.</summary>
		public static Func<bool> IsEmbeddedSceneActive { get; set; }

		/// <summary>Gameplay camera of the current embed, or null.</summary>
		public static Func<Camera> GetEmbeddedSceneCamera { get; set; }

		/// <summary>Character Lab assigns this at plugin load. Ability Lab Play uses it even if Host was rebuilt.</summary>
		public static Func<EmbeddedFightRequest, string> StartEmbeddedFight { get; set; }

		/// <summary>Unloads an embedded fight. Assigned with <see cref="StartEmbeddedFight"/>.</summary>
		public static Action StopEmbeddedFight { get; set; }

		/// <summary>True while an embedded fight scene is loaded beside the lab.</summary>
		public static Func<bool> IsEmbeddedFightActive { get; set; }

		/// <summary>Optional Character (or other type) legacy-mod import. Character assigns this at plugin load.</summary>
		public static Action<string> ImportLegacyFolder { get; set; }

		/// <summary>File → Import Legacy Pack. Character assigns a Mods/ folder picker at plugin load.</summary>
		public static Action PromptLegacyImport { get; set; }

		/// <summary>Optional recent project folders for Project Browser ordering. Null skips recents.</summary>
		public static Func<IReadOnlyList<string>> RecentProjectFolders { get; set; }

		/// <summary>Raised once after the lab scene chrome is built. Project types parent popups here.</summary>
		public static event Action<LabSceneContext> LabOpened;

		/// <summary>Raised before the lab scene is torn down. Project types drop widget refs here.</summary>
		public static event Action LabClosing;

		/// <summary>Raised when the dockable shell is shown.</summary>
		public static event Action ShellShown;

		/// <summary>Raised after a named lab screen is shown (Browser, Shell, Home, or a workstation id).</summary>
		public static event Action<string> ScreenShown;

		/// <summary>Every registered project type, in registration order.</summary>
		public static IReadOnlyCollection<ProjectTypeRegistration> ProjectTypes => projectTypes.Values;

		/// <summary>Registered top-level menus, lowest priority first.</summary>
		public static IReadOnlyList<MenuRegistration> Menus
		{
			get
			{
				menus.Sort((a, b) => a.Priority.CompareTo(b.Priority));
				return menus;
			}
		}

		/// <summary>Registers a project type, replacing any existing one with the same id.</summary>
		public static ProjectTypeRegistration RegisterProjectType(string id, string displayName, string iconKey, string folderRoot)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("Project type id must be non-empty.", nameof(id));
			}
			ProjectTypeRegistration registration = new ProjectTypeRegistration(id, displayName, iconKey, folderRoot);
			projectTypes[id] = registration;
			return registration;
		}

		/// <summary>Looks up a registered project type by id, or null if none matches.</summary>
		public static ProjectTypeRegistration GetProjectType(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			projectTypes.TryGetValue(id, out ProjectTypeRegistration registration);
			return registration;
		}

		/// <summary>Opens the shared cross-project picker for the given type. Returns null if cancelled or if no picker is installed.</summary>
		public static ProjectReference PickProjectReference(string projectTypeId)
		{
			return ProjectReferencePicker != null ? ProjectReferencePicker(projectTypeId) : null;
		}

		/// <summary>Switches to a project of the given type. folder null uses the type's FolderRoot (singletons). selectNodeId is selected after the switch when present.</summary>
		public static void JumpToProject(string projectTypeId, string folder = null, string selectNodeId = null)
		{
			OpenProject?.Invoke(projectTypeId, folder, selectNodeId);
		}

		/// <summary>Asks the shell to rebuild trees and inspector. No-op until LokrLab assigns RefreshShell.</summary>
		public static void RequestRefresh()
		{
			RefreshShell?.Invoke();
		}

		/// <summary>Invokes <see cref="LabOpened"/> for the shell.</summary>
		public static void RaiseLabOpened(LabSceneContext context)
		{
			LabOpened?.Invoke(context);
		}

		/// <summary>Invokes <see cref="LabClosing"/> for the shell.</summary>
		public static void RaiseLabClosing()
		{
			LabClosing?.Invoke();
		}

		/// <summary>Invokes <see cref="ShellShown"/> for the shell.</summary>
		public static void RaiseShellShown()
		{
			ShellShown?.Invoke();
		}

		/// <summary>Invokes <see cref="ScreenShown"/> for the shell.</summary>
		public static void RaiseScreenShown(string name)
		{
			ScreenShown?.Invoke(name);
		}

		/// <summary>Registers a top-level menu on the shell menu bar. Existing menus of the same name are left in place (items stay).</summary>
		public static void RegisterMenu(string name, int priority = 0)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Menu name must be non-empty.", nameof(name));
			}
			if (menus.Exists(menu => menu.Name == name))
			{
				return;
			}
			menus.Add(new MenuRegistration(name, priority));
		}

		/// <summary>Registers an item into a menu, creating the menu if needed. <paramref name="isEnabled"/> greys the item; <paramref name="isVisible"/> omits it from the dropdown.</summary>
		public static void RegisterMenuItem(string menuName, string label, Action onClick, int priority = 0, Func<bool> isEnabled = null, Func<bool> isVisible = null)
		{
			if (string.IsNullOrEmpty(menuName) || string.IsNullOrEmpty(label))
			{
				throw new ArgumentException("Menu item needs a menu name and a label.");
			}
			MenuRegistration menu = menus.Find(entry => entry.Name == menuName);
			if (menu == null)
			{
				RegisterMenu(menuName);
				menu = menus.Find(entry => entry.Name == menuName);
			}
			menu.AddItem(new MenuItemRegistration(label, onClick, priority, isEnabled, isVisible));
		}
	}
}
