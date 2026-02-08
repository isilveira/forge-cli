using Forge.CLI.Persistence;
using static Forge.CLI.Shared.Helpers.ForgeHelper;
using Spectre.Console;
using Spectre.Console.Cli;
using Forge.CLI.Shared.Helpers;

namespace Forge.CLI.Commands.List
{
	public sealed class ListSettings : CommandSettings
	{
		[CommandOption("-c|--context")]
		public string? Context { get; set; }

		[CommandOption("-e|--entity")]
		public string? Entity { get; set; }
	}
	public sealed class ListCommand : AsyncCommand<ListSettings>
	{
		public override async Task<int> ExecuteAsync(
			CommandContext context,
			ListSettings settings,
			CancellationToken cancellationToken)
		{
			var loader = new ProjectLoader();

			var project = loader.TryLoad();

			if (project is null)
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Forge not inicialized on this project!", "red");
				return -1;
			}

			if (project.Contexts.Count == 0)
			{
				AnsiConsoleHelper.SafeMarkupLine("No contexts found.", "yellow");
				return 0;
			}

			foreach (var ctx in project.Contexts)
			{
				if (!string.IsNullOrEmpty(settings.Context) &&
					ctx.Key != settings.Context)
					continue;

				var contextNode = new Tree(Markup.Escape(ctx.Key));

				foreach (var entityKvp in ctx.Value.Entities)
				{
					if (!string.IsNullOrEmpty(settings.Entity) &&
						entityKvp.Key != settings.Entity)
						continue;

					var entityNode = contextNode.AddNode(Markup.Escape(entityKvp.Key));

					// Properties
					foreach (var prop in entityKvp.Value.Properties)
					{
						entityNode.AddNode(Markup.Escape($"{prop.Key} ({prop.Value.Type})"));
					}

					// Relations
					foreach (var rel in entityKvp.Value.Relations)
					{
						entityNode.AddNode(Markup.Escape($"relation: {rel.Key} ({rel.Value.Type})"));
					}
				}

				AnsiConsole.Write(contextNode);
			}

			return 0;
		}
	}
}