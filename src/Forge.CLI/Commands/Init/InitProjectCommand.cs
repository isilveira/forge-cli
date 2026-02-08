using Forge.CLI.Models;
using Forge.CLI.Persistence;
using static Forge.CLI.Shared.Helpers.ForgeHelper;
using Spectre.Console.Cli;
using Forge.CLI.Shared.Helpers;

namespace Forge.CLI.Commands.Init
{
	public sealed class InitProjectSettings : CommandSettings
	{
		[CommandOption("-n|--name <NAME>")]
		public string? Name { get; set; }
		[CommandOption("-d|--default-id-typed <DEFAULTIDTYPE>")]
		public string DefaultIdType { get; set; } = "Guid";
	}
	public sealed class InitProjectCommand : AsyncCommand<InitProjectSettings>
	{
		public override async Task<int> ExecuteAsync(
			CommandContext context,
			InitProjectSettings settings,
			CancellationToken cancellationToken)
		{
			var loader = new ProjectLoader();
			var saver = new ProjectSaver();

			loader.EnsureNotInitialized();

			var project = new ForgeProject
			{
				Name = settings.Name ?? new DirectoryInfo(
					Directory.GetCurrentDirectory()).Name,
				DefaultIdType = settings.DefaultIdType,
			};

			await saver.SaveAsync(project);

			AnsiConsoleHelper.SafeMarkupLine(
				$"Projeto Forge inicializado!");

			return 0;
		}
    }
}
