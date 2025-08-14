namespace OwlDomain.CommandLine.Parsing.Values.Collections;

/// <summary>
/// 	Represents the base implementation for a collection value parser.
/// </summary>
/// <typeparam name="TCollection">The type of the collection that was parsed.</typeparam>
/// <typeparam name="TValue">The type of the values in the collection.</typeparam>
/// <param name="valueParser">The parser that was selected for the values in the collection.</param>
public abstract class BaseCollectionValueParser<TCollection, TValue>(IValueParser<TValue> valueParser) : ICollectionValueParser<TCollection, TValue>
{
	#region Nested types
	private readonly record struct Structure(
		TextToken? Prefix,
		IReadOnlyList<TextToken> Separators,
		IReadOnlyList<IValueParseResult<TValue>> Values,
		TextToken? Suffix);
	#endregion

	#region Properties
	/// <inheritdoc/>
	public Type ValueType => typeof(TCollection);

	/// <inheritdoc/>
	public IValueParser<TValue> ValueParser { get; } = valueParser;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public ICollectionValueParseResult<TCollection, TValue> Parse(IValueParseContext context, ITextParser parser)
	{
		context.CancellationToken.ThrowIfCancellationRequested();

		Structure structure;
		string? error;
		TextPoint start = parser.Point;

		if (IsValueMissing(parser))
		{
			structure = default;
			error = string.Empty;
		}
		else
			structure = Parse(context, parser, out error);

		TextPoint end = parser.Point;

		if (error is not null)
			return new CollectionValueParseResult<TCollection, TValue>(context, new(start, end), error);

		IReadOnlyList<TValue?> values = [.. structure.Values.Select(r => r.Value)];
		TCollection collection = CreateCollection(values!);

		return new CollectionValueParseResult<TCollection, TValue>(
			context,
			new(start, end),
			collection,
			structure.Prefix,
			structure.Separators,
			structure.Values,
			structure.Suffix);
	}
	private Structure Parse(IValueParseContext context, ITextParser parser, out string? error)
	{
		string prefix = context.Engine.Settings.ListPrefix;
		string separator = context.Engine.Settings.ListValueSeparator;
		string suffix = context.Engine.Settings.ListSuffix;

		if (parser.Match(prefix, TextTokenKind.Symbol, out TextToken prefixToken))
		{
			parser.SkipTrivia();

			using (parser.WithBreakCharacters(prefix[0], separator[0], suffix[0]))
				return ParseSurrounded(context, parser, prefixToken, out error);
		}

		// Note(Nightowl): This might not need the prefix and suffix break points, TBD;
		using (parser.WithBreakCharacters(prefix[0], separator[0], suffix[0]))
			return ParseInline(context, parser, out error);
	}
	private Structure ParseSurrounded(IValueParseContext context, ITextParser parser, TextToken prefix, out string? error)
	{
		string separator = context.Engine.Settings.ListValueSeparator;
		string suffix = context.Engine.Settings.ListSuffix;

		List<TextToken> separators = [];
		List<IValueParseResult<TValue>> values = [];

		while (IsValueMissing(parser) is false)
		{
			context.CancellationToken.ThrowIfCancellationRequested();

			if (parser.Match(suffix, TextTokenKind.Symbol, out TextToken suffixToken))
			{
				parser.SkipTrivia();

				error = default;
				return new(prefix, separators, values, suffixToken);
			}

			IValueParseResult<TValue> valueResult = ParseValue(context, parser);
			if (valueResult.Successful is false)
			{
				error = valueResult.Error;
				return default;
			}

			values.Add(valueResult);
			parser.SkipTrivia();

			if (parser.Match(suffix, TextTokenKind.Symbol, out suffixToken))
			{
				error = default;
				return new(prefix, separators, values, suffixToken);
			}

			if (parser.Match(separator, TextTokenKind.Symbol, out TextToken separatorToken) is false)
			{
				error = $"Expected the '{separator}' symbol to separate the collection values, or the '{suffix}' symbol to end the collection.";
				return default;
			}

			parser.SkipTrivia();
			separators.Add(separatorToken);
		}

		error = $"Expected the '{suffix}' symbol to end the collection.";
		return default;
	}
	private IValueParseResult<TValue> ParseValue(IValueParseContext context, ITextParser parser)
	{
		bool oldIsLazy = parser.IsLazy;
		try
		{
			parser.IsLazy = true;

			IValueParseResult<TValue> valueResult = ValueParser.Parse(context, parser);
			return valueResult;
		}
		finally
		{
			parser.IsLazy = oldIsLazy;
		}
	}
	private Structure ParseInline(IValueParseContext context, ITextParser parser, out string? error)
	{
		string separator = context.Engine.Settings.ListValueSeparator;

		List<TextToken> separators = [];
		List<IValueParseResult<TValue>> values = [];

		while (IsValueMissing(parser) is false)
		{
			context.CancellationToken.ThrowIfCancellationRequested();

			IValueParseResult<TValue> valueResult = ParseValue(context, parser);
			if (valueResult.Successful is false)
			{
				error = valueResult.Error;
				return default;
			}

			values.Add(valueResult);
			parser.SkipTrivia();

			if (parser.Match(separator, TextTokenKind.Symbol, out TextToken separatorToken) is false)
			{
				error = default;
				return new(null, separators, values, null);
			}

			separators.Add(separatorToken);
			parser.SkipTrivia();
		}

		error = default;
		return new(null, separators, values, null);
	}

	/// <summary>Creates a collection instance filled with the given <paramref name="values"/>.</summary>
	/// <param name="values">The values to fill the created collection with.</param>
	/// <returns>The created collection.</returns>
	protected abstract TCollection CreateCollection(IReadOnlyList<TValue> values);
	#endregion

	#region Helpers
	private static bool IsValueMissing(ITextParser parser)
	{
		if (parser.IsLazy)
			return parser.IsAtEnd;

		Debug.Assert(parser.IsGreedy);

		return parser.IsAtEnd && parser.CurrentFragment.Length > 0;
	}
	#endregion
}
