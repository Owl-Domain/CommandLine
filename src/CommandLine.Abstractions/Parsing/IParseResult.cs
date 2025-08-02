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

/// <summary>
/// 	Contains various extension methods related to the <see cref="IParseResult"/>.
/// </summary>
public static class IParseResultExtensions
{
	#region Methods
	/// <summary>Enumerates all of the tokens in the given <paramref name="collection"/>.</summary>
	/// <typeparam name="T">The type of parse results in the <paramref name="collection"/>.</typeparam>
	/// <param name="collection">The collection that contains the parse results who's text tokens should be enumerated.</param>
	/// <returns>An enumerable of all of the tokens in the given <paramref name="collection"/>.</returns>
	public static IEnumerable<TextToken> EnumerateTokens<T>(this IEnumerable<T> collection)
		where T : IParseResult
	{
		IEnumerable<TextToken> tokens = [];

		foreach (T item in collection)
		{
			if (item is null)
				continue;

			tokens = tokens.Concat(item.EnumerateTokens());
		}

		return tokens.OrderBy(token => token.Location.Start);
	}

	/// <summary>Sorts the given <paramref name="tokens"/> by their location.</summary>
	/// <param name="tokens">The tokens to sort.</param>
	/// <returns>an enumerable of all of the tokens in their sorted order.</returns>
	public static IEnumerable<TextToken> Sort(this IEnumerable<TextToken> tokens) => tokens.OrderBy(token => token.Location.Start);
	#endregion
}