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

			var localRoot = Path.Combine(projectRoot, ".forge", "Artifacts");
			var scaffoldingRoot = Path.Combine(projectRoot, "Scaffolding", "Artifacts");

			var loadedFromDisk = false;

			if (Directory.Exists(localRoot))
			{
				ProcessFileSystemArtifacts(localRoot, artifacts, errors);
				loadedFromDisk = true;
			}

			if (Directory.Exists(scaffoldingRoot))
			{
				ProcessFileSystemArtifacts(scaffoldingRoot, artifacts, errors);
				loadedFromDisk = true;
			}

			if (!loadedFromDisk)
			{
				errors.Add(
					$"Artifacts folder not found under .forge/Artifacts or Scaffolding/Artifacts. Falling back to embedded artifacts.");
			}

			ProcessEmbeddedArtifacts(artifacts, errors);

			// Prefer filesystem over embedded when the same id appears more than once.
			artifacts = PreferFilesystemOverEmbedded(artifacts, errors);

			return new ArtifactDiscoveryResult
			{
				Artifacts = artifacts,
				Errors = errors
			};
		}

		private void ProcessFileSystemArtifacts(
			string artifactsRoot,
			List<ArtifactDescriptor> artifacts,
			List<string> errors)
		{
			foreach (var layerDir in Directory.GetDirectories(artifactsRoot))
			{
				var layer = Path.GetFileName(layerDir);

				foreach (var file in Directory.GetFiles(layerDir, "*.yaml", SearchOption.AllDirectories))
				{
					var relative = Path.GetRelativePath(layerDir, file);
					var resolverFileName = relative
						.Replace(Path.DirectorySeparatorChar, '.')
						.Replace(Path.AltDirectorySeparatorChar, '.');

					ProcessFile(layer, file, resolverFileName, artifacts, errors);
				}
			}
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
				var fileName = rest.Substring(firstDot + 1); // e.g. "entity.yaml" or "Service.Update.yaml"

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
			string resolverFileName,
			List<ArtifactDescriptor> artifacts,
			List<string> errors)
		{
			var yaml = File.ReadAllText(filePath);
			ProcessYaml(layerFromFolder, resolverFileName, filePath, yaml, artifacts, errors);
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

		private static List<ArtifactDescriptor> PreferFilesystemOverEmbedded(
			List<ArtifactDescriptor> artifacts,
			List<string> errors)
		{
			var result = new List<ArtifactDescriptor>();

			foreach (var group in artifacts.GroupBy(a => a.Id, StringComparer.OrdinalIgnoreCase))
			{
				var items = group.ToList();
				if (items.Count == 1)
				{
					result.Add(items[0]);
					continue;
				}

				var filesystem = items
					.Where(a => !a.SourceFile.StartsWith("embedded:", StringComparison.OrdinalIgnoreCase))
					.ToList();

				if (filesystem.Count == 1)
				{
					result.Add(filesystem[0]);
					continue;
				}

				if (filesystem.Count > 1)
				{
					errors.Add($"Duplicate artifact id detected on filesystem: {group.Key}");
					continue;
				}

				errors.Add($"Duplicate artifact id detected: {group.Key}");
			}

			return result;
		}
	}
}
