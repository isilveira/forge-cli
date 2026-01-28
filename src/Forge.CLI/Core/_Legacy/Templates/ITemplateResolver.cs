namespace Forge.CLI.Core._Legacy.Templates
{
	public interface ITemplateResolver
	{
		TemplateDefinition Resolve(string templateKey);
	}
}
