namespace Forge.CLI.Core.CodeScanning.Scanning
{
	public sealed class FileScanner
	{
		public IEnumerable<string> Scan(ScanOptions options)
		{
			return Directory
				.EnumerateFiles(options.RootPath, "*.*", SearchOption.AllDirectories)
				.Where(path =>
				{
					var ext = Path.GetExtension(path);

					if (!options.AllowedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
						return false;

					return !options.IgnoredFolders.Any(f =>
						path.Contains($"{Path.DirectorySeparatorChar}{f}{Path.DirectorySeparatorChar}"));
				});
		}
	}
}