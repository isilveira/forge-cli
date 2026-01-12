using Forge.CLI.Commands.List;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands
{
	public static class ListForgeCommandGroup
	{
		public static void Register(IConfigurator config)
		{
			config.AddBranch("list", ctx =>
			{
				ctx.AddCommand<ListCommand>("all");
			});
		}
	}
}
