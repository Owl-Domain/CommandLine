namespace OwlDomain.CommandLine.Parsing.Values;

/// <summary>
/// 	Represents the result of a value parsing operation.
/// </summary>
public interface IValueParseResult
{
	#region Properties
	/// <summary>The context for the value that was parsed.</summary>
	IValueParseContext Context { get; }

	/// <summary>The error that caused the parsing to fail.</summary>
	string? Error { get; }

	/// <summary>The parsed value.</summary>
	object? Value { get; }

	/// <summary>The location of the parsed value, or the location relevant to the error.</summary>
	TextLocation Location { get; }
	#endregion

	#region Methods
	/// <summary>Enumerates all of the parsed text tokens.</summary>
	/// <returns>An enumerable of all of the parsed text tokens.</returns>
	IEnumerable<TextToken> EnumerateTokens();
	#endregion
}

/// <summary>
/// 	Represents the result of a value parsing operation.
/// </summary>
/// <typeparam name="T">The type of the parsed value.</typeparam>
public interface IValueParseResult<out T> : IValueParseResult
{
	#region Properties
	/// <summary>The parsed value.</summary>
	new T? Value { get; }
	object? IValueParseResult.Value => Value;
	#endregion
}
