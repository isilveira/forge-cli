using Forge.CLI.Core.Templates.Renderers.Razor;
using RazorLight;
using RazorLight.Compilation;
using System.Reflection;

namespace Forge.CLI.Core.Templates.Renderers
{
	public sealed class RazorTemplateRenderer : ITemplateRenderer
	{
		private readonly RazorLightEngine _engine;

		public RazorTemplateRenderer()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var rootNamespace = assembly.GetName().Name!;

			var project = new ForgeRazorProject(assembly, rootNamespace);

			_engine = new RazorLightEngineBuilder()
				.UseProject(project)
				.UseMemoryCachingProvider()
				.Build();
		}

		public async Task<string> RenderAsync(string templateKey, object model)
		{
			try
			{
				return await _engine.CompileRenderAsync(templateKey, model);
			}
			catch (TemplateCompilationException ex)
			{
				throw new InvalidOperationException(
					$"Erro ao compilar template Razor '{templateKey}'.",
					ex);
			}
		}
	}
}

