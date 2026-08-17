using BepInEx;
using BepInEx.Logging;

namespace LokrLabApi
{
	/// <summary>Plugin entry point for the LokrLab contracts library.</summary>
	/// <remarks>
	/// A passive shared-library plugin, same shape as SimpleUI: no Harmony patches and no
	/// rendering. LokrLab (shell), LokrCharacterLab, and LokrAbilityLab all depend on it as
	/// true peers so editor contracts do not live in the runtime content-loading plugin.
	/// </remarks>
	[BepInPlugin(Guid, Name, Version)]
	public class LokrLabApiPlugin : BaseUnityPlugin
	{
		/// <summary>This plugin's BepInEx GUID.</summary>
		public const string Guid = "com.lokrmodding.labapi";
		/// <summary>This plugin's display name.</summary>
		public const string Name = "LoKR Lab API";
		/// <summary>This plugin's version string.</summary>
		public const string Version = "1.5.3";

		/// <summary>This plugin's shared BepInEx log source, set once in Awake().</summary>
		internal static ManualLogSource Log;

		/// <summary>Logs that the plugin loaded.</summary>
		private void Awake()
		{
			Log = base.Logger;
			Log.LogInfo(Name + " v" + Version + " loaded.");
		}
	}
}
