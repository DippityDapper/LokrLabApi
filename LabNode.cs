using System.Collections.Generic;

namespace LokrLabApi
{
	/// <summary>One node in the shell's Node Tree. Kind is an extensible string, not a closed enum.</summary>
	/// <remarks>
	/// Presentation-layer only: contributors build a fresh tree from on-disk files each
	/// refresh. Nothing is itself persisted as a node (editor-redesign §5.2.1 option A).
	/// </remarks>
	public sealed class LabNode
	{
		/// <summary>Stable id within a session.</summary>
		public string Id { get; set; }

		/// <summary>Text shown in the Node Tree.</summary>
		public string DisplayName { get; set; }

		/// <summary>Extensible kind key (e.g. "Part", "AnimationClip", "AbilityRef").</summary>
		public string Kind { get; set; }

		/// <summary>Optional icon key for the tree row.</summary>
		public string IconKey { get; set; }

		/// <summary>Child nodes. Never null after construction.</summary>
		public List<LabNode> Children { get; set; } = new List<LabNode>();

		/// <summary>The real object this node represents (a part, a clip, a project reference, …).</summary>
		public object Payload { get; set; }
	}
}
