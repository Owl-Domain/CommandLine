namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents a scope for adding additional break point characters to an <see cref="ITextParser"/>.
/// </summary>
/// <param name="parser">The parser that the scope is for.</param>
/// <param name="oldCharacters">The old characters to restore when the scope is disposed.</param>
public readonly struct BreakPointScope(ITextParser parser, IReadOnlyCollection<char> oldCharacters) : IDisposable
{
	#region Properties
	/// <summary>The parser that the scope is for.</summary>
	public readonly ITextParser Parser { get; } = parser;

	/// <summary>The old characters to restore when the scope is disposed.</summary>
	public readonly IReadOnlyCollection<char> OldCharacters { get; } = oldCharacters;
	#endregion

	#region Methods
	/// <inheritdoc/>
	/// <remarks>This will restore the old break point characters to the parser.</remarks>
	public readonly void Dispose()
	{
		Parser.SetBreakCharacters([.. OldCharacters]);
	}
	#endregion
}

/// <summary>
/// 	Contains various extension methods related to the <see cref="BreakPointScope"/>.
/// </summary>
public static class BreakPointScopeExtensions
{
	#region Methods
	/// <summary>Starts a new scope during which the given <paramref name="characters"/> will be counted as additional break point characters.</summary>
	/// <param name="parser">The parser to start the scope for.</param>
	/// <param name="characters">The additional characters to consider as break point characters inside of the scope.</param>
	/// <returns>The scope which can be disposed to restore the old break point characters.</returns>
	public static BreakPointScope WithBreakCharacters(this ITextParser parser, params ReadOnlySpan<char> characters)
	{
		IReadOnlyCollection<char> old = [.. parser.BreakCharacters];
		parser.AddBreakCharacters(characters);

		return new(parser, old);
	}
	#endregion
}