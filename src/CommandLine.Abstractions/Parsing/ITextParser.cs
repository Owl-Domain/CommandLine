using System.Text;

namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents a general text parser.
/// </summary>
public interface ITextParser
{
	#region Properties
	/// <summary>The collection of the fragments that make up the text that should be parsed.</summary>
	IReadOnlyList<TextFragment> Fragments { get; }

	/// <summary>The current fragment that is being parsed.</summary>
	TextFragment CurrentFragment { get; }

	/// <summary>The point in the current fragment.</summary>
	TextPoint Point { get; }

	/// <summary>whether the current fragment is the last fragment.</summary>
	bool IsLastFragment { get; }

	/// <summary>The offset in the current fragment.</summary>
	int Offset { get; }

	/// <summary>The remaining text in the current fragment.</summary>
	ReadOnlySpan<char> Text { get; }

	/// <summary>The remaining text until the next natural breaking point in the current fragment.</summary>
	/// <remarks>If the parser is in greedy mode then this will be equivalent to <see cref="Text"/>.</remarks>
	ReadOnlySpan<char> TextUntilBreak { get; }

	/// <summary>The current character to parse in the current fragment.</summary>
	char Current { get; }

	/// <summary>The next character to parse in the current fragment.</summary>
	char Next { get; }

	/// <summary>Whether the end of the current fragment has been reached.</summary>
	bool IsAtEnd { get; }

	/// <summary>Whether the current fragment should be greedy or lazy parsed.</summary>
	/// <remarks>Only specialised parsers should ever modify this value.</remarks>
	bool IsLazy { get; set; }

	/// <summary>Whether the current fragment should be greedy or lazy parsed.</summary>
	/// <remarks>Only specialised parsers should ever modify this value.</remarks>
	bool IsGreedy { get; set; }
	#endregion

	#region Methods
	/// <summary>Moves the parser back to the fragment at the given <paramref name="fragmentIndex"/>.</summary>
	/// <param name="fragmentIndex">The index of the fragment to return to.</param>
	/// <param name="offset">The offset inside of the fragment to return to.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// 	Thrown if either the given <paramref name="fragmentIndex"/> or the given <paramref name="offset"/> are out of their valid ranges.
	/// </exception>
	void Restore(int fragmentIndex, int offset);

	/// <summary>Selects the next fragment for parsing.</summary>
	/// <exception cref="InvalidOperationException">Thrown if the current fragment is the last one.</exception>
	void NextFragment();

	/// <summary>Advances the offset in the current fragment by the given <paramref name="amount"/>.</summary>
	/// <param name="amount">The amount of characters to advance the fragment by.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if the given <paramref name="amount"/> is less than <c>1</c>.</exception>
	void Advance(int amount = 1);

	/// <summary>Skips any inconsequential characters, which might including going over to the next fragment.</summary>
	/// <exception cref="InvalidOperationException">Thrown if the text parser is at the end of the last fragment.</exception>
	void SkipTrivia();

	/// <summary>Skips any white-space characters that the parser is currently on.</summary>
	void SkipWhitespace();

	/// <summary>Gets the character at the given <paramref name="offset"/>.</summary>
	/// <param name="offset">The offset of the character to peek at.</param>
	/// <returns>
	/// 	The character at the given <paramref name="offset"/>, or <c>'\0'</c>
	/// 	if the given <paramref name="offset"/> is past the current fragment.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if the given <paramref name="offset"/> is negative.</exception>
	char Peek(int offset);
	#endregion
}

/// <summary>
/// 	Contains various extension methods related to the <see cref="ITextParser"/>.
/// </summary>
public static class ITextParserExtensions
{
	#region Methods
	/// <summary>Advances the given text <paramref name="parser"/> until the end of the text remaining in the current fragment.</summary>
	/// <param name="parser">The text parser to advance.</param>
	/// <returns>The text that was consumed.</returns>
	public static string AdvanceText(this ITextParser parser)
	{
		if (parser.Text.Length is 0)
			return string.Empty;

		string value = parser.Text.ToString();
		parser.Advance(value.Length);

		return value;
	}

	/// <summary>Advances the given text <paramref name="parser"/> until the next natural break in the text remaining in the current fragment.</summary>
	/// <param name="parser">The text parser to advance.</param>
	/// <returns>The text that was consumed.</returns>
	public static string AdvanceUntilBreak(this ITextParser parser)
	{
		if (parser.TextUntilBreak.Length is 0)
			return string.Empty;

		string value = parser.TextUntilBreak.ToString();
		parser.Advance(value.Length);

		return value;
	}

	/// <summary>Advances the given text <paramref name="parser"/> while the given <paramref name="predicate"/> returns <see langword="true"/>.</summary>
	/// <param name="parser">The text parser to advance.</param>
	/// <param name="predicate">The predicate to use for controlling the advancement.</param>
	/// <returns>The text that was consumed.</returns>
	public static string AdvanceWhile(this ITextParser parser, Predicate<char> predicate)
	{
		ReadOnlySpan<char> text = parser.Text;

		for (int i = 0; i < text.Length; i++)
		{
			if (predicate.Invoke(text[i]) is false)
			{
				if (i > 0)
					parser.Advance(i);

				return text[..i].ToString();
			}
		}

		return parser.AdvanceText();
	}

	/// <summary>Advances the given text <paramref name="parser"/> until the given <paramref name="predicate"/> returns <see langword="true"/>.</summary>
	/// <param name="parser">The text parser to advance.</param>
	/// <param name="predicate">The predicate to use for controlling the advancement.</param>
	/// <returns>The text that was consumed.</returns>
	public static string AdvanceUntil(this ITextParser parser, Predicate<char> predicate)
	{
		ReadOnlySpan<char> text = parser.Text;

		for (int i = 0; i < text.Length; i++)
		{
			if (predicate.Invoke(text[i]))
			{
				if (i > 0)
					parser.Advance(i);

				return text[..i].ToString();
			}
		}

		return parser.AdvanceText();
	}

	/// <summary>Skips to the end of the last fragment.</summary>
	/// <param name="parser">The text parser to advance.</param>
	/// <returns>The text that was consumed.</returns>
	public static string SkipToEnd(this ITextParser parser)
	{
		StringBuilder builder = new();

		while (parser.IsLastFragment is false)
		{
			builder.Append(parser.Text);
			parser.NextFragment();
		}

		string text = parser.AdvanceText();
		builder.Append(text);

		return builder.ToString();
	}

	/// <summary>Checks whether the given character is the current character that the given text <paramref name="parser"/> is at.</summary>
	/// <param name="parser">The text parser to check.</param>
	/// <param name="current">The current character to check for.</param>
	/// <returns>
	/// 	<see langword="true"/> if the given character is the current
	/// 	character that the parser is on, <see langword="false"/> otherwise.
	/// </returns>
	/// <remarks>
	/// 	If the given <paramref name="current"/> character is the current character that the given text
	///	<paramref name="parser"/> is on, then this method will also advance the <paramref name="parser"/>.
	/// </remarks>
	public static bool Match(this ITextParser parser, char current)
	{
		if (parser.Current == current)
		{
			parser.Advance();
			return true;
		}

		return false;
	}

	/// <summary>
	/// 	Checks whether the next characters in the given text <paramref name="parser"/> match
	/// 	the given <paramref name="current"/> and <paramref name="next"/> characters.
	/// </summary>
	/// <param name="parser">The text parser to check.</param>
	/// <param name="current">The current character in the sequence to check for.</param>
	/// <param name="next">The next character in the sequence to check for.</param>
	/// <returns>
	/// 	<see langword="true"/> if next characters in the given text parser match the given
	/// 	<paramref name="current"/> and <paramref name="next"/> characters, <see langword="false"/> otherwise.
	/// </returns>
	/// <remarks>
	/// 	If the given sequences matches the <paramref name="parser"/> then
	/// 	this method will also advance the <paramref name="parser"/>.
	/// </remarks>
	public static bool Match(this ITextParser parser, char current, char next)
	{
		if (parser.Current == current && parser.Next == next)
		{
			parser.Advance(2);
			return true;
		}

		return false;
	}

	/// <summary>
	/// 	Checks whether the given <paramref name="sequence"/> is the next
	/// 	sequence of characters in the given text <paramref name="parser"/>.
	/// </summary>
	/// <param name="parser">The text parser to check.</param>
	/// <param name="sequence">The sequence to check for.</param>
	/// <returns>
	/// 	<see langword="true"/> if the given <paramref name="sequence"/> is the next sequence of
	/// 	characters in the given text <paramref name="parser"/>, <see langword="false"/> otherwise.
	/// </returns>
	/// <remarks>
	/// 	If the given <paramref name="sequence"/> is matched, then the
	/// 	given text <paramref name="parser"/> will also be advanced.
	/// </remarks>
	public static bool Match(this ITextParser parser, string sequence)
	{
		sequence.ThrowIfEmpty(nameof(sequence));

		for (int i = 0; i < sequence.Length; i++)
		{
			if (parser.Peek(i) != sequence[i])
				return false;
		}

		parser.Advance(sequence.Length);
		return true;
	}
	#endregion
}