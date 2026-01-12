using Forge.CLI.Commands.Add;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands
{
	public static class AddForgeCommandGroup
	{
		public static void Register(IConfigurator config)
		{
			config.AddBranch("add", ctx =>
			{
				ctx.AddCommand<AddContextCommand>("context");
				ctx.AddCommand<AddEntityCommand>("entity");
				ctx.AddCommand<AddPropertyCommand>("property");
				ctx.AddCommand<AddRelationCommand>("relation");
			});
		}
	}
}
