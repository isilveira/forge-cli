namespace Forge.CLI.Core.Execution
{
	public sealed class ExecutionOptions
	{
		public bool WhatIf { get; init; }
		public bool Force { get; init; }
		public bool ConfirmEach { get; init; } = true;
	}
}
