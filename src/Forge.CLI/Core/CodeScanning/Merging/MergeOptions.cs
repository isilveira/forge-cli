namespace Forge.CLI.Core.CodeScanning.Merging
{
	/// <summary>
	/// Opções de configuração para o processo de merge entre projetos.
	/// Controla como conflitos são resolvidos e quais dados podem ser sobrescritos.
	/// </summary>
	public sealed class MergeOptions
	{
		/// <summary>
		/// Se true, sobrescreve entidades existentes com dados do scan.
		/// Se false, mantém dados existentes e adiciona apenas novos.
		/// Default: false (preserva dados manuais do usuário)
		/// </summary>
		public bool OverwriteEntities { get; init; } = false;

		/// <summary>
		/// Se true, sobrescreve propriedades existentes com dados do scan.
		/// Se false, mantém propriedades existentes e adiciona apenas novas.
		/// Default: false
		/// </summary>
		public bool OverwriteProperties { get; init; } = false;

		/// <summary>
		/// Se true, sobrescreve relacionamentos existentes com dados do scan.
		/// Se false, mantém relacionamentos existentes e adiciona apenas novos.
		/// Default: false
		/// </summary>
		public bool OverwriteRelations { get; init; } = false;

		/// <summary>
		/// Se true, cria contextos que existem no scan mas não no projeto base.
		/// Default: true
		/// </summary>
		public bool CreateMissingContexts { get; init; } = true;

		/// <summary>
		/// Se true, cria entidades que existem no scan mas não no projeto base.
		/// Default: true
		/// </summary>
		public bool CreateMissingEntities { get; init; } = true;

		/// <summary>
		/// Se true, cria propriedades que existem no scan mas não no projeto base.
		/// Default: true
		/// </summary>
		public bool CreateMissingProperties { get; init; } = true;

		/// <summary>
		/// Se true, cria relacionamentos que existem no scan mas não no projeto base.
		/// Default: true
		/// </summary>
		public bool CreateMissingRelations { get; init; } = true;

		/// <summary>
		/// Preset: apenas adiciona novos itens, nunca sobrescreve.
		/// Comportamento mais seguro para preservar dados do usuário.
		/// </summary>
		public static MergeOptions AddOnly => new()
		{
			OverwriteEntities = false,
			OverwriteProperties = false,
			OverwriteRelations = false,
			CreateMissingContexts = true,
			CreateMissingEntities = true,
			CreateMissingProperties = true,
			CreateMissingRelations = true
		};

		/// <summary>
		/// Preset: sobrescreve tudo com dados do scan.
		/// Use com cautela - pode perder dados manuais do usuário.
		/// </summary>
		public static MergeOptions OverwriteAll => new()
		{
			OverwriteEntities = true,
			OverwriteProperties = true,
			OverwriteRelations = true,
			CreateMissingContexts = true,
			CreateMissingEntities = true,
			CreateMissingProperties = true,
			CreateMissingRelations = true
		};
	}
}
