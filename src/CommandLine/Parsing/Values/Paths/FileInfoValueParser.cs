using System.IO;

namespace OwlDomain.CommandLine.Parsing.Values.Paths;

/// <summary>
/// 	Represents a value parser for the <see cref="FileInfo"/> type.
/// </summary>
public sealed class FileInfoValueParser : BaseFileSystemInfoValueParser<FileInfo>
{
	#region Methods
	/// <inheritdoc/>
	protected override FileInfo? TryCreate(string text, out string? error)
	{
		error = default;
		return new(text);
	}
	#endregion
}
