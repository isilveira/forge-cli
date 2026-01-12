// See https://aka.ms/new-console-template for more information
using Forge.CLI.Registers;
using Forge.CLI.Shared.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
	config.SetApplicationName("forge");

	ForgeRegister.Load(config);
});

try
{
	return app.Run(args);
}
catch (Exception ex)
{
	AnsiConsoleHelper.SafeMarkupLine($"Erro: {ex.Message}", "red");
	return -1;
}