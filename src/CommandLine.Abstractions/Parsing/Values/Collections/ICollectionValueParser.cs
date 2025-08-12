namespace OwlDomain.CommandLine.Parsing.Values.Collections;

/// <summary>
/// 	Represents a parser for a collection value.
/// </summary>
public interface ICollectionValueParser : IValueParser
{
	#region Properties
	/// <summary>The parser that was selected for the values in the collection.</summary>
	IValueParser ValueParser { get; }
	#endregion

	#region Methods
	/// <summary>Parses a value using the given text <paramref name="parser"/>.</summary>
	/// <param name="context">The context that describes what the parsed value is for.</param>
	/// <param name="parser">The general text parser that should be used for parsing the collection.</param>
	/// <returns>The result of the collection parsing operation.</returns>
	new ICollectionValueParseResult Parse(IValueParseContext context, ITextParser parser);
	IValueParseResult IValueParser.Parse(IValueParseContext context, ITextParser parser) => Parse(context, parser);
	#endregion
}

/// <summary>
/// 	Represents a parser for a collection value.
/// </summary>
/// <typeparam name="TCollection">The type of the collection that was parsed.</typeparam>
public interface ICollectionValueParser<out TCollection> : ICollectionValueParser, IValueParser<TCollection>
{
	#region Methods
	/// <summary>Parses a value using the given text <paramref name="parser"/>.</summary>
	/// <param name="context">The context that describes what the parsed value is for.</param>
	/// <param name="parser">The general text parser that should be used for parsing the collection.</param>
	/// <returns>The result of the collection parsing operation.</returns>
	new ICollectionValueParseResult<TCollection> Parse(IValueParseContext context, ITextParser parser);
	ICollectionValueParseResult ICollectionValueParser.Parse(IValueParseContext context, ITextParser parser) => Parse(context, parser);
	IValueParseResult IValueParser.Parse(IValueParseContext context, ITextParser parser) => Parse(context, parser);
	IValueParseResult<TCollection> IValueParser<TCollection>.Parse(IValueParseContext context, ITextParser parser) => Parse(context, parser);
	#endregion
}

/// <summary>
/// 	Represents a parser for a collection value.
/// </summary>
/// <typeparam name="TCollection">The type of the collection that was parsed.</typeparam>
/// <typeparam name="TValue">The type of the values in the collection.</typeparam>
public interface ICollectionValueParser<out TCollection, out TValue> : ICollectionValueParser<TCollection>
{
	#region Properties
	/// <summary>The parser that was selected for the values in the collection.</summary>
	new IValueParser<TValue> ValueParser { get; }
	IValueParser ICollectionValueParser.ValueParser => ValueParser;
	#endregion

	#region Methods
	/// <summary>Parses a value using the given text <paramref name="parser"/>.</summary>
	/// <param name="context">The context that describes what the parsed value is for.</param>
	/// <param name="parser">The general text parser that should be used for parsing the collection.</param>
	/// <returns>The result of the collection parsing operation.</returns>
	new ICollectionValueParseResult<TCollection, TValue> Parse(IValueParseContext context, ITextParser parser);
	ICollectionValueParseResult<TCollection> ICollectionValueParser<TCollection>.Parse(IValueParseContext context, ITextParser parser) => Parse(context, parser);
	ICollectionValueParseResult ICollectionValueParser.Parse(IValueParseContext context, ITextParser parser) => Parse(context, parser);
	IValueParseResult IValueParser.Parse(IValueParseContext context, ITextParser parser) => Parse(context, parser);
	IValueParseResult<TCollection> IValueParser<TCollection>.Parse(IValueParseContext context, ITextParser parser) => Parse(context, parser);
	#endregion
}
