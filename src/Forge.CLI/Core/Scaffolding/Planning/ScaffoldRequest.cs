using Forge.CLI.Core.Scaffolding.Conflict;

namespace Forge.CLI.Core.Scaffolding.Planning
{
	public sealed class ScaffoldRequest
	{
		public string? Layer { get; init; }
		public string? Type { get; init; }
		public string? Variant { get; init; }

		public string? ContextName { get; init; }
		public string? EntityName { get; init; }

		/// <summary>
		/// Used by templates that generate "New" variations (e.g. Patch{NAME}{Entity}...).
		/// </summary>
		public string Name { get; init; } = "New";

		public OverwriteStrategy OverwriteStrategy { get; init; } = OverwriteStrategy.MarkersOnly;
	}
}

