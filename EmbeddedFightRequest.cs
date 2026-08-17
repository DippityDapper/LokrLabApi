using System;
using UnityEngine;

namespace LokrLabApi
{
	/// <summary>Arguments for <see cref="LabHost.StartEmbeddedFight"/> — a real fight scene shown in a Stage hole.</summary>
	public sealed class EmbeddedFightRequest
	{
		/// <summary>Unit definition id to spawn as the friendly caster.</summary>
		public string CasterUnitId;

		/// <summary>1-based hero rank to spawn. 0 or 1 is the base definition; higher ranks walk <c>nextLevelArchetype</c>.</summary>
		public int CasterLevel;

		/// <summary>Optional enemy id. Character Lab defaults to BanditRaider when empty.</summary>
		public string EnemyUnitId;

		/// <summary>Stage hole whose world corners crop the fight camera. Required for a live embed.</summary>
		public RectTransform Hole;

		/// <summary>Called with the fight gameplay camera once the scene is ready.</summary>
		public Action<Camera> BindCamera;

		/// <summary>Called after units spawn and the camera is bound.</summary>
		public Action OnReady;

		/// <summary>Called if the additive load or spawn fails after Start returned success.</summary>
		public Action<string> OnFailed;

		/// <summary>Called when the fight ends on its own (win/lose). Stop already ran.</summary>
		public Action OnEnded;
	}
}
