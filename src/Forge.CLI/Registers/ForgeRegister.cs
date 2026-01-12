using Forge.CLI.Commands;
using Spectre.Console.Cli;

namespace Forge.CLI.Registers
{
	public static class ForgeRegister
	{
		public static void Load(IConfigurator config)
		{
			InitForgeCommandGroup.Register(config);
			AddForgeCommandGroup.Register(config);
			UpdateForgeCommandGroup.Register(config);
			RemoveForgeCommandGroup.Register(config);
			ListForgeCommandGroup.Register(config);
			ScaffoldForgeCommandGroup.Register(config);
		}
	}

}
