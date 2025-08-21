namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents the parsing mode of an <see cref="ITextParser"/>.
/// </summary>
public enum ParsingMode : byte
{
	/// <summary>The current fragment is parsed lazily, until a natural break point is reached.</summary>
	Lazy,

	/// <summary>The current fragment is parsed greedily, consuming the entire token regardless of natural break points.</summary>
	Greedy,
}
