namespace Forge.CLI.Core.Artifacts.Validators
{
	public static class ArtifactSemanticValidator
	{
		public static IEnumerable<string> Validate(ArtifactDefinition artifact)
		{
			var errors = new List<string>();

			var expectedId = artifact.Variant is null
				? $"{artifact.Layer}.{artifact.Type}"
				: $"{artifact.Layer}.{artifact.Type}.{artifact.Variant}";

			if (!string.Equals(artifact.Artifact.Id, expectedId, StringComparison.OrdinalIgnoreCase))
			{
				errors.Add($"artifact.id '{artifact.Artifact.Id}' does not match '{expectedId}'");
			}

			if (artifact.Variant is not null && string.IsNullOrWhiteSpace(artifact.Type))
			{
				errors.Add("variant cannot exist without type");
			}

			return errors;
		}
	}
}