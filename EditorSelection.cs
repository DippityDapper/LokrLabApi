using System.Collections.Generic;

namespace LokrLabApi
{
	/// <summary>Shell-level selection every panel reads from one place. Primary is always a member of All.</summary>
	public sealed class EditorSelection
	{
		/// <summary>The last-clicked (or only) selected node, or null if nothing is selected.</summary>
		public LabNode Primary { get; private set; }

		/// <summary>Every currently selected node, including Primary.</summary>
		public IReadOnlyList<LabNode> All { get; private set; } = new LabNode[0];

		/// <summary>Clears Primary and All.</summary>
		public void Clear()
		{
			Primary = null;
			All = new LabNode[0];
		}

		/// <summary>Replaces the selection. The first node is Primary; All is a copy of the list. Empty/null clears.</summary>
		public void Set(IReadOnlyList<LabNode> nodes)
		{
			if (nodes == null || nodes.Count == 0)
			{
				Clear();
				return;
			}

			List<LabNode> copy = new List<LabNode>(nodes.Count);
			foreach (LabNode node in nodes)
			{
				if (node != null)
				{
					copy.Add(node);
				}
			}

			if (copy.Count == 0)
			{
				Clear();
				return;
			}

			Primary = copy[0];
			All = copy;
		}

		/// <summary>Selects a single node, or clears when node is null.</summary>
		public void Set(LabNode node)
		{
			if (node == null)
			{
				Clear();
				return;
			}

			Primary = node;
			All = new LabNode[] { node };
		}
	}
}
