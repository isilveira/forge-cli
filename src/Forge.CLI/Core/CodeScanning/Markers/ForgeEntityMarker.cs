namespace Forge.CLI.Core.CodeScanning.Markers
{
	/// <summary>
	/// Representa uma marcação de entidade encontrada no código.
	/// Formato: <forge:entity context="X" name="Y" [description="Z"] [aggregateRoot="true"] [auditable="true"]>
	/// </summary>
	public sealed class ForgeEntityMarker
	{
		/// <summary>
		/// Nome do contexto (bounded context). Default: "Default"
		/// </summary>
		public string Context { get; init; } = "";

		/// <summary>
		/// Nome da entidade (obrigatório).
		/// </summary>
		public string Name { get; init; } = "";

		/// <summary>
		/// Tipo do Id da entidade (obrigatório).
		/// </summary>
		public string IdType { get; init; } = "Guid";

		/// <summary>
		/// Define o nome da tabela no banco de dados para esta entidade. Se não especificado, será gerado a partir do nome da entidade (considerando convenções de pluralização).
		/// </summary>
		public string Table { get; init; } = "";

		/// <summary>
		/// Descrição opcional da entidade.
		/// </summary>
		public string? Description { get; init; }

		/// <summary>
		/// Indica se é uma raiz de agregado. Default: true
		/// </summary>
		public bool? AggregateRoot { get; init; }

		/// <summary>
		/// Indica se a entidade é auditável. Default: true
		/// </summary>
		public bool? Auditable { get; init; }
	}
}