using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LokrLabApi
{
	/// <summary>One lab-scene build, passed to <see cref="LokrLabApi.LabOpened"/> so project types can parent popups and legacy screens.</summary>
	public sealed class LabSceneContext
	{
		/// <summary>The in-memory lab scene.</summary>
		public Scene Scene { get; set; }

		/// <summary>Root canvas.</summary>
		public Transform Canvas { get; set; }

		/// <summary>Built-in Arial used by lab chrome.</summary>
		public Font DefaultFont { get; set; }

		/// <summary>Backdrop camera.</summary>
		public Camera BackdropCamera { get; set; }

		/// <summary>Named screen root (Browser, Shell, Home, Load, or a workstation id).</summary>
		public Func<string, Transform> GetScreenRoot { get; set; }
	}
}
