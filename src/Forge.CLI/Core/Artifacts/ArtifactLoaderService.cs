using Forge.CLI.Core.Artifacts.Interfaces;
using Forge.CLI.Core.Artifacts.Results;
using Forge.CLI.Core.Artifacts.Validators;

namespace Forge.CLI.Core.Artifacts
{
	public sealed class ArtifactLoaderService
	{
		private readonly IArtifactLoader _loader;
		private readonly ArtifactStructuralValidator _structuralValidator = new();
        public ArtifactLoaderService(IArtifactLoader loader)
        {
            _loader = loader;
		}
        public ArtifactLoadResult Load(string yamlContent)
		{
			ArtifactDefinition artifact;

			try
			{
				artifact = _loader.Load(yamlContent);
			}
			catch (Exception ex)
			{
				return new ArtifactLoadResult
				{
					Errors = new[] { $"YAML parsing error: {ex.Message}" }
				};
			}

			var structuralResult = _structuralValidator.Validate(artifact);
			if (!structuralResult.IsValid)
			{
				return new ArtifactLoadResult
				{
					Errors = structuralResult.Errors
						.Select(e => e.ErrorMessage)
						.ToArray()
				};
			}

			var semanticErrors = ArtifactSemanticValidator.Validate(artifact).ToArray();
			if (semanticErrors.Any())
			{
				return new ArtifactLoadResult
				{
					Errors = semanticErrors
				};
			}

			return new ArtifactLoadResult
			{
				Artifact = artifact
			};
		}
	}
}