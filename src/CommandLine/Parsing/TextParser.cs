namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents a general text parser.
/// </summary>
public sealed class TextParser : ITextParser
{
	#region Fields
	private int _currentFragmentIndex;
	#endregion

	#region Properties
	/// <inheritdoc/>
	public IReadOnlyList<TextFragment> Fragments { get; }

	/// <inheritdoc/>
	public TextFragment CurrentFragment { get; private set; }

	/// <inheritdoc/>
	public TextPoint Point => new(CurrentFragment, Offset);

	/// <inheritdoc/>
	public bool IsLastFragment => CurrentFragment.Index >= Fragments.Count - 1;

	/// <inheritdoc/>
	public int Offset { get; private set; }

	/// <inheritdoc/>
	public ReadOnlySpan<char> Text => CurrentFragment.Text.AsSpan(Offset);

	/// <inheritdoc/>
	public ReadOnlySpan<char> TextUntilBreak
	{
		get
		{
			if (IsLazy is false)
				return Text;

			ReadOnlySpan<char> span = Text;
			int index = span.IndexOf(' ');

			if (index >= 0)
				return span[..index];

			return span;
		}
	}

	/// <inheritdoc/>
	public char Current => Peek(0);

	/// <inheritdoc/>
	public char Next => Peek(1);

	/// <inheritdoc/>
	public bool IsAtEnd { get; }

	/// <inheritdoc/>
	public bool IsLazy { get; set; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="TextParser"/>.</summary>
	/// <param name="fragments">The text fragments to initialise the parser with.</param>
	/// <param name="isLazy">Whether the parser should be greedy or lazy.</param>
	public TextParser(IReadOnlyList<TextFragment> fragments, bool isLazy)
	{
		if (fragments.Count <= 1)
			Throw.New.ArgumentException(nameof(fragments), $"The {nameof(TextParser)} requires at least one fragment.");

		Fragments = fragments;
		CurrentFragment = Fragments[0];
		IsLazy = isLazy;
	}

	/// <summary>Creates a new instance of the <see cref="TextParser"/>.</summary>
	/// <param name="fragments">The text fragments to initialise the parser with.</param>
	/// <param name="isLazy">Whether the parser should be greedy or lazy.</param>
	public TextParser(IReadOnlyList<string> fragments, bool isLazy)
	{
		if (fragments.Count <= 1)
			Throw.New.ArgumentException(nameof(fragments), $"The {nameof(TextParser)} requires at least one fragment.");

		Fragments = [.. fragments.Select((fragment, index) => new TextFragment(fragment, index))];
		CurrentFragment = Fragments[0];
		IsLazy = isLazy;
	}
	#endregion

	#region Methods
	/// <inheritdoc/>
	public void Advance(int amount = 1)
	{
		amount.ThrowIfLessThan(1, nameof(amount));

		Offset = Math.Min(Text.Length, Offset + amount);
	}

	/// <inheritdoc/>
	public void NextFragment()
	{
		if (IsLastFragment)
			Throw.New.InvalidOperationException($"The {nameof(TextParser)} is already at the last text fragment.");

		_currentFragmentIndex++;
		CurrentFragment = Fragments[_currentFragmentIndex];
		Offset = 0;
	}

	/// <inheritdoc/>
	public void Restore(int fragmentIndex, int offset)
	{
		fragmentIndex.ThrowIfNotBetween(0, Fragments.Count, RangeCheckMode.InclusiveExclusive, nameof(fragmentIndex));

		TextFragment fragment = Fragments[fragmentIndex];
		offset.ThrowIfNotBetween(0, fragment.Length, RangeCheckMode.InclusiveExclusive, nameof(offset));

		_currentFragmentIndex = fragmentIndex;
		CurrentFragment = fragment;
		Offset = offset;
	}

	/// <inheritdoc/>
	public void SkipWhitespace()
	{
		ReadOnlySpan<char> text = Text;

#if NET7_0_OR_GREATER
		int index = text.IndexOfAnyExcept(' ');
#else
		int index = -1;
		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] is not ' ')
			{
				index = i;
				break;
			}
		}
#endif

		if (index > 0)
			Advance(index);
		else if (index < 0)
			Advance(text.Length);
	}
	#endregion

	#region Helpers
	private char Peek(int offset)
	{
		offset.ThrowIfLessThan(0, nameof(offset));

		int index = Offset + offset;
		ReadOnlySpan<char> text = Text;

		return index < text.Length ? text[index] : '\0';
	}
	#endregion
}
