using Forge.CLI.Commands.Scan;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands
{
	public static class ScanForgeCommandGroup
	{
		public static void Register(IConfigurator config)
		{
			config.AddBranch("scan", ctx =>
			{
				ctx.AddCommand<ScanCommand>("markers")
					.WithDescription("Scan source code for Forge markers and update project.json");
			});
		}
	}
}
