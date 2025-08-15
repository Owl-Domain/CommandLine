using System.IO;

namespace OwlDomain.CommandLine.Parsing.Values.Paths;

/// <summary>
/// 	Represents a value parser for the <see cref="DirectoryInfo"/> type.
/// </summary>
public sealed class DirectoryInfoValueParser : BaseFileSystemInfoValueParser<DirectoryInfo>
{
	#region Methods
	/// <inheritdoc/>
	protected override DirectoryInfo? TryCreate(string text, out string? error)
	{
		error = default;
		return new(text);
	}
	#endregion
}
