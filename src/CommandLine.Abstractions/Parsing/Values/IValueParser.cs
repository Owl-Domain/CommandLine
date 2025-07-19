namespace OwlDomain.CommandLine.Parsing.Values;

/// <summary>
/// 	Represents a parser for a value.
/// </summary>
public interface IValueParser
{
	#region Properties
	/// <summary>The type of the value that this parser can parse.</summary>
	Type ValueType { get; }
	#endregion

	#region Methods
	/// <summary>Parses a value using the given text <paramref name="parser"/>.</summary>
	/// <param name="context">The context that describes what the parsed value is for.</param>
	/// <param name="parser">The general text parser that should be used for parsing the value.</param>
	/// <returns>The result of the parse operation.</returns>
	IValueParseResult Parse(IValueParseContext context, ITextParser parser);
	#endregion
}

/// <summary>
/// 	Represents a parser for a value.
/// </summary>
/// <typeparam name="T">The type of the value that will be parsed.</typeparam>
public interface IValueParser<out T> : IValueParser
{
	#region Properties
	Type IValueParser.ValueType => typeof(T);
	#endregion

	#region Methods
	/// <summary>Parses a value using the given text <paramref name="parser"/>.</summary>
	/// <param name="context">The context that describes what the parsed value is for.</param>
	/// <param name="parser">The general text parser that should be used for parsing the value.</param>
	/// <returns>The result of the parse operation.</returns>
	new IValueParseResult<T> Parse(IValueParseContext context, ITextParser parser);
	IValueParseResult IValueParser.Parse(IValueParseContext context, ITextParser parser) => Parse(context, parser);
	#endregion
}
