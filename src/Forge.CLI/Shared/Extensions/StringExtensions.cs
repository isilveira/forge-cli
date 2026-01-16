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
	}
}
