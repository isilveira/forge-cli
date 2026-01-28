using Forge.CLI.Shared.Helpers;
using System.Text;

namespace Forge.CLI.Core.Scaffolding.Execution
{
	public sealed class FileSystemExecutor : IScaffoldExecutor
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

				AnsiConsoleHelper.SafeMarkupLine($"→ {action.FilePath}", "grey");

				switch (action.ActionType)
				{
					case Planning.ScaffoldActionType.Skip:
						skipped++;
						AnsiConsoleHelper.SafeMarkupLine(
							$"Ignorado: {action.FilePath}{(string.IsNullOrWhiteSpace(action.Reason) ? "" : $" ({action.Reason})")}",
							"yellow");
						continue;

					case Planning.ScaffoldActionType.Conflict:
						conflicts++;
						AnsiConsoleHelper.SafeMarkupLine(
							$"Conflito: {action.FilePath}{(string.IsNullOrWhiteSpace(action.Reason) ? "" : $" ({action.Reason})")}",
							"red");
						continue;
				}

				if (options.ConfirmEach)
				{
					var ok = AnsiConsoleHelper.SafeConfirm(
						$"Aplicar {action.ActionType} em '{action.FilePath}'?",
						defaultValue: false);

					if (!ok)
					{
						skipped++;
						AnsiConsoleHelper.SafeMarkupLine($"Ignorado: {action.FilePath}", "yellow");
						continue;
					}
				}

				var directory = Path.GetDirectoryName(action.FilePath);
				if (!string.IsNullOrWhiteSpace(directory))
				{
					Directory.CreateDirectory(directory);
				}

				if (action.Content is null)
				{
					throw new InvalidOperationException(
						$"Missing content for action '{action.ActionType}' on '{action.FilePath}'.");
				}

				File.WriteAllText(
					action.FilePath,
					action.Content,
					new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

				switch (action.ActionType)
				{
					case Planning.ScaffoldActionType.Create:
						created++;
						AnsiConsoleHelper.SafeMarkupLine($"Criado: {action.FilePath}", "green");
						break;
					case Planning.ScaffoldActionType.Update:
						updated++;
						AnsiConsoleHelper.SafeMarkupLine($"Atualizado: {action.FilePath}", "green");
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
