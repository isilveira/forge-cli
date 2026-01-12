using Forge.CLI.Commands.Remove;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands
{
	public static class RemoveForgeCommandGroup
	{
		public static void Register(IConfigurator config)
		{
			config.AddBranch("remove", ctx =>
			{
				ctx.AddCommand<RemoveContextCommand>("context");
				ctx.AddCommand<RemoveEntityCommand>("entity");
				ctx.AddCommand<RemovePropertyCommand>("property");
				ctx.AddCommand<RemoveRelationCommand>("relation");
			});
		}
	}
}
