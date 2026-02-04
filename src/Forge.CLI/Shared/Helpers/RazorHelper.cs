using BAYSOFT.Abstractions.Crosscutting.Extensions;
using Forge.CLI.Models;
using Forge.CLI.Shared.Extensions;
using static BAYSOFT.Abstractions.Crosscutting.Extensions.StringExtensions;

namespace Forge.CLI.Shared.Helpers
{
    public static class RazorHelper
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
		public static string Tab(int count, string tabPattern = "    ")
        {
            var tab = string.Empty;
            for (var i = 0; i < count; i++)
            {
                tab += tabPattern;
            }
            return tab;
        }
        public static List<string> GetEntityTrails(ForgeProject project, string contextName, ForgeContext context, string entityName, ForgeEntity entity, TrailType type = TrailType.Index,Case urlCase = Case.Kebab, Case paramCase = Case.Camel, string partialPath = "")
        {
            var trails = new List<string>();
            var queryRelationsManyToOneRequired = entity.Relations.Where(r => r.Value.Type == "many-to-one" && r.Value.Required).AsQueryable();

            if (string.IsNullOrWhiteSpace(partialPath))
            {
                partialPath = $"{TrailPart(entityName, type, urlCase, paramCase, project.IdType)}";
            }

            if (entity.AggregateRoot)
            {
                trails.Add($"/{contextName.ToKebabCase()}{(string.IsNullOrWhiteSpace(partialPath) ? "": $"{partialPath}")}");
            }
            else if(queryRelationsManyToOneRequired.Any())
            {
                foreach(var relation in queryRelationsManyToOneRequired.ToList())
                {
                    var trail = $"{TrailPart(relation.Key, TrailType.Edit, urlCase, paramCase, project.IdType)}{(string.IsNullOrWhiteSpace(partialPath) ? "" : $"{partialPath}")}";
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
        public static List<(string, List<(string,string)>)> GetTrailsIds(List<string> trails)
        {
            var trailsIds = new List<(string, List<(string, string)>)>();
            foreach(var trail in trails)
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
            foreach(var segment in segments)
            {
                if(segment.StartsWith("{") && segment.EndsWith("}"))
                {
                    var idSegment = segment.TrimStart('{').TrimEnd('}');
                    var parts = idSegment.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    if(parts.Length == 2)
                    {
                        ids.Add((parts[0], parts[1]));
                    }
                }
            }
            return ids;
		}
	}
}
