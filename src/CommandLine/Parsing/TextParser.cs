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
			if (IsGreedy)
				return Text;

			ReadOnlySpan<char> span = Text;

			for (int i = 0; i < span.Length; i++)
			{
				if (char.IsWhiteSpace(span[i]))
					return span[..i];
			}

			return span;
		}
	}

	/// <inheritdoc/>
	public char Current => Peek(0);

	/// <inheritdoc/>
	public char Next => Peek(1);

	/// <inheritdoc/>
	public bool IsAtEnd => Offset >= CurrentFragment.Length;

	/// <inheritdoc/>
	public bool IsLazy { get; set; }

	/// <inheritdoc/>
	public bool IsGreedy
	{
		get => IsLazy is false;
		set => IsLazy = value is false;
	}
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="TextParser"/>.</summary>
	/// <param name="fragments">The text fragments to initialise the parser with.</param>
	/// <param name="isLazy">Whether the parser should be greedy or lazy.</param>
	public TextParser(IReadOnlyList<TextFragment> fragments, bool isLazy)
	{
		if (fragments.Count < 1)
			Throw.New.ArgumentException(nameof(fragments), $"The {nameof(TextParser)} requires at least one fragment.");

		for (int i = 0; i < fragments.Count; i++)
		{
			if (fragments[i].Index != i)
				Throw.New.ArgumentException(nameof(fragments), $"The fragment at index #{i:n0} had the incorrect index of #({fragments[i].Index:n0}).");
		}

		Fragments = fragments;
		CurrentFragment = Fragments[0];
		IsLazy = isLazy;
	}

	/// <summary>Creates a new instance of the <see cref="TextParser"/>.</summary>
	/// <param name="fragments">The text fragments to initialise the parser with.</param>
	/// <param name="isLazy">Whether the parser should be greedy or lazy.</param>
	public TextParser(IReadOnlyList<string> fragments, bool isLazy)
	{
		if (fragments.Count < 1)
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

		Offset = Math.Min(CurrentFragment.Length, Offset + amount);
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
	public RestorePoint GetRestorePoint() => new(CurrentFragment, Offset);

	/// <inheritdoc/>
	public void Restore(RestorePoint point)
	{
		if (Fragments.Contains(point.Fragment) is false)
			Throw.New.ArgumentException(nameof(point), $"The fragment in the given point was not a part of the current text parser.");

		_currentFragmentIndex = point.Fragment.Index;
		CurrentFragment = point.Fragment;
		Offset = point.Offset;
	}

	/// <inheritdoc/>
	public void SkipTrivia()
	{
		SkipWhitespace();

		if (IsLastFragment is false && IsGreedy && IsAtEnd)
			NextFragment();
	}

	/// <inheritdoc/>
	public void SkipWhitespace()
	{
		while (char.IsWhiteSpace(Current))
			Advance();
	}

	/// <inheritdoc/>
	public char Peek(int offset)
	{
		offset.ThrowIfLessThan(0, nameof(offset));

		int index = Offset + offset;
		ReadOnlySpan<char> text = CurrentFragment.Text;

		return index < text.Length ? text[index] : '\0';
	}
	#endregion
}
