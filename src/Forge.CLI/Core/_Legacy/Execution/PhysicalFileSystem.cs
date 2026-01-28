using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.CLI.Core._Legacy.Execution
{
	public sealed class PhysicalFileSystem : IFileSystem
	{
		public bool FileExists(string path)
			=> File.Exists(path);

		public void CreateDirectory(string path)
			=> Directory.CreateDirectory(path);

		public void WriteFile(string path, string content)
			=> File.WriteAllText(path, content);
	}
}
