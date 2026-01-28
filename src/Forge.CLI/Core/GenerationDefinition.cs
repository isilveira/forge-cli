namespace Forge.CLI.Core
{
	public sealed class GenerationDefinition
	{
		public bool Enabled { get; init; }

		public GenerationTarget Target { get; init; } = default!;
		public TemplateDefinition Template { get; init; } = default!;
	}
}
