namespace Forge.CLI.Core.Templates
{
	public interface ITemplateRenderer
	{
		string Render(
			TemplateDefinition template,
			TemplateModel model);
	}

}
