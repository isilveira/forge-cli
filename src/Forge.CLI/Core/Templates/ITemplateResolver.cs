namespace Forge.CLI.Core.Templates
{
	public interface ITemplateResolver
	{
		TemplateDefinition Resolve(string templateKey);
	}
}
