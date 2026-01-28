namespace Forge.CLI.Core.Scaffolding.Execution
{
	public sealed class ScaffoldExecutionResult
	{
		public int Created { get; init; }
		public int Updated { get; init; }
		public int Skipped { get; init; }
		public int Conflicts { get; init; }
	}
}

