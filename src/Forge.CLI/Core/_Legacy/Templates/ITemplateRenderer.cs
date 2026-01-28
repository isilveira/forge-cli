namespace Forge.CLI.Core._Legacy.Templates
{
	public interface ITemplateRenderer
	{
		string Render(
			TemplateDefinition template,
			TemplateModel model);
		Task<string> RenderAsync(
			TemplateDefinition template,
			TemplateModel model);
	}

}
