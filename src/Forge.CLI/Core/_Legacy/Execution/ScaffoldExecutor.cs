using Forge.CLI.Shared.Helpers;

namespace Forge.CLI.Core._Legacy.Execution
{
	public sealed class ScaffoldExecutor
	{
		private readonly IFileSystem _fs;
		private readonly PathResolver _paths;
		private readonly OverwritePolicy _overwrite;

		public ScaffoldExecutor(
			IFileSystem fs,
			PathResolver paths,
			OverwritePolicy overwrite)
		{
			_fs = fs;
			_paths = paths;
			_overwrite = overwrite;
		}

		public void Execute(
			IReadOnlyCollection<RenderedArtifact> artifacts,
			ExecutionOptions options)
		{
			foreach (var artifact in artifacts)
			{
				ExecuteOne(artifact, options);
			}
		}

		private void ExecuteOne(
			RenderedArtifact artifact,
			ExecutionOptions options)
		{
			var path = _paths.Resolve(artifact.Descriptor);
			var directory = Path.GetDirectoryName(path)!;

			AnsiConsoleHelper.SafeMarkupLine(
				$"→ {path}", "grey");

			if (options.WhatIf)
				return;

			_fs.CreateDirectory(directory);

			if (_fs.FileExists(path))
			{
				if (!_overwrite.ShouldOverwrite(path))
				{
					AnsiConsoleHelper.SafeMarkupLine(
						$"Ignorado: {path}", "yellow");
					return;
				}
			}

			_fs.WriteFile(path, artifact.Content);

			AnsiConsoleHelper.SafeMarkupLine(
				$"Criado: {path}", "green");
		}
	}
}
