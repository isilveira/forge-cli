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
			_engine = new RazorLightEngineBuilder()
				.UseEmbeddedResourcesProject(
					Assembly.GetExecutingAssembly())
				.UseMemoryCachingProvider()
				.Build();
		}

        public string Render(
			TemplateDefinition template, TemplateModel model)
        {
            return RenderAsync(template, model).GetAwaiter().GetResult();
		}

        public async Task<string> RenderAsync(
			TemplateDefinition template, TemplateModel model)
		{
			try
			{
				return await _engine.CompileRenderStringAsync(
					template.Key,
					template.Content,
					model,
					null);
			}
			catch (TemplateCompilationException ex)
			{
				throw new InvalidOperationException(
					$"Erro ao compilar template Razor '{template.Key}'.",
					ex);
			}
		}
	}
}
