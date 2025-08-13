namespace OwlDomain.CommandLine.Parsing.Values.Collections;

/// <summary>
/// 	Represents the result of a collection value parsing operation.
/// </summary>
public interface ICollectionValueParseResult : IValueParseResult
{
	#region Properties
	/// <summary>The token used to prefix the collection.</summary>
	TextToken? Prefix { get; }

	/// <summary>The tokens used to separate the values in the collection.</summary>
	IReadOnlyList<TextToken> Separators { get; }

	/// <summary>The parsing results for the values in the collection.</summary>
	IReadOnlyList<IValueParseResult> Values { get; }

	/// <summary>The token used to suffix the collection.</summary>
	TextToken? Suffix { get; }
	#endregion
}

/// <summary>
/// 	Represents the result of a collection value parsing operation.
/// </summary>
/// <typeparam name="TCollection">The type of the collection that was parsed.</typeparam>
public interface ICollectionValueParseResult<out TCollection> : ICollectionValueParseResult, IValueParseResult<TCollection>
{
}

/// <summary>
/// 	Represents the result of a collection value parsing operation.
/// </summary>
/// <typeparam name="TCollection">The type of the collection that was parsed.</typeparam>
/// <typeparam name="TValue">The type of the values in the collection.</typeparam>
public interface ICollectionValueParseResult<out TCollection, out TValue> : ICollectionValueParseResult<TCollection>
{
	#region Properties
	/// <summary>The results for parsing the values in the collection.</summary>
	new IReadOnlyList<IValueParseResult<TValue>> Values { get; }
	#endregion
}
