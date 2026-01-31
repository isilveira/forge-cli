namespace Forge.CLI.Core.CodeScanning.Scanning
{
	/// <summary>
	/// Opções de configuração para o scanner de arquivos.
	/// Record permite uso de 'with' para criar cópias modificadas.
	/// </summary>
	public sealed record ScanOptions
	{
		/// <summary>
		/// Diretório raiz para iniciar o scan.
		/// </summary>
		public string RootPath { get; init; } = Directory.GetCurrentDirectory();

		/// <summary>
		/// Extensões de arquivo permitidas para scan.
		/// </summary>
		public string[] AllowedExtensions { get; init; } = [".cs"];

		/// <summary>
		/// Pastas ignoradas durante o scan.
		/// </summary>
		public string[] IgnoredFolders { get; init; } = ["bin", "obj", ".git", "node_modules", ".forge"];
	}
}