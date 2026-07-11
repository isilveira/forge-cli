using System.Reflection;

namespace Forge.CLI.Core.Artifacts
{
	public sealed class EmbeddedArtifactExportItem
	{
		public required string ResourceName { get; init; }
		public required string RelativePath { get; init; }
		public required string DestinationPath { get; init; }
		public bool Exists { get; init; }
	}

	public sealed class EmbeddedArtifactExportResult
	{
		public List<string> Written { get; } = [];
		public List<string> Skipped { get; } = [];
		public List<string> WouldWrite { get; } = [];
		public List<string> Errors { get; } = [];
	}

	/// <summary>
	/// Copies embedded YAML artifacts under <c>Scaffolding/Artifacts</c>
	/// into <c>.forge/Artifacts</c>.
	/// </summary>
	public sealed class EmbeddedArtifactExporter
	{
		private const string ForgeFolder = ".forge";
		private const string ArtifactsFolder = "Artifacts";
		private const string ResourceSegment = "Scaffolding.Artifacts.";

		public IReadOnlyList<EmbeddedArtifactExportItem> Discover(string projectRoot)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var rootNamespace = assembly.GetName().Name!;
			var prefix = rootNamespace + "." + ResourceSegment;
			var artifactsRoot = Path.Combine(projectRoot, ForgeFolder, ArtifactsFolder);

			var items = new List<EmbeddedArtifactExportItem>();

			foreach (var resourceName in assembly.GetManifestResourceNames())
			{
				if (!resourceName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
					continue;
				if (!resourceName.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase))
					continue;

				if (!TryMapToRelativePath(resourceName, prefix, out var relativePath))
					continue;

				var destinationPath = Path.Combine(
					artifactsRoot,
					relativePath.Replace('/', Path.DirectorySeparatorChar));

				items.Add(new EmbeddedArtifactExportItem
				{
					ResourceName = resourceName,
					RelativePath = relativePath,
					DestinationPath = destinationPath,
					Exists = File.Exists(destinationPath)
				});
			}

			return items
				.OrderBy(i => i.RelativePath, StringComparer.OrdinalIgnoreCase)
				.ToList();
		}

		public async Task<EmbeddedArtifactExportResult> ExportAsync(
			string projectRoot,
			bool force,
			bool whatIf,
			CancellationToken cancellationToken)
		{
			var result = new EmbeddedArtifactExportResult();
			var assembly = Assembly.GetExecutingAssembly();
			var items = Discover(projectRoot);

			foreach (var item in items)
			{
				cancellationToken.ThrowIfCancellationRequested();

				if (item.Exists && !force)
				{
					result.Skipped.Add(item.RelativePath);
					continue;
				}

				if (whatIf)
				{
					result.WouldWrite.Add(item.RelativePath);
					continue;
				}

				try
				{
					await using var stream = assembly.GetManifestResourceStream(item.ResourceName);
					if (stream is null)
					{
						result.Errors.Add($"Falha ao ler recurso embutido: {item.ResourceName}");
						continue;
					}

					var directory = Path.GetDirectoryName(item.DestinationPath);
					if (!string.IsNullOrEmpty(directory))
						Directory.CreateDirectory(directory);

					await using var file = File.Create(item.DestinationPath);
					await stream.CopyToAsync(file, cancellationToken);

					result.Written.Add(item.RelativePath);
				}
				catch (Exception ex)
				{
					result.Errors.Add($"{item.RelativePath}: {ex.Message}");
				}
			}

			return result;
		}

		internal static bool TryMapToRelativePath(
			string resourceName,
			string prefix,
			out string relativePath)
		{
			relativePath = string.Empty;
			var rest = resourceName.Substring(prefix.Length);

			const string extension = ".yaml";
			if (!rest.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
				return false;

			var withoutExtension = rest.Substring(0, rest.Length - extension.Length);
			if (string.IsNullOrWhiteSpace(withoutExtension))
				return false;

			relativePath = withoutExtension.Replace('.', '/') + extension;
			return true;
		}
	}
}
