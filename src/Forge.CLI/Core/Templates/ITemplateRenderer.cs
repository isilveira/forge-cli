namespace Forge.CLI.Core.Templates
{
	public interface ITemplateRenderer
	{
		Task<string> RenderAsync(string templateKey, object model);
	}
}

