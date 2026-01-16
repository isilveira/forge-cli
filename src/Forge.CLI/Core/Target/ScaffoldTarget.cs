namespace Forge.CLI.Core.Target
{
	public sealed class ScaffoldTarget
	{
		public bool AllContexts { get; init; }
		public bool AllEntities { get; init; }
		public TargetScope Scope { get; init; }
		public string? ContextName { get; init; }
		public string? EntityName { get; init; }
	}

}
