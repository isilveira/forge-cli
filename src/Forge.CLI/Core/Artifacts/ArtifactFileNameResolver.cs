namespace Forge.CLI.Core.Artifacts
{
	public static class ArtifactFileNameResolver
	{
		public static bool TryResolve(
			string fileName,
			out string type,
			out string? variant)
		{
			type = string.Empty;
			variant = null;

			var name = Path.GetFileNameWithoutExtension(fileName);
			var parts = name.Split('.', StringSplitOptions.RemoveEmptyEntries);

			if (parts.Length == 1)
			{
				type = parts[0];
				return true;
			}

			if (parts.Length == 2)
			{
				type = parts[0];
				variant = parts[1];
				return true;
			}

			return false;
		}
	}
}
