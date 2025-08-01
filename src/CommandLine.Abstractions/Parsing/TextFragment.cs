namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents a fragment of text.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public readonly struct TextFragment :
#if NET7_0_OR_GREATER
	IEqualityOperators<TextFragment, TextFragment, bool>,
#endif
	IEquatable<TextFragment>
{
	#region Properties
	/// <summary>The text that makes up the fragment.</summary>
	public readonly string Text { get; }

	/// <summary>The index of the fragment.</summary>
	public readonly int Index { get; }

	/// <summary>The length of the fragment.</summary>
	public readonly int Length => Text.Length;
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="TextFragment"/>.</summary>
	/// <param name="text">The text that makes up the fragment.</param>
	/// <param name="index">The index of the fragment.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if the given <paramref name="index"/> is less than <c>0</c>.</exception>
	public TextFragment(string text, int index)
	{
		index.ThrowIfLessThan(0, nameof(index));

		Text = text;
		Index = index;
	}

	/// <summary>Creates a new empty instance of the <see cref="TextFragment"/>.</summary>
	public TextFragment() => Text = string.Empty;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public readonly bool Equals(TextFragment other)
	{
		return
			Text == other.Text &&
			Index == other.Index;
	}

	/// <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is TextFragment other)
			return Equals(other);

		return false;
	}

	/// <inheritdoc/>
	public readonly override int GetHashCode() => HashCode.Combine(Text, Index);
	#endregion

	#region Helpers
	private readonly string DebuggerDisplay()
	{
		const string typeName = nameof(TextFragment);
		const string indexName = nameof(Index);
		const string textName = nameof(Text);

		return $"{typeName} {{ {indexName} = ({Index:n0}), {textName} = ({Text}) }}";
	}
	#endregion

	#region Operators
	/// <summary>Compares two values to determine equality.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator ==(TextFragment left, TextFragment right) => left.Equals(right);

	/// <summary>Compares two values to determine inequality.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator !=(TextFragment left, TextFragment right) => left.Equals(right) is false;
	#endregion
}
