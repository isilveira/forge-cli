using Forge.CLI.Core.Artifacts.Interfaces;
using Forge.CLI.Core.Artifacts.Results;
using System.Reflection;

namespace Forge.CLI.Core.Artifacts
{
	public sealed class ArtifactDiscoveryService
	{
		private readonly ArtifactLoaderService _loaderService;
        public ArtifactDiscoveryService(IArtifactLoader artifactLoader)
        {
			_loaderService = new ArtifactLoaderService(artifactLoader);
		}
        public ArtifactDiscoveryResult Discover(string projectRoot)
		{
			var artifacts = new List<ArtifactDescriptor>();
			var errors = new List<string>();

			var artifactsRoot = Path.Combine(projectRoot, "Scaffolding", "Artifacts");

			// Prefer file system artifacts when present (useful in dev), otherwise fall back to embedded artifacts.
			if (Directory.Exists(artifactsRoot))
			{
				foreach (var layerDir in Directory.GetDirectories(artifactsRoot))
				{
					var layer = Path.GetFileName(layerDir);

					foreach (var file in Directory.GetFiles(layerDir, "*.yaml"))
					{
						ProcessFile(layer, file, artifacts, errors);
					}
				}
			}
			else
			{
				errors.Add($"Artifacts folder not found: {artifactsRoot}. Falling back to embedded artifacts.");
			}

			ProcessEmbeddedArtifacts(artifacts, errors);

			// Conflito de ID
			var duplicatedIds = artifacts
				.GroupBy(a => a.Id, StringComparer.OrdinalIgnoreCase)
				.Where(g => g.Count() > 1)
				.Select(g => g.Key);

			foreach (var id in duplicatedIds)
			{
				errors.Add($"Duplicate artifact id detected: {id}");
			}

			artifacts = artifacts
				.Where(a => !duplicatedIds.Contains(a.Id, StringComparer.OrdinalIgnoreCase))
				.ToList();

			return new ArtifactDiscoveryResult
			{
				Artifacts = artifacts,
				Errors = errors
			};
		}

		private void ProcessEmbeddedArtifacts(
			List<ArtifactDescriptor> artifacts,
			List<string> errors)
		{
			// Resource name example:
			//   Forge.CLI.Scaffolding.Artifacts.Domain.entity.yaml
			var assembly = Assembly.GetExecutingAssembly();
			var rootNamespace = assembly.GetName().Name!;
			var prefix = rootNamespace + ".Scaffolding.Artifacts.";

			foreach (var resourceName in assembly.GetManifestResourceNames())
			{
				if (!resourceName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
					continue;
				if (!resourceName.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase))
					continue;

				var rest = resourceName.Substring(prefix.Length); // e.g. "Domain.entity.yaml"
				var firstDot = rest.IndexOf('.');
				if (firstDot <= 0)
				{
					errors.Add($"Invalid embedded artifact resource name: {resourceName}");
					continue;
				}

				var layer = rest.Substring(0, firstDot);
				var fileName = rest.Substring(firstDot + 1); // e.g. "entity.yaml"

				string yaml;
				try
				{
					using var stream = assembly.GetManifestResourceStream(resourceName);
					if (stream is null)
					{
						errors.Add($"Failed to read embedded artifact: {resourceName}");
						continue;
					}

					using var reader = new StreamReader(stream);
					yaml = reader.ReadToEnd();
				}
				catch (Exception ex)
				{
					errors.Add($"{resourceName}: {ex.Message}");
					continue;
				}

				ProcessYaml(layer, fileName, $"embedded:{resourceName}", yaml, artifacts, errors);
			}
		}

		private void ProcessFile(
			string layerFromFolder,
			string filePath,
			List<ArtifactDescriptor> artifacts,
			List<string> errors)
		{
			var fileName = Path.GetFileName(filePath);
			var yaml = File.ReadAllText(filePath);

			ProcessYaml(layerFromFolder, fileName, filePath, yaml, artifacts, errors);
		}

		private void ProcessYaml(
			string layerFromFolder,
			string fileName,
			string source,
			string yaml,
			List<ArtifactDescriptor> artifacts,
			List<string> errors)
		{
			if (!ArtifactFileNameResolver.TryResolve(
				fileName,
				out var type,
				out var variant))
			{
				errors.Add($"Invalid artifact filename: {source}");
				return;
			}

			var loadResult = _loaderService.Load(yaml);

			if (!loadResult.IsValid)
			{
				foreach (var error in loadResult.Errors)
				{
					errors.Add($"{source}: {error}");
				}
				return;
			}

			var artifact = loadResult.Artifact!;

			// Validação folder × YAML
			if (!string.Equals(layerFromFolder, artifact.Layer, StringComparison.OrdinalIgnoreCase))
			{
				errors.Add(
					$"{source}: layer mismatch. Folder='{layerFromFolder}', YAML='{artifact.Layer}'");
				return;
			}

			if (!string.Equals(type, artifact.Type, StringComparison.OrdinalIgnoreCase))
			{
				errors.Add(
					$"{source}: type mismatch. Filename='{type}', YAML='{artifact.Type}'");
				return;
			}

			if (!string.Equals(variant, artifact.Variant, StringComparison.OrdinalIgnoreCase))
			{
				errors.Add(
					$"{source}: variant mismatch. Filename='{variant}', YAML='{artifact.Variant}'");
				return;
			}

			artifacts.Add(new ArtifactDescriptor
			{
				Id = artifact.Artifact.Id,
				Layer = artifact.Layer,
				Type = artifact.Type,
				Variant = artifact.Variant,
				Definition = artifact,
				SourceFile = source
			});
		}
	}
}