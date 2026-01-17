using Forge.CLI.Models;

namespace Forge.CLI.Core.Target
{
	public sealed class ScaffoldTarget
	{
		public ForgeProject Project { get; init; }
		public TargetScope Scope { get; init; }
		public string? ContextName { get; init; }
		public string? EntityName { get; init; }
		public string? Name { get; init; }
	}

}
