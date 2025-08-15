using System.IO;

namespace OwlDomain.CommandLine.Parsing.Values.Paths;

/// <summary>
/// 	Represents the value parser selector for path-like types.
/// </summary>
public sealed class PathValueParserSelector : BaseValueParserSelector
{
	#region Methods
	/// <inheritdoc/>
	protected override IValueParser? TrySelect(IRootValueParserSelector rootSelector, Type type)
	{
		if (type == typeof(Uri))
			return new UriValueParser();

		if (type == typeof(FileInfo))
			return new FileInfoValueParser();

		if (type == typeof(DirectoryInfo))
			return new DirectoryInfoValueParser();

		return default;
	}
	#endregion
}
