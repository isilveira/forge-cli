using System.Runtime.CompilerServices;
using Forge.CLI.Core.Templates;
using Forge.CLI.Models;
using Forge.CLI.Shared.Extensions;

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

        public static string Tab(int count, string tabPattern = "    ")
        {
            var tab = string.Empty;
            for (var i = 0; i < count; i++)
            {
                tab += tabPattern;
            }
            return tab;
        }

        public static List<string> GetEntityTrails(ForgeProject project, string contextName, ForgeContext context, string entityName, ForgeEntity entity, TrailType type = TrailType.Index, string partialPath = "")
        {
            var trails = new List<string>();
            var queryRelationsManyToOneRequired = entity.Relations.Where(r => r.Value.Type == "many-to-one" && r.Value.Required).AsQueryable();

            if (string.IsNullOrWhiteSpace(partialPath))
            {
                partialPath = $"{TrailPart(entityName, type, project.IdType)}";
            }

            if (entity.AggregateRoot)
            {
                var entityCollection = Pluralize(entityName);
                trails.Add($"/{contextName}{(string.IsNullOrWhiteSpace(partialPath) ? "": $"{partialPath}")}");
            }
            else if(queryRelationsManyToOneRequired.Any())
            {
                foreach(var relation in queryRelationsManyToOneRequired.ToList())
                {
                    var trail = $"{TrailPart(relation.Key, TrailType.Edit, project.IdType)}{(string.IsNullOrWhiteSpace(partialPath) ? "" : $"{partialPath}")}";
                    trails.AddRange(GetEntityTrails(project, contextName, context, relation.Key, context.Entities.FirstOrDefault(e => e.Key == relation.Value.Target).Value, type, trail));
                }
            }

            return trails;
        }
        public enum TrailType
        {
            Index, Create, Edit
        }
        private static string TrailPart(string trailName, TrailType type = TrailType.Index, string idType = "int")
        {
            return $"/{Pluralize(trailName)}{(type == TrailType.Create ? "/Create" : type == TrailType.Edit ? $"/{{{trailName}Id:{idType}}}" : "")}";
        }
	}
}
