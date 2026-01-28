namespace Forge.CLI.Core.Scaffolding.Execution
{
	public sealed class ScaffoldExecutionOptions
	{
		public bool ConfirmEach { get; init; }

		/// <summary>
		/// If true, conflicts make the command exit with non-zero (even if files are not written).
		/// </summary>
		public bool TreatConflictsAsError { get; init; } = true;
	}
}

