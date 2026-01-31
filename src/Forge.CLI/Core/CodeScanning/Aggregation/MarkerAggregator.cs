using Forge.CLI.Core.CodeScanning.Markers;

namespace Forge.CLI.Core.CodeScanning.Aggregation
{
	public sealed class MarkerAggregator
	{
		public ScannedProjectModel Aggregate(IEnumerable<object> markers)
		{
			var model = new ScannedProjectModel();

			foreach (var m in markers)
			{
				switch (m)
				{
					case ForgeEntityMarker e:
						model.Entities.Add(e);
						break;

					case ForgePropertyMarker p:
						model.Properties.Add(p);
						break;

					case ForgeRelationshipMarker r:
						model.Relationships.Add(r);
						break;
				}
			}

			return model;
		}
	}
}
