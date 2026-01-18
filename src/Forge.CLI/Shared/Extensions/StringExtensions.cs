using BAYSOFT.Abstractions.Crosscutting.Pluralization;
using BAYSOFT.Abstractions.Crosscutting.Pluralization.English;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.CLI.Shared.Extensions
{
	public static class StringExtensions
	{
		public static string ToCamel(this string value)
			=> string.IsNullOrEmpty(value)
				? value
				: char.ToLowerInvariant(value[0]) + value[1..];
		public static string ToPascal(this string value)
			=> string.IsNullOrEmpty(value)
				? value
				: char.ToUpperInvariant(value[0]) + value[1..];
		public static string Pluralize(this string word)
			=> Pluralizer.GetInstance().AddEnglishPluralizer().Pluralize(word, "en-US");
		public static string PluralizeAsPascal(this string word)
			=> ToPascal(Pluralizer.GetInstance().AddEnglishPluralizer().Pluralize(word, "en-US"));
	}
}
