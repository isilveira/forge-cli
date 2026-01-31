namespace Forge.CLI.Core.CodeScanning.Parsing
{
	public sealed class ForgeMarkerParseException : Exception
	{
		public string FilePath { get; }
		public int LineNumber { get; }

		public ForgeMarkerParseException(
			string filePath,
			int lineNumber,
			string message)
			: base($"{filePath}:{lineNumber} → {message}")
		{
			FilePath = filePath;
			LineNumber = lineNumber;
		}
	}
}