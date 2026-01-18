namespace Forge.CLI.Core.Templates
{
	public sealed class TemplateResolver : ITemplateResolver
	{
		private readonly IDictionary<string, TemplateDefinition> _templates;

		public TemplateResolver(
			IDictionary<string, TemplateDefinition> templates)
		{
			_templates = templates;
		}

		public TemplateDefinition Resolve(string key)
		{
			if (_templates.TryGetValue(key, out var template))
				return template;

			throw new InvalidOperationException(
				$"Template '{key}' not found.");
		}
	}
}
