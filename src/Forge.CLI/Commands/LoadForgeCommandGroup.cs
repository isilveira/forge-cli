using Forge.CLI.Commands.Load;
using Spectre.Console.Cli;

namespace Forge.CLI.Commands
{
	public static class LoadForgeCommandGroup
	{
		public static void Register(IConfigurator config)
		{
			config.AddBranch("load", ctx =>
			{
				ctx.AddCommand<LoadSqlCommand>("sql")
					.WithDescription("Load ForgeProject from a SQL script (CREATE TABLE style, e.g. EF migrations)");
			});
		}
	}
}
