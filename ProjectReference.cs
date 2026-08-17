namespace LokrLabApi
{
	/// <summary>A read-only cross-project pointer (id + type), resolved lazily by the picker or a jump action.</summary>
	public sealed class ProjectReference
	{
		/// <summary>The referenced project's type id.</summary>
		public string ProjectTypeId { get; }

		/// <summary>The referenced project's id (folder name).</summary>
		public string ProjectId { get; }

		/// <summary>Absolute folder path, when known at pick time.</summary>
		public string FolderPath { get; }

		/// <summary>Display name at pick time; may go stale if the target is renamed later.</summary>
		public string DisplayName { get; }

		/// <summary>Creates a reference to a project of the given type.</summary>
		public ProjectReference(string projectTypeId, string projectId, string folderPath, string displayName)
		{
			ProjectTypeId = projectTypeId;
			ProjectId = projectId;
			FolderPath = folderPath;
			DisplayName = displayName;
		}
	}
}
