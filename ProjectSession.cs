namespace LokrLabApi
{
	/// <summary>The currently open project, whichever type it is. CharacterSession is the first concrete subclass.</summary>
	public abstract class ProjectSession
	{
		/// <summary>The project type id that created this session (e.g. "character").</summary>
		public string ProjectTypeId { get; protected set; }

		/// <summary>Stable project id, usually the folder name.</summary>
		public string Id { get; protected set; }

		/// <summary>Absolute path of the project folder on disk.</summary>
		public string FolderPath { get; protected set; }

		/// <summary>Player-facing name shown in the shell chrome.</summary>
		public string DisplayName { get; set; }

		/// <summary>True when the session has unsaved edits.</summary>
		public bool IsDirty { get; set; }
	}
}
