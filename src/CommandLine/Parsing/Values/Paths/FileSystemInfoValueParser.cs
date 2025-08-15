using System.IO;

namespace OwlDomain.CommandLine.Parsing.Values.Paths;

/// <summary>
/// 	Represents a value parser for the <see cref="FileSystemInfo"/> type.
/// </summary>
/// <remarks>
/// 	This parser requires the path to exist, and it'll either select a <see cref="FileInfo"/>
/// 	value or a <see cref="DirectoryInfo"/> depending on what the path is pointing to.
/// </remarks>
public sealed class FileSystemInfoValueParser : BaseFileSystemInfoValueParser<FileSystemInfo>
{
	#region Methods
	/// <inheritdoc/>
	protected override FileSystemInfo? TryCreate(string text, out string? error)
	{
		error = default;

		if (text is "." or "..")
			return new DirectoryInfo(text);

		FileInfo? file = TryGetFile(text, out string? fileError);
		if (file is not null)
			return file;

		DirectoryInfo? directory = TryGetDirectory(text, out string? directoryError);
		if (directory is not null)
			return directory;

		error = fileError ?? directoryError ?? $"The given path '{text}' did not exist.";
		return default;
	}
	private static FileInfo? TryGetFile(string text, out string? error)
	{
		try
		{
			error = default;

			FileInfo file = new(text);

			if (file.Exists)
				return file;

			return default;
		}
		catch (Exception exception)
		{
			error = exception.Message;
			return default;
		}
	}
	private static DirectoryInfo? TryGetDirectory(string text, out string? error)
	{
		try
		{
			error = default;

			DirectoryInfo directory = new(text);

			if (directory.Exists)
				return directory;

			return default;
		}
		catch (Exception exception)
		{
			error = exception.Message;
			return default;
		}
	}
	#endregion
}
