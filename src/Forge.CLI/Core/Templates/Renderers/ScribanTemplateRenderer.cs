
namespace Forge.CLI.Core.Templates.Renderers
{
	public sealed class ScribanTemplateRenderer : ITemplateRenderer
	{
		public string Render(
			TemplateDefinition template,
			TemplateModel model)
		{
			var scriban = Scriban.Template.Parse(template.Content);

			if (scriban.HasErrors)
				throw new InvalidOperationException(
					scriban.Messages.First().Message);

			return scriban.Render(model, member => member.Name);
		}

        public Task<string> RenderAsync(TemplateDefinition template, TemplateModel model)
        {
			return Task.FromResult(Render(template, model));
        }
    }

}
