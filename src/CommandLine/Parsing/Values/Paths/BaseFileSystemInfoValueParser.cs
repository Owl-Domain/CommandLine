using System.IO;

namespace OwlDomain.CommandLine.Parsing.Values.Paths;

/// <summary>
/// 	Represents the base value parser for <see cref="FileSystemInfo"/> types.
/// </summary>
/// <typeparam name="T">The type to parse.</typeparam>
public abstract class BaseFileSystemInfoValueParser<T> : BaseValueParser<T>
	where T : FileSystemInfo
{
	#region Methods
	/// <inheritdoc/>
	protected override T? TryParse(IValueParseContext context, ITextParser parser, out string? error)
	{
		string text = parser.AdvanceUntilBreak();

		try
		{
			T? value = TryCreate(text, out error);
			return value;
		}
		catch (Exception exception)
		{
			error = exception.Message;
			return default;
		}
	}

	/// <summary>Tries to create a instance of the type <typeparamref name="T"/> from the given <paramref name="text"/>.</summary>
	/// <param name="text">The text that was parsed.</param>
	/// <param name="error">The error that occured during parsing.</param>
	/// <returns>
	/// 	The created value, or <see langword="null"/> if the value couldn't be created,
	/// 	in which case the <paramref name="error"/> value should be set.
	/// </returns>
	/// <remarks>Exceptions will be caught automatically and they do not need to be handled by this method.</remarks>
	protected abstract T? TryCreate(string text, out string? error);
	#endregion
}
