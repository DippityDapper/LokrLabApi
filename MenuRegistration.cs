using System;
using System.Collections.Generic;

namespace LokrLabApi
{
	/// <summary>One top-level menu (File / Edit / View / Help) on the shell menu bar.</summary>
	public sealed class MenuRegistration
	{
		/// <summary>Menu name shown on the bar.</summary>
		public string Name { get; }

		/// <summary>Lower values appear first.</summary>
		public int Priority { get; }

		/// <summary>Items registered into this menu, in priority order.</summary>
		public IReadOnlyList<MenuItemRegistration> Items => items;

		private readonly List<MenuItemRegistration> items = new List<MenuItemRegistration>();

		/// <summary>Creates a menu registration.</summary>
		internal MenuRegistration(string name, int priority)
		{
			Name = name;
			Priority = priority;
		}

		/// <summary>Adds or replaces an item with the same label.</summary>
		internal void AddItem(MenuItemRegistration item)
		{
			items.RemoveAll(existing => existing.Label == item.Label);
			items.Add(item);
			items.Sort((a, b) => a.Priority.CompareTo(b.Priority));
		}
	}

	/// <summary>One item inside a top-level menu.</summary>
	public sealed class MenuItemRegistration
	{
		/// <summary>Item label shown in the dropdown.</summary>
		public string Label { get; }

		/// <summary>Invoked when the item is clicked.</summary>
		public Action OnClick { get; }

		/// <summary>Lower values appear first.</summary>
		public int Priority { get; }

		/// <summary>Optional enable predicate; null means always enabled.</summary>
		public Func<bool> IsEnabled { get; }

		/// <summary>Optional visibility predicate; null means always shown. Hidden items are omitted from the dropdown.</summary>
		public Func<bool> IsVisible { get; }

		/// <summary>Creates a menu item registration.</summary>
		internal MenuItemRegistration(string label, Action onClick, int priority, Func<bool> isEnabled, Func<bool> isVisible)
		{
			Label = label;
			OnClick = onClick;
			Priority = priority;
			IsEnabled = isEnabled;
			IsVisible = isVisible;
		}
	}
}
