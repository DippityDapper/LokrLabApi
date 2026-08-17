using System;
using UnityEngine;

namespace LokrLabApi
{
	/// <summary>Arguments for <see cref="LabHost.StartEmbeddedScene"/> — an additive scene cropped to a hole.</summary>
	/// <remarks>
	/// <see cref="Hole"/> must have a real height (<c>Grow()</c>, <c>minHeight</c>, no
	/// <c>ContentSizeFitter</c>). A zero-size hole leaves the gameplay camera fullscreen.
	/// </remarks>
	public sealed class EmbeddedSceneRequest
	{
		/// <summary>Asset-bundle id passed to <c>AssetBundleManager.LoadScene</c> (fight uses <c>scenes</c>).</summary>
		public string BundleId;

		/// <summary>Scene name inside that bundle (fight uses <c>SceneDB.GetScene("fight")</c>).</summary>
		public string SceneName;

		/// <summary>UI hole whose world corners become the gameplay camera <c>rect</c>.</summary>
		public RectTransform Hole;

		/// <summary>When true, remaps Overlay HUD canvases onto the gameplay camera and scales them to the hole.</summary>
		public bool FitHud = true;

		/// <summary>When true, disables extra EventSystems, cameras, and AudioListeners in the loaded scene.</summary>
		public bool DisableExtraCameras = true;

		/// <summary>Called with the gameplay camera once it is found (may fire again after a late bind).</summary>
		public Action<Camera> OnCamera;

		/// <summary>Called after the scene is loaded and the camera is bound to the hole.</summary>
		public Action OnReady;

		/// <summary>Called if the additive load fails after Start returned success.</summary>
		public Action<string> OnFailed;

		/// <summary>Called after Stop unloads the scene (caller-initiated or host teardown).</summary>
		public Action OnEnded;
	}
}
