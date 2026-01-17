using Forge.CLI.Core.Artifacts;
using System.Runtime.CompilerServices;

namespace Forge.CLI.Core.Templates
{
	public sealed class TemplateLoader
	{
		public IDictionary<string, TemplateDefinition> Load()
		{
			var templates = new Dictionary<string, TemplateDefinition>();

			foreach (var file in Directory.GetFiles(
				"Scaffolding\\Templates", "*.cshtml", SearchOption.AllDirectories))
			{
				var key = file
					.Replace("Scaffolding\\Templates\\", "")
					.Replace(".cshtml", "")
					.Replace("\\", ".");

				templates[key] = new TemplateDefinition
				{
					Key = key,
					Content = File.ReadAllText(file)
				};
			}

			return templates;
		}

		public IDictionary<string, TemplateDefinition> Load(ArtifactDescriptor descriptor)
		{
			var templates = new Dictionary<string, TemplateDefinition>();

			var filePath = Path.Combine(
				".forge",
				"Scaffolding",
				"Templates",
				descriptor.TemplateKey.Replace('.', Path.DirectorySeparatorChar) + ".cshtml"
				);

			if (File.Exists(filePath))
			{
				var content = File.ReadAllText(filePath);
				templates[descriptor.TemplateKey] = new TemplateDefinition
				{
					Key = descriptor.TemplateKey,
					HasContent = true,
					Content = content
				};
			}
			else {
				templates[descriptor.TemplateKey] = new TemplateDefinition
				{
					Key = descriptor.TemplateKey,
				};
			}

			return templates;
		}
	}
}
