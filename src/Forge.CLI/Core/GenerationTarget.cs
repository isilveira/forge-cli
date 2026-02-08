namespace Forge.CLI.Core
{
	public sealed class GenerationTarget
	{
		public string? NamespacePattern { get; init; } = default!;
		public string Path { get; init; } = default!;
		public string Filename { get; init; } = default!;
	}
}
