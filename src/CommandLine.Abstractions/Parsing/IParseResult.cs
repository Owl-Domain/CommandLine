namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents a general parse result.
/// </summary>
public interface IParseResult
{
	#region Methods
	/// <summary>Enumerates all of the parsed tokens.</summary>
	/// <returns>An enumerable of all of the parsed tokens.</returns>
	IEnumerable<TextToken> EnumerateTokens();
	#endregion
}
