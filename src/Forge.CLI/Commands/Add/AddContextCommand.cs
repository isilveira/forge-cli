using Forge.CLI.Models;
using Forge.CLI.Persistence;
using static Forge.CLI.Shared.Helpers.ForgeHelper;
using Spectre.Console.Cli;
using Forge.CLI.Shared.Helpers;

namespace Forge.CLI.Commands.Add
{
	public sealed class AddContextSettings : CommandSettings
	{
		[CommandArgument(0, "<context>")]
		public string Context { get; set; } = default!;

		[CommandOption("-d|--description <DESCRIPTION>")]
		public string? Description { get; set; }
	}
	public sealed class AddContextCommand : AsyncCommand<AddContextSettings>
	{
		public override async Task<int> ExecuteAsync(CommandContext context, AddContextSettings settings, CancellationToken cancellationToken)
		{
			var loader = new ProjectLoader();
			var saver = new ProjectSaver();

			var project = loader.TryLoad();

			if (project is null)
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Forge not inicialized on this project!", "red");
				return -1;
			}

			if (project.Contexts.ContainsKey(settings.Context))
			{
				AnsiConsoleHelper.SafeMarkupLine(
					$"Context '{settings.Context}' já existe!","red");
				return -1;
			}

			project.Contexts[settings.Context] = new ForgeContext
			{
				Description = settings.Description!,
				Entities = new Dictionary<string, ForgeEntity>()
			};

			await saver.SaveAsync(project);

			AnsiConsoleHelper.SafeMarkupLine(
				$"Context '{settings.Context}' criado com sucesso!");

			return 0;
		}
    }
}