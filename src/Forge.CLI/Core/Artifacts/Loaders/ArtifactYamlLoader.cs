using Forge.CLI.Core.Artifacts.Interfaces;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Forge.CLI.Core.Artifacts.Loaders
{
	public sealed class ArtifactYamlLoader : IArtifactLoader
	{
		private readonly IDeserializer _deserializer;

		public ArtifactYamlLoader()
		{
			_deserializer = new DeserializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.IgnoreUnmatchedProperties()
				.Build();
		}

		public ArtifactDefinition Load(string yamlContent)
		{
			return _deserializer.Deserialize<ArtifactDefinition>(yamlContent);
		}
	}
}
