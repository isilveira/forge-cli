using RazorLight.Razor;
using System.Reflection;

namespace Forge.CLI.Core.Templates.Renderers.Razor
{
	/// <summary>
	/// Loads Razor templates from embedded resources under `Scaffolding/Templates`.
	/// </summary>
	public sealed class ForgeRazorProject : RazorLightProject
	{
		private readonly Assembly _assembly;
		private readonly string _rootNamespace;

		public ForgeRazorProject(Assembly assembly, string rootNamespace)
		{
			_assembly = assembly;
			_rootNamespace = rootNamespace;
		}

		public override Task<RazorLightProjectItem> GetItemAsync(string templateKey)
		{
			// templateKey: "Domain/Entity"
			var resourceKey =
				"Scaffolding.Templates." +
				templateKey.Replace("/", ".") +
				".cshtml";

			return Task.FromResult<RazorLightProjectItem>(
				new EmbeddedRazorProjectItem(_assembly, _rootNamespace, resourceKey));
		}

		public override Task<IEnumerable<RazorLightProjectItem>> GetImportsAsync(string templateKey)
		{
			return Task.FromResult(
				Enumerable.Empty<RazorLightProjectItem>());
		}
	}
}

