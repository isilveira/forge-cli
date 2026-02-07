using BAYSOFT.Abstractions.Crosscutting.Extensions;
using BAYSOFT.Abstractions.Crosscutting.Pluralization;
using BAYSOFT.Abstractions.Crosscutting.Pluralization.English;
using BAYSOFT.Abstractions.Crosscutting.Singularization;
using BAYSOFT.Abstractions.Crosscutting.Singularization.English;

namespace Forge.CLI.Shared.Extensions
{
	public static class StringExtensions
	{
		public static string ToCamel(this string value)
			=> string.IsNullOrEmpty(value)
				? value
				: char.ToLowerInvariant(value[0]) + value[1..];
		public static string Pluralize(this string word)
			=> Pluralizer.GetInstance().AddEnglishPluralizer().Pluralize(word, "en-US");
		public static string PluralizeAsPascal(this string word)
			=> word.ToKebabCase().Pluralize().ToPascalCase();
		public static string Singularize(this string word)
			=> Singularizer.GetInstance().AddEnglishSingularizer().Singularize(word, "en-US");
		public static string SingularizeAsPascal(this string word)
			=> word.ToKebabCase().Singularize().ToPascalCase();
	}
}
