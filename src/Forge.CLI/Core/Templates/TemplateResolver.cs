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
			var current = key;
			//TODO: Essa lógica está imprecisa.
			while (true)
			{
				if (_templates.TryGetValue(current, out var template))
					return template;

				var lastDot = current.LastIndexOf('.');
				if (lastDot < 0)
					break;

				current = current[..lastDot];
			}

			throw new InvalidOperationException(
				$"Template '{key}' not found.");
		}
	}
}
