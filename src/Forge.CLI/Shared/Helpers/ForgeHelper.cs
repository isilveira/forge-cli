using BAYSOFT.Abstractions.Crosscutting.Extensions;
using Forge.CLI.Core.Scaffolding.Planning;
using Forge.CLI.Core.Templates;
using Forge.CLI.Models;
using Forge.CLI.Shared.Extensions;
using Scriban;
using Scriban.Runtime;
using static BAYSOFT.Abstractions.Crosscutting.Extensions.StringExtensions;

namespace Forge.CLI.Shared.Helpers
{
    public static class ForgeHelper
    {
        public static string ResolveResourceKey(string resourceKey)
            => resourceKey.Replace(".", "_").Replace("-", "_");
        public static string Pluralize(string word)
            => word.PluralizeAsPascal();
        public static string Singularize(string word)
            => word.SingularizeAsPascal();
        public static string ToCase(string word, Case toCase)
            => word.ToCase(toCase);
        public static string ToKebabCase(string word)
            => word.ToKebabCase();
        public enum ConventionType
        {
            Path, Namespace
        }
        public static string GetProjectConvention(ForgeProject project, ConventionType conventionType = ConventionType.Namespace)
        {
            var convention = conventionType == ConventionType.Namespace ? project.DefaultConventions.DefaultProject : project.DefaultConventions.DefaultProjectPath;
            return convention.Replace("{projectName}", project.Name);
        }
        public static string GetContextConvention(ForgeProject project, string contextName, ConventionType conventionType = ConventionType.Namespace)
        {
            var convention = conventionType == ConventionType.Namespace ? project.DefaultConventions.DefaultContext : project.DefaultConventions.DefaultContextPath;
            return convention.Replace("{contextName}", contextName);
        }
        public static string GetEntityConvention(ForgeProject project, string entityName, ConventionType conventionType = ConventionType.Namespace)
        {
            var convention = conventionType == ConventionType.Namespace ? project.DefaultConventions.DefaultEntity : project.DefaultConventions.DefaultEntityPath;
            return convention.Replace("{entityName}", project.DefaultConventions.DefaultEntityPluralized ? Pluralize(entityName) : entityName);
        }
        public static string Tab(int count, string tabPattern = "    ")
        {
            var tab = string.Empty;
            for (var i = 0; i < count; i++)
            {
                tab += tabPattern;
            }
            return tab;
		}
		public static string UsingInCSharp(string reference, bool isStatic = false, bool isCommented = false)
		{
			return $"{(isCommented ? "//":"")}using {(isStatic? "static ":"")}{reference};";
		}
		public static string UsingInBlazor(string reference, bool isStatic = false, bool isCommented = false)
		{
            var result = $"@using {(isStatic ? "static " : "")}{reference}";

            if (isCommented)
            {
                result = $"@* {result} *@";
			}

            return result;
		}
		public static List<string> GetEntityTrails(ForgeProject project, string contextName, ForgeContext context, string entityName, ForgeEntity entity, TrailType type = TrailType.Index, Case urlCase = Case.Kebab, Case paramCase = Case.Camel, string partialPath = "")
        {
            var trails = new List<string>();
            var queryRelationsManyToOneRequired = entity.Relations.Where(r => r.Value.Type == "many-to-one" && r.Value.Required).AsQueryable();

            if (string.IsNullOrWhiteSpace(partialPath))
            {
                partialPath = $"{TrailPart(entityName, type, urlCase, paramCase, entity.IdType)}";
            }

            if (entity.AggregateRoot)
            {
                trails.Add($"/{contextName.ToKebabCase()}{(string.IsNullOrWhiteSpace(partialPath) ? "" : $"{partialPath}")}");
            }
            else if (queryRelationsManyToOneRequired.Any())
            {
                foreach (var relation in queryRelationsManyToOneRequired.ToList())
                {
                    var trail = $"{TrailPart(relation.Key, TrailType.Edit, urlCase, paramCase, relation.Value.GetTargetIdType())}{(string.IsNullOrWhiteSpace(partialPath) ? "" : $"{partialPath}")}";
                    trails.AddRange(GetEntityTrails(project, contextName, context, relation.Key, context.Entities.FirstOrDefault(e => e.Key == relation.Value.Target).Value, type, urlCase, paramCase, trail));
                }
            }

            return trails;
        }
        public enum TrailType
        {
            Index, Create, Edit
        }
        private static string TrailPart(string trailName, TrailType type = TrailType.Index, Case urlCase = Case.Kebab, Case paramCase = Case.Camel, string idType = "int")
        {
            return $"/{Pluralize(trailName).ToCase(urlCase)}{(type == TrailType.Create ? $"/{"Create".ToCase(urlCase)}" : type == TrailType.Edit ? $"/{{{trailName.ToCase(paramCase)}Id:{idType.ToLower()}}}" : "")}";
        }
        public static List<(string, List<(string, string)>)> GetTrailsIds(List<string> trails)
        {
            var trailsIds = new List<(string, List<(string, string)>)>();
            foreach (var trail in trails)
            {
                trailsIds.Add((GetTrailWithoutTypes(trail), GetTrailIds(trail)));
            }
            return trailsIds.OrderByDescending(trailIds => trailIds.Item2.Count).ToList();
        }
        private static string GetTrailWithoutTypes(string trail)
        {
            var segments = trail.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var cleanSegments = new List<string>();
            foreach (var segment in segments)
            {
                if (segment.StartsWith("{") && segment.EndsWith("}"))
                {
                    var idSegment = segment.TrimStart('{').TrimEnd('}');
                    var parts = idSegment.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 1)
                    {
                        cleanSegments.Add($"{{{parts[0]}}}");
                    }
                }
                else
                {
                    cleanSegments.Add(segment);
                }
            }
            return "/" + string.Join('/', cleanSegments);
        }
        private static List<(string, string)> GetTrailIds(string trail)
        {
            var ids = new List<(string, string)>();
            var segments = trail.Split('/', StringSplitOptions.RemoveEmptyEntries);
            foreach (var segment in segments)
            {
                if (segment.StartsWith("{") && segment.EndsWith("}"))
                {
                    var idSegment = segment.TrimStart('{').TrimEnd('}');
                    var parts = idSegment.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        ids.Add((parts[0], TypeMapperHelper.Map(parts[1])));
                    }
                }
            }
            return ids;
        }
        public sealed record Target(string? ContextName, string? EntityName);
        public static ScriptObject BuildVariables(ForgeProject _project, string targetContextName, string targetEntityName, string name, ConventionType conventionType = ConventionType.Namespace)
        {
            var variables = new ScriptObject();

            variables["projectName"] = _project.Name;
            variables["name"] = name;
            variables["contextName"] = targetContextName;
            variables["entityName"] = targetEntityName;
            variables["entityCollection"] = Pluralize(targetEntityName);
            variables["projectConvention"] = GetProjectConvention(_project, conventionType);
            variables["contextConvention"] = GetContextConvention(_project, targetContextName, conventionType);
            variables["entityConvention"] = GetEntityConvention(_project, targetEntityName, conventionType);

            return variables;
        }
        public static string RenderScriban(string template, ScriptObject variables)
        {
            var parsed = Template.Parse(template);
            if (parsed.HasErrors)
            {
                var errors = string.Join("; ", parsed.Messages.Select(m => m.Message));
                throw new InvalidOperationException($"Invalid Scriban template: {errors}");
            }

            var ctx = new TemplateContext();
            ctx.PushGlobal(variables);
            return parsed.Render(ctx);
        }
        public static string RenderArtifactNamespace(string id, TemplateModel templateModel, string? targetContext = null, string? targetEntity = null)
        {
            if (templateModel.ArtifactsNamespacePattern.ContainsKey(id) && !string.IsNullOrWhiteSpace(templateModel.ArtifactsNamespacePattern[id]))
            {
                var template = templateModel.ArtifactsNamespacePattern[id];
                return RenderScriban(template, BuildVariables(templateModel.Project, !string.IsNullOrWhiteSpace(targetContext) ? targetContext : templateModel.ContextName, !string.IsNullOrWhiteSpace(targetEntity) ? targetEntity : templateModel.EntityName, templateModel.Name));
            }

            return $"// No namespace pattern found for the artifact '{id}'!";
        }
    }
}
