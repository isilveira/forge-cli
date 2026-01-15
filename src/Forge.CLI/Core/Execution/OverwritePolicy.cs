using Forge.CLI.Shared.Helpers;

namespace Forge.CLI.Core.Execution
{
	public sealed class OverwritePolicy
	{
		private readonly ExecutionOptions _options;
		public OverwritePolicy(ExecutionOptions options)
		{
			_options = options;
		}
		public bool ShouldOverwrite(string path)
		{
			if (_options.Force)
				return true;

			if (!_options.ConfirmEach)
				return false;

			return AnsiConsoleHelper.SafeConfirm(
				$"Arquivo '{path}' já existe. Sobrescrever?");
		}
	}
}
