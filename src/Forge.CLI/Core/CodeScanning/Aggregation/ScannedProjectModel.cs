using Forge.CLI.Core.CodeScanning.Markers;

namespace Forge.CLI.Core.CodeScanning.Aggregation
{
	public sealed class ScannedProjectModel
	{
		public List<ForgeEntityMarker> Entities { get; } = [];
		public List<ForgePropertyMarker> Properties { get; } = [];
		public List<ForgeRelationshipMarker> Relationships { get; } = [];
	}
}
