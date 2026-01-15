using Forge.CLI.Core.Artifacts;

namespace Forge.CLI.Core.Execution
{
	public sealed class PathResolver
	{
		private readonly string _root;
		public PathResolver(string root)
		{
			_root = root;
		}
		public string Resolve(ArtifactDescriptor descriptor)
		{
			return Path.Combine(
				_root,
				descriptor.RelativePath,
				descriptor.FileName);
		}
	}
}
