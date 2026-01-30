namespace Forge.CLI.Core.Scaffolding.Planning
{
	public class ScaffoldPlan
	{
		private readonly List<ScaffoldAction> _actions = new();

		public IReadOnlyCollection<ScaffoldAction> Actions => _actions;

		public void Add(ScaffoldAction action)
			=> _actions.Add(action);

		public bool HasConflicts()
			=> _actions.Any(a => a.ActionType == ScaffoldActionType.Conflict);
	}
}