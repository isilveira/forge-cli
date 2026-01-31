namespace Forge.CLI.Core.CodeScanning.Merging
{
	/// <summary>
	/// Resultado do processo de merge entre projetos.
	/// Contém estatísticas e rastreabilidade das alterações realizadas.
	/// </summary>
	public sealed class MergeResult
	{
		/// <summary>
		/// Número de contextos criados durante o merge.
		/// </summary>
		public int ContextsCreated { get; set; }

		/// <summary>
		/// Número de entidades criadas durante o merge.
		/// </summary>
		public int EntitiesCreated { get; set; }

		/// <summary>
		/// Número de entidades atualizadas durante o merge.
		/// </summary>
		public int EntitiesUpdated { get; set; }

		/// <summary>
		/// Número de propriedades criadas durante o merge.
		/// </summary>
		public int PropertiesCreated { get; set; }

		/// <summary>
		/// Número de propriedades atualizadas durante o merge.
		/// </summary>
		public int PropertiesUpdated { get; set; }

		/// <summary>
		/// Número de relacionamentos criados durante o merge.
		/// </summary>
		public int RelationsCreated { get; set; }

		/// <summary>
		/// Número de relacionamentos atualizados durante o merge.
		/// </summary>
		public int RelationsUpdated { get; set; }

		/// <summary>
		/// Número de itens ignorados por já existirem e overwrite estar desabilitado.
		/// </summary>
		public int Skipped { get; set; }

		/// <summary>
		/// Lista de avisos gerados durante o merge.
		/// </summary>
		public List<string> Warnings { get; } = [];

		/// <summary>
		/// Total de alterações realizadas.
		/// </summary>
		public int TotalChanges =>
			ContextsCreated +
			EntitiesCreated + EntitiesUpdated +
			PropertiesCreated + PropertiesUpdated +
			RelationsCreated + RelationsUpdated;

		/// <summary>
		/// Indica se houve alterações no projeto.
		/// </summary>
		public bool HasChanges => TotalChanges > 0;

		/// <summary>
		/// Gera um resumo textual do resultado do merge.
		/// </summary>
		public string GetSummary()
		{
			var parts = new List<string>();

			if (ContextsCreated > 0)
				parts.Add($"{ContextsCreated} context(s) created");

			if (EntitiesCreated > 0)
				parts.Add($"{EntitiesCreated} entity(ies) created");

			if (EntitiesUpdated > 0)
				parts.Add($"{EntitiesUpdated} entity(ies) updated");

			if (PropertiesCreated > 0)
				parts.Add($"{PropertiesCreated} property(ies) created");

			if (PropertiesUpdated > 0)
				parts.Add($"{PropertiesUpdated} property(ies) updated");

			if (RelationsCreated > 0)
				parts.Add($"{RelationsCreated} relation(s) created");

			if (RelationsUpdated > 0)
				parts.Add($"{RelationsUpdated} relation(s) updated");

			if (Skipped > 0)
				parts.Add($"{Skipped} item(s) skipped");

			return parts.Count > 0
				? string.Join(", ", parts)
				: "No changes";
		}
	}
}
