using Forge.CLI.Core.Capabilities;
using Forge.CLI.Core.Planning;
using Forge.CLI.Core.Target;

namespace Forge.CLI.Core.Artifacts
{
	public sealed class ArtifactDescriptorResolver
	{
		public ArtifactDescriptor Resolve(ScaffoldTask task)
		{
			var scope = ResolveScope(task.Type);

			var path = ResolvePath(task.Layer, task.Target);
			var fileName = ResolveFileName(task);
			var templateKey = ResolveTemplateKey(task);

			return new ArtifactDescriptor
			{
				Layer = task.Layer,
				Type = task.Type,
				Variant = task.Variant,
				Scope = scope,

				RelativePath = path,
				FileName = fileName,
				TemplateKey = templateKey,

				Target = task.Target
			};
		}
		private static TargetScope ResolveScope(ArtifactType type)
		{
			return type switch
			{
				ArtifactType.Resource => TargetScope.Project,
				ArtifactType.DbContext => TargetScope.Context,
				ArtifactType.ContextResource => TargetScope.Context,
				_ => TargetScope.Entity
			};
		}
		private static string ResolvePath(
			Layer layer,
			ScaffoldTarget target)
		{
			return target.Scope switch
			{
				TargetScope.Entity =>
					$"{layer}/{target.ContextName}/Entities/{target.EntityName}",

				TargetScope.Context =>
					$"{layer}/{target.ContextName}",

				TargetScope.Project =>
					$"{layer}",

				_ => throw new InvalidOperationException()
			};
		}
		private static string ResolveFileName(
			ScaffoldTask task)
		{
			var entity = task.Target.EntityName;

			if (task.Variant is Variant.None)
				return $"{entity}{task.Type}.cs";

			return $"{entity}{task.Type}{task.Variant}.cs";
		}
		private static string ResolveTemplateKey(ScaffoldTask task)
		{
			if (task.Variant is Variant.None)
				return $"{task.Layer}.{task.Type}";

			return $"{task.Layer}.{task.Type}.{task.Variant}";
		}
	}
}
