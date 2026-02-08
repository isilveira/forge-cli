using Forge.CLI.Core.CodeScanning.Markers;
using System.Text.RegularExpressions;

namespace Forge.CLI.Core.CodeScanning.Parsing
{
	/// <summary>
	/// Parser de marcações Forge encontradas no código-fonte.
	/// 
	/// Formatos suportados:
	/// - <forge:entity context="X" name="Y" [description="Z"] [aggregateRoot="true"] [auditable="true"]>
	/// - <forge:property entity="X" name="Y" type="Z" [required="true"] [length="100"] [precision="18"] [scale="2"] [context="C"]>
	/// - <forge:relationship from="X" to="Y" kind="one-to-many" [required="true"] [context="C"]>
	/// 
	/// O parser é extensível: novos tipos de marcação podem ser adicionados ao switch.
	/// </summary>
	public sealed class MarkerParser
	{
		// Regex compilado para performance em múltiplas execuções
		private static readonly Regex MarkerRegex =
			new(@"<forge:(\w+)\s+(.*?)>",
				RegexOptions.Compiled);

		// Regex para extração de atributos key="value"
		private static readonly Regex AttributeRegex =
			new(@"(\w+)=""(.*?)""",
				RegexOptions.Compiled);

		/// <summary>
		/// Faz parsing de um RawForgeMarker para o tipo de marker específico.
		/// </summary>
		/// <param name="marker">Marcação bruta extraída do código</param>
		/// <returns>Objeto tipado (ForgeEntityMarker, ForgePropertyMarker, etc.)</returns>
		/// <exception cref="ForgeMarkerParseException">Quando a marcação é inválida</exception>
		public object Parse(RawForgeMarker marker)
		{
			var match = MarkerRegex.Match(marker.RawText);

			if (!match.Success)
				throw new ForgeMarkerParseException(
					marker.FilePath,
					marker.LineNumber,
					"Marcador inválido");

			var type = match.Groups[1].Value;
			var payload = match.Groups[2].Value;

			var attributes = ParseAttributes(payload);

			return type switch
			{
				"entity" => ParseEntityMarker(attributes),
				"property" => ParsePropertyMarker(attributes),
				"relationship" => ParseRelationshipMarker(attributes),
				_ => throw new ForgeMarkerParseException(
					marker.FilePath,
					marker.LineNumber,
					$"Tipo desconhecido: {type}")
			};
		}

		/// <summary>
		/// Cria um ForgeEntityMarker a partir dos atributos parseados.
		/// </summary>
		private static ForgeEntityMarker ParseEntityMarker(Dictionary<string, string> attrs)
		{
			return new ForgeEntityMarker
			{
				Context = attrs.GetValueOrDefault("context", ""),
				Name = attrs.GetValueOrDefault("name", ""),
				IdType = attrs.GetValueOrDefault("id-type", "Guid"),
				Table = attrs.GetValueOrDefault("table", ""),
				Description = attrs.GetValueOrDefault("description"),
				AggregateRoot = ParseBool(attrs.GetValueOrDefault("aggregateRoot")),
				Auditable = ParseBool(attrs.GetValueOrDefault("auditable"))
			};
		}

		/// <summary>
		/// Cria um ForgePropertyMarker a partir dos atributos parseados.
		/// </summary>
		private static ForgePropertyMarker ParsePropertyMarker(Dictionary<string, string> attrs)
		{
			return new ForgePropertyMarker
			{
				Entity = attrs.GetValueOrDefault("entity", ""),
				Name = attrs.GetValueOrDefault("name", ""),
				Type = attrs.GetValueOrDefault("type", ""),
				Context = attrs.GetValueOrDefault("context"),
				Required = ParseBool(attrs.GetValueOrDefault("required")),
				Length = ParseInt(attrs.GetValueOrDefault("length")),
				HasMaxLength = ParseBool(attrs.GetValueOrDefault("has-max-length")),
				Precision = ParseInt(attrs.GetValueOrDefault("precision")),
				Scale = ParseInt(attrs.GetValueOrDefault("scale")),
				DbColumn = attrs.GetValueOrDefault("db-column"),
				DisplayOnSelect = ParseBool(attrs.GetValueOrDefault("display-on-select"))
			};
		}

		/// <summary>
		/// Cria um ForgeRelationshipMarker a partir dos atributos parseados.
		/// </summary>
		private static ForgeRelationshipMarker ParseRelationshipMarker(Dictionary<string, string> attrs)
		{
			return new ForgeRelationshipMarker
			{
				From = attrs.GetValueOrDefault("from", ""),
				To = attrs.GetValueOrDefault("to", ""),
				Kind = attrs.GetValueOrDefault("kind", ""),
				Context = attrs.GetValueOrDefault("context"),
				Required = ParseBool(attrs.GetValueOrDefault("required"))
			};
		}

		/// <summary>
		/// Extrai atributos key="value" de uma string.
		/// </summary>
		private static Dictionary<string, string> ParseAttributes(string input)
		{
			var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			foreach (Match m in AttributeRegex.Matches(input))
				dict[m.Groups[1].Value] = m.Groups[2].Value;

			return dict;
		}

		/// <summary>
		/// Converte string para bool nullable.
		/// Aceita: "true", "false", "1", "0", "yes", "no"
		/// </summary>
		private static bool? ParseBool(string? value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;

			return value.ToLowerInvariant() switch
			{
				"true" or "1" or "yes" => true,
				"false" or "0" or "no" => false,
				_ => null
			};
		}

		/// <summary>
		/// Converte string para int nullable.
		/// </summary>
		private static int? ParseInt(string? value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;

			return int.TryParse(value, out var result) ? result : null;
		}
	}
}
