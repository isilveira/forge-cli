using Forge.CLI.Core.Scaffolding.Planning;

namespace Forge.CLI.Core.Scaffolding.Conflict
{
	public class ConflictDetector
	{
		public ScaffoldActionType Detect(
			string filePath,
			OverwriteStrategy strategy,
			bool hasForgeMarkers)
		{
			if (!File.Exists(filePath))
				return ScaffoldActionType.Create;

			return strategy switch
			{
				OverwriteStrategy.Always => ScaffoldActionType.Update,
				OverwriteStrategy.Never => ScaffoldActionType.Skip,
				OverwriteStrategy.MarkersOnly when hasForgeMarkers => ScaffoldActionType.Update,
				_ => ScaffoldActionType.Conflict
			};
		}
	}

}
