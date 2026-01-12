using Forge.CLI.Commands.Scaffold;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands
{
	public static class ScaffoldForgeCommandGroup
	{
		public static void Register(IConfigurator config)
		{
			config.AddBranch("scaffold", ctx =>
			{
				ctx.AddCommand<ScaffoldEntityCommand>("entity");
			});
		}
	}
}
