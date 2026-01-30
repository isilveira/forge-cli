using Forge.CLI.Core.Scaffolding.Planning;
using Forge.CLI.Shared.Helpers;

namespace Forge.CLI.Core.Scaffolding.Execution
{
	public sealed class DryRunExecutor : IScaffoldExecutor
	{
		public Task<ScaffoldExecutionResult> ExecuteAsync(
			ScaffoldPlan plan,
			ScaffoldExecutionOptions options,
			CancellationToken cancellationToken = default)
		{
			if (plan is null) throw new ArgumentNullException(nameof(plan));
			if (options is null) throw new ArgumentNullException(nameof(options));

			var created = 0;
			var updated = 0;
			var skipped = 0;
			var conflicts = 0;

			foreach (var action in plan.Actions)
			{
				cancellationToken.ThrowIfCancellationRequested();

				switch (action.ActionType)
				{
					case Planning.ScaffoldActionType.Create:
						created++;
						AnsiConsoleHelper.SafeMarkupLine($"[WHAT-IF] CREATE → {action.FilePath}", "grey");
						break;
					case Planning.ScaffoldActionType.Update:
						updated++;
						AnsiConsoleHelper.SafeMarkupLine($"[WHAT-IF] UPDATE → {action.FilePath}", "grey");
						break;
					case Planning.ScaffoldActionType.Skip:
						skipped++;
						AnsiConsoleHelper.SafeMarkupLine($"[WHAT-IF] SKIP   → {action.FilePath}{(string.IsNullOrWhiteSpace(action.Reason) ? "" : $" ({action.Reason})")}", "yellow");
						break;
					case Planning.ScaffoldActionType.Conflict:
						conflicts++;
						AnsiConsoleHelper.SafeMarkupLine($"[WHAT-IF] CONFLICT → {action.FilePath}{(string.IsNullOrWhiteSpace(action.Reason) ? "" : $" ({action.Reason})")}", "red");
						break;
					default:
						AnsiConsoleHelper.SafeMarkupLine($"[WHAT-IF] UNKNOWN → {action.FilePath}", "yellow");
						break;
				}
			}

			return Task.FromResult(new ScaffoldExecutionResult
			{
				Created = created,
				Updated = updated,
				Skipped = skipped,
				Conflicts = conflicts
			});
		}
	}
}
