namespace Forge.CLI.Core.CodeScanning.Markers
{
	/// <summary>
	/// Representa uma marcação de propriedade encontrada no código.
	/// Formato: <forge:property entity="X" name="Y" type="Z" [required="true"] [length="100"] [precision="18"] [scale="2"]>
	/// </summary>
	public sealed class ForgePropertyMarker
	{
		/// <summary>
		/// Nome da entidade à qual a propriedade pertence (obrigatório).
		/// </summary>
		public string Entity { get; init; } = "";

		/// <summary>
		/// Nome da propriedade (obrigatório).
		/// </summary>
		public string Name { get; init; } = "";

		/// <summary>
		/// Tipo da propriedade (obrigatório). Ex: string, int, decimal, DateTime
		/// </summary>
		public string Type { get; init; } = "";

		/// <summary>
		/// Contexto opcional (se não especificado, busca em todos os contextos).
		/// </summary>
		public string? Context { get; init; }

		/// <summary>
		/// Indica se a propriedade é obrigatória. Default: false
		/// </summary>
		public bool? Required { get; init; }

		/// <summary>
		/// Comprimento máximo para strings.
		/// </summary>
		public int? Length { get; init; }

		/// <summary>
		/// Comprimento máximo para strings.
		/// </summary>
		public bool? HasMaxLength { get; init; }

		/// <summary>
		/// Precisão para tipos decimais.
		/// </summary>
		public int? Precision { get; init; }

		/// <summary>
		/// Escala para tipos decimais.
		/// </summary>
		public int? Scale { get; init; }

		/// <summary>
		/// DbColumn opcional para mapear a propriedade a uma coluna específica no banco de dados.
		/// </summary>
		public string? DbColumn { get; init; }

		/// <summary>
		/// Distinga se a propriedade deve ser exibida em componentes de seleção. Default: false
		/// </summary>
		public bool? DisplayOnSelect { get; init; }
	}
}