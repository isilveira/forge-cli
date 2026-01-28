namespace Forge.CLI.Core._Legacy.Templates
{
	public sealed class TemplateDefinition
	{
		public string Key { get; init; } = null!;
		public bool HasContent { get; init; } = false;
		public string Content { get; init; } = null!;
	}
}
