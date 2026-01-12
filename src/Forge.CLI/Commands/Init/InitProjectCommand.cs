using Forge.CLI.Models;
using Forge.CLI.Shared.Helpers;
using Forge.CLI.Tools;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands.Init
{
	public sealed class InitProjectSettings : CommandSettings
	{
		[CommandOption("-n|--name <NAME>")]
		public string? Name { get; set; }
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
					Directory.GetCurrentDirectory()).Name
			};

			await saver.SaveAsync(project);

			AnsiConsoleHelper.SafeMarkupLine(
				$"Projeto Forge inicializado!");

			return 0;
		}
    }
}
