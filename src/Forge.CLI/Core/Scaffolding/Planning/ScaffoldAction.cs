namespace Forge.CLI.Core.Scaffolding.Planning
{
	public class ScaffoldAction
	{
		public ScaffoldActionType ActionType { get; }
		public string FilePath { get; }
		public string? Content { get; }
		public string? Reason { get; }

		public ScaffoldAction(
			ScaffoldActionType actionType,
			string filePath,
			string? content = null,
			string? reason = null)
		{
			ActionType = actionType;
			FilePath = filePath;
			Content = content;
			Reason = reason;
		}
	}
}