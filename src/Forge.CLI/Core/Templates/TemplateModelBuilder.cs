using Forge.CLI.Core.Artifacts;
using Forge.CLI.Models;

namespace Forge.CLI.Core.Templates
{
	public sealed class TemplateModelBuilder
	{
		private readonly ForgeProject _project;

		public TemplateModelBuilder(ForgeProject project)
		{
			_project = project;
		}

		public TemplateModel Build(ArtifactDescriptor descriptor)
		{
			var context = descriptor.Target.ContextName is not null
				? _project.Contexts[descriptor.Target.ContextName]
				: null;

			var entity = descriptor.Target.EntityName is not null
				? context?.Entities[descriptor.Target.EntityName]
				: null;

			return new TemplateModel
			{
				Project = _project,
				ContextName = descriptor.Target?.ContextName,
				Context = context!,
				EntityName = descriptor.Target?.EntityName,
				Entity = entity!,
				Descriptor = descriptor
			};
		}
	}

}
