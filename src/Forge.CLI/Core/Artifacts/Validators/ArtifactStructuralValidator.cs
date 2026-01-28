using FluentValidation;

namespace Forge.CLI.Core.Artifacts.Validators
{
	public sealed class ArtifactStructuralValidator : AbstractValidator<ArtifactDefinition>
	{
		private static readonly string[] AllowedModelSources =
			{ "metadata", "code", "hybrid" };

		private static readonly string[] AllowedTemplateEngines =
			{ "razor" };

		public ArtifactStructuralValidator()
		{
			RuleFor(x => x.Artifact).NotNull();
			RuleFor(x => x.Artifact.Id).NotEmpty();
			RuleFor(x => x.Artifact.Version).GreaterThan(0);

			RuleFor(x => x.Layer).NotEmpty();
			RuleFor(x => x.Type).NotEmpty();

			RuleFor(x => x.Model.Source)
				.Must(s => AllowedModelSources.Contains(s))
				.WithMessage($"model.source must be one of: {string.Join(", ", AllowedModelSources)}");

			RuleFor(x => x.Generation).NotNull();
			RuleFor(x => x.Generation.Enabled).NotNull();

			When(x => x.Generation.Enabled, () =>
			{
				RuleFor(x => x.Generation.Target).NotNull();
				RuleFor(x => x.Generation.Template).NotNull();

				RuleFor(x => x.Generation.Target.Path).NotEmpty();
				RuleFor(x => x.Generation.Target.Filename).NotEmpty();

				RuleFor(x => x.Generation.Template.File).NotEmpty();
				RuleFor(x => x.Generation.Template.Engine)
					.Must(e => AllowedTemplateEngines.Contains(e))
					.WithMessage($"template.engine must be one of: {string.Join(", ", AllowedTemplateEngines)}");
			});
		}
	}
}
