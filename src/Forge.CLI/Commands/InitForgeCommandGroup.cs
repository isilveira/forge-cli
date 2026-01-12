using Forge.CLI.Commands.Init;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands
{
	public static class InitForgeCommandGroup
	{
		public static void Register(IConfigurator config)
		{
			config.AddBranch("init", ctx =>
			{
				ctx.AddCommand<InitProjectCommand>("project");
			});
		}
	}
}
