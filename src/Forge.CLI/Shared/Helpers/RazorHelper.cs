using Forge.CLI.Shared.Extensions;

namespace Forge.CLI.Shared.Helpers
{
    public static class RazorHelper
    {
        public static string ResolveResourceKey(string resourceKey)
            => resourceKey.Replace(".", "_").Replace("-", "_");

        public static string Pluralize(string word)
            => word.PluralizeAsPascal();

        public static string Singularize(string word)
            => word.SingularizeAsPascal();

        public static string Tab(int count, string tabPattern = "    ")
        {
            var tab = string.Empty;
            for (var i = 0; i < count; i++)
            {
                tab += tabPattern;
            }
            return tab;
        }
	}
}
