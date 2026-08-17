using System;
using System.Collections.Generic;
using UnityEngine;

namespace LokrLabApi
{
	/// <summary>A persistent inspector form that the shell must not rebuild on every selection identity change.</summary>
	/// <remarks>
	/// Used when widgets must survive selection changes (Properties categories, live Animator
	/// InspectorPanel). The shell gives each registration its own Grow() scroll host. Inner
	/// content must not nest another ScrollRect.
	/// </remarks>
	public sealed class PersistentInspectorRegistration
	{
		/// <summary>Stable id for this host (e.g. "properties", "animator-live").</summary>
		public string Id { get; set; }

		/// <summary>True when this host should show for the current selection.</summary>
		public Func<IReadOnlyList<LabNode>, bool> Matches { get; set; }

		/// <summary>Builds widgets into the host once. Safe to call repeatedly.</summary>
		public Action<Transform> EnsureBuilt { get; set; }

		/// <summary>Shows the matching form for the current selection.</summary>
		public Action<IReadOnlyList<LabNode>> Show { get; set; }

		/// <summary>Hides this host without destroying widgets.</summary>
		public Action Hide { get; set; }

		/// <summary>Optional same-identity tick (e.g. live Animator playback refresh).</summary>
		public Action Refresh { get; set; }

		/// <summary>When false, the shell host is a non-scroll Grow() column so an inner widget can own the only ScrollRect.</summary>
		/// <remarks>
		/// Default true matches existing Properties / Animator hosts. Nested ScrollRects collapse
		/// to zero height — set this false when the form's list (for example a catalogue) scrolls.
		/// </remarks>
		public bool Scrollable { get; set; } = true;
	}
}
