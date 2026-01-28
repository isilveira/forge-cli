namespace Forge.CLI.Core._Legacy.Planning
{
	public sealed class ScaffoldPlan
	{
		public IReadOnlyCollection<ScaffoldTask> Tasks { get; init; } = [];
	}
}
