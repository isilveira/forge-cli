using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.CLI.Core.Artifacts.Interfaces
{
    public interface IArtifactLoader
    {
        ArtifactDefinition Load(string content);
	}
}
