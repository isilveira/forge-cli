using static Forge.CLI.Scaffolding.Types.ScaffoldTypes;

namespace Forge.CLI.Scaffolding.Types
{
	public static class ScaffoldTypeParser
	{
		public static IReadOnlyList<EntityScaffoldType> Parse(string? input)
		{
			if (string.IsNullOrWhiteSpace(input) || input.Equals("all", StringComparison.OrdinalIgnoreCase))
				return Enum.GetValues<EntityScaffoldType>();

			return input.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(t => Enum.Parse<EntityScaffoldType>(t, true))
				.ToList();
		}
	}
}
