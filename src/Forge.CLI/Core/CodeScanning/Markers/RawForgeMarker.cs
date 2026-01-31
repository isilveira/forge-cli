namespace Forge.CLI.Core.CodeScanning.Markers
{
	public sealed class RawForgeMarker
	{
		public string FilePath { get; }
		public int LineNumber { get; }
		public string RawText { get; }

		public RawForgeMarker(string filePath, int lineNumber, string rawText)
		{
			FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
			RawText = rawText ?? throw new ArgumentNullException(nameof(rawText));
			LineNumber = lineNumber;
		}
	}
}