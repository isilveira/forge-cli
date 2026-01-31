using Forge.CLI.Models;
using System.Text.Json;

namespace Forge.CLI.Persistence
{
	public sealed class ProjectSaver
	{
		private const string ForgeFolder = ".forge";
		private const string ProjectFile = "project.json";

		public async Task SaveAsync(ForgeProject project)
		{
			project.Sharpen();

			var forgeDir = Path.Combine(
				Directory.GetCurrentDirectory(),
				ForgeFolder);

			if (!Directory.Exists(forgeDir))
				Directory.CreateDirectory(forgeDir);

			var path = Path.Combine(forgeDir, ProjectFile);

			var json = JsonSerializer.Serialize(
				project,
				new JsonSerializerOptions
				{
					WriteIndented = true
				});

			await File.WriteAllTextAsync(path, json);
		}
	}
}
