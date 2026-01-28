namespace Forge.CLI.Core._Legacy.Execution
{
	public interface IFileSystem
	{
		bool FileExists(string path);
		void CreateDirectory(string path);
		void WriteFile(string path, string content);
	}
}
