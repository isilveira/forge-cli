namespace Forge.CLI.Core.CodeScanning.Markers
{
	/// <summary>
	/// Representa uma marcação de relacionamento encontrada no código.
	/// Formato: <forge:relationship from="X" to="Y" kind="one-to-many" [required="true"] [context="Z"]>
	/// </summary>
	public sealed class ForgeRelationshipMarker
	{
		/// <summary>
		/// Entidade de origem do relacionamento (obrigatório).
		/// </summary>
		public string From { get; init; } = "";

		/// <summary>
		/// Entidade de destino do relacionamento (obrigatório).
		/// </summary>
		public string To { get; init; } = "";

		/// <summary>
		/// Tipo do relacionamento: one-to-many, many-to-one, many-to-many, one-to-one
		/// </summary>
		public string Kind { get; init; } = "";

		/// <summary>
		/// Contexto opcional da entidade de origem.
		/// </summary>
		public string? Context { get; init; }

		/// <summary>
		/// Indica se o relacionamento é obrigatório. Default: false
		/// </summary>
		public bool? Required { get; init; }
	}
}