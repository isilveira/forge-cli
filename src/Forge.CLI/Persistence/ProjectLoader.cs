namespace Forge.CLI.Persistence
{
    using Forge.CLI.Models;
    using System.Text.Json;

	public sealed class ProjectLoader
	{
		private const string ForgeFolder = ".forge";
		private const string ProjectFile = "project.json";

		public ForgeProject? TryLoad()
		{
			var path = GetProjectFilePath();

			if (!File.Exists(path))
				return null;

			var json = File.ReadAllText(path);

			var project = JsonSerializer.Deserialize<ForgeProject>(json);

			if(project is not null)
				project.Sharpen();

			return project;
		}

		public void EnsureNotInitialized()
		{
			if (File.Exists(GetProjectFilePath()))
				throw new InvalidOperationException(
					"Forge já foi inicializado neste projeto.");
		}

		private static string GetProjectFilePath()
		{
			return Path.Combine(
				Directory.GetCurrentDirectory(),
				ForgeFolder,
				ProjectFile);
		}
	}

}
