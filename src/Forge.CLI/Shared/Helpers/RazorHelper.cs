using Forge.CLI.Shared.Extensions;

namespace Forge.CLI.Shared.Helpers
{
    public static class RazorHelper
    {
        public static string ResolveResourceKey(string resourceKey)
            => resourceKey.Replace(".", "_").Replace("-", "_");

        public static string Pluralize(string word)
            => word.PluralizeAsPascal();
	}
}
