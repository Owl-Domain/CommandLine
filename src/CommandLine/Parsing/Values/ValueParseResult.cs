namespace OwlDomain.CommandLine.Parsing.Values;

/// <summary>
/// 	Represents the result of a value parsing operation.
/// </summary>
/// <typeparam name="T">The type of the parsed value.</typeparam>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public class ValueParseResult<T> : IValueParseResult<T>
{
	#region Properties
	/// <inheritdoc/>
	public IValueParseContext Context { get; }

	/// <inheritdoc/>
	public string? Error { get; }

	/// <inheritdoc/>
	public T? Value { get; }

	/// <inheritdoc/>
	public TextLocation Location { get; }

	/// <inheritdoc/>
	public bool Successful { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of a successful <see cref="ValueParseResult{T}"/>.</summary>
	/// <param name="context">The context for the <paramref name="value"/> that was parsed.</param>
	/// <param name="location">The location of the parsed <paramref name="value"/>.</param>
	/// <param name="value">The value that was parsed.</param>
	public ValueParseResult(IValueParseContext context, TextLocation location, T? value)
	{
		Successful = true;
		Context = context;
		Location = location;
		Value = value;
	}

	/// <summary>Creates a new instance of the <see cref="ValueParseResult{T}"/>.</summary>
	/// <param name="context">The context that was used during the parsing attempt.</param>
	/// <param name="location">The location that the <paramref name="error"/> came from.</param>
	/// <param name="error">The error that occurred during the parse attempt.</param>
	public ValueParseResult(IValueParseContext context, TextLocation location, string error)
	{
		Successful = false;
		Context = context;
		Location = location;
		Error = error;
	}
	#endregion

	#region Methods
	/// <inheritdoc/>
	public virtual IEnumerable<TextToken> EnumerateTokens()
	{
		if (Error is not null)
			return [];

		TextToken token = new(TextTokenKind.Value, Location, Value);

		return [token];
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		string typeName = nameof(ValueParseResult<T>);

		if (Error is not null)
			return $"{typeName} {{ {nameof(Error)} = ({Error}) }}";

		return $"{typeName} {{ {nameof(Value)} = ({Value}) }}";
	}
	#endregion
}
