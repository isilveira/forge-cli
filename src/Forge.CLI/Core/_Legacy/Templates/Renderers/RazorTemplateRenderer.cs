using Forge.CLI.Core._Legacy.Templates;
using Forge.CLI.Core.Templates.Renderers.Razor;
using RazorLight;
using RazorLight.Compilation;
using System.Reflection;

namespace Forge.CLI.Core._Legacy.Templates.Renderers
{
	public sealed class RazorTemplateRenderer : ITemplateRenderer
	{
		private readonly RazorLightEngine _engine;

		public RazorTemplateRenderer()
		{
			//_engine = new RazorLightEngineBuilder()
			//	.UseEmbeddedResourcesProject(
			//		Assembly.GetExecutingAssembly())
			//	.UseMemoryCachingProvider()
			//	.Build();

			var assembly = Assembly.GetExecutingAssembly();
			var rootNamespace = assembly.GetName().Name!;

			var project = new ForgeRazorProject(assembly, rootNamespace);

			_engine = new RazorLightEngineBuilder()
				.UseProject(project)
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
				if (template.HasContent)
				{
					return await _engine.CompileRenderStringAsync(
						template.Key,
						template.Content,
						model,
						null);
				}
				else
				{
					return await _engine.CompileRenderAsync(
						template.Key,
						model);
				}
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
