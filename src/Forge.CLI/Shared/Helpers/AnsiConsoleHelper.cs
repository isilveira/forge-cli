using Spectre.Console;

namespace Forge.CLI.Shared.Helpers
{
    public static class AnsiConsoleHelper
    {
        public static void SafeMarkupLine(string text, string color = "green")
        {
            try
            {
                AnsiConsole.MarkupLine($"[{color}]{Markup.Escape(text)}[/]");
            }
            catch
            {
                // Ignore markup errors
                AnsiConsole.WriteLine(Markup.Escape(text));
            }
		}
        public static bool SafeConfirm(string message, bool defaultValue = false)
        {
            try
            {
                return AnsiConsole.Confirm(Markup.Escape(message), defaultValue);
            }
            catch
            {
                // Ignore markup errors
                Console.WriteLine(Markup.Escape(message) + $" (y/n) [default: {(defaultValue ? "y" : "n")}]");
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    return defaultValue;
                }
                return input.Trim().ToLower() switch
                {
                    "y" or "yes" => true,
                    "n" or "no" => false,
                    _ => defaultValue,
                };
            }
		}
	}
}
