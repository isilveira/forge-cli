namespace Forge.CLI.Core.Execution
{
	public interface IFileSystem
	{
		bool FileExists(string path);
		void CreateDirectory(string path);
		void WriteFile(string path, string content);
	}
}
