namespace Forge.CLI.Core
{
	public sealed class TemplateDefinition
	{
		public string Engine { get; init; } = "razor";
		public string File { get; init; } = default!;
	}

}
