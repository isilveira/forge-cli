using Forge.CLI.Core.Scaffolding.Planning;

namespace Forge.CLI.Core.Scaffolding.Execution
{
	public interface IScaffoldExecutor
	{
		Task<ScaffoldExecutionResult> ExecuteAsync(
			ScaffoldPlan plan,
			ScaffoldExecutionOptions options,
			CancellationToken cancellationToken = default);
	}
}
