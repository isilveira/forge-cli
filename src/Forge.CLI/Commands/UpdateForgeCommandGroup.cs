using Forge.CLI.Commands.Update;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands
{
	public static class UpdateForgeCommandGroup
	{
		public static void Register(IConfigurator config)
		{
			config.AddBranch("update", ctx =>
			{
				ctx.AddCommand<UpdateContextCommand>("context");
				ctx.AddCommand<UpdateEntityCommand>("entity");
				ctx.AddCommand<UpdatePropertyCommand>("property");
				ctx.AddCommand<UpdateRelationCommand>("relation");
			});
		}
	}
}
