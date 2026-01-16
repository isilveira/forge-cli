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
	}
}
