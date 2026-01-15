namespace Forge.CLI.Core.Planning
{
	public sealed class ScaffoldPlan
	{
		public IReadOnlyCollection<ScaffoldTask> Tasks { get; init; } = [];
	}
}
