namespace OwlDomain.CommandLine.Parsing.Values.Collections;

/// <summary>
/// 	Represents the result of a collection parsing operation.
/// </summary>
/// <typeparam name="TCollection">The type of the collection that was parsed.</typeparam>
/// <typeparam name="TValue">The type of the values in the collection.</typeparam>
public class CollectionValueParseResult<TCollection, TValue> : ValueParseResult<TCollection>, ICollectionValueParseResult<TCollection, TValue>
{
	#region Properties
	/// <inheritdoc/>
	public TextToken? Prefix { get; }

	/// <inheritdoc/>
	public IReadOnlyList<TextToken> Separators { get; }

	/// <inheritdoc/>
	public IReadOnlyList<IValueParseResult<TValue>> Values { get; }
	IReadOnlyList<IValueParseResult> ICollectionValueParseResult.Values => Values;

	/// <inheritdoc/>
	public TextToken? Suffix { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of a successful <see cref="CollectionValueParseResult{TCollection, TValue}"/>.</summary>
	/// <param name="context">The context for the <paramref name="value"/> that was parsed.</param>
	/// <param name="location">The location of the parsed <paramref name="value"/>.</param>
	/// <param name="value">The value that was parsed.</param>
	/// <param name="prefix">The token used to prefix the collection.</param>
	/// <param name="separators">The tokens used to separate the <paramref name="values"/> in the collection.</param>
	/// <param name="values">The parsing results for the values in the collection.</param>
	/// <param name="suffix">The token used to suffix the collection.</param>
	public CollectionValueParseResult(
		IValueParseContext context,
		TextLocation location,
		TCollection? value,
		TextToken? prefix,
		IReadOnlyList<TextToken> separators,
		IReadOnlyList<IValueParseResult<TValue>> values,
		TextToken? suffix)
		: base(context, location, value)
	{
		Prefix = prefix;
		Separators = separators;
		Values = values;
		Suffix = suffix;
	}

	/// <summary>Creates a new instance of the <see cref="CollectionValueParseResult{TCollection, TValue}"/>.</summary>
	/// <param name="context">The context that was used during the parsing attempt.</param>
	/// <param name="location">The location that the <paramref name="error"/> came from.</param>
	/// <param name="error">The error that occurred during the parse attempt.</param>
	public CollectionValueParseResult(IValueParseContext context, TextLocation location, string error) : base(context, location, error)
	{
		Separators = [];
		Values = [];
	}
	#endregion

	#region Methods
	/// <inheritdoc/>
	public override IEnumerable<TextToken> EnumerateTokens()
	{
		IEnumerable<TextToken> tokens = [];

		if (Prefix is not null)
			tokens = tokens.Append(Prefix.Value);

		tokens = tokens.Concat(Separators);

		foreach (IValueParseResult value in Values)
			tokens = tokens.Concat(value.EnumerateTokens());

		if (Suffix is not null)
			tokens = tokens.Append(Suffix.Value);

		return tokens.Sort();
	}
	#endregion
}
