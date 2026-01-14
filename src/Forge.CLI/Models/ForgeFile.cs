namespace Forge.CLI.Models
{
    public class ForgeFile
    {
        public string Path { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
        public ForgeFile(string path, string name, string content)
        {
            Path = path;
            Name = name;
			Content = content;
		}
    }
}
