namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents a single point inside of a text fragment.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public readonly struct TextPoint :
#if NET7_0_OR_GREATER
	IEqualityOperators<TextPoint, TextPoint, bool>,
	IComparisonOperators<TextPoint, TextPoint, bool>,
#endif
	IEquatable<TextPoint>, IComparable<TextPoint>
{
	#region Properties
	/// <summary>The fragment that the point is inside of.</summary>
	public readonly TextFragment Fragment { get; }

	/// <summary>The offset inside of the fragment.</summary>
	public readonly int Offset { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="TextPoint"/>.</summary>
	/// <param name="fragment">The fragment that the point is inside of.</param>
	/// <param name="offset">The offset inside of the fragment.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// 	Thrown if the given <paramref name="offset"/> is outside of the given <paramref name="fragment"/>.
	/// </exception>
	public TextPoint(TextFragment fragment, int offset)
	{
		offset.ThrowIfLessThan(0, nameof(offset));

		if (fragment.Length > 0 && offset >= fragment.Length)
			Throw.New.ArgumentOutOfRangeException(nameof(offset), offset, $"The given offset ({offset:n0}) was greater than the length of the fragment ({fragment.Length:n0}).");

		Fragment = fragment;
		Offset = offset;
	}
	#endregion

	#region Methods
	/// <inheritdoc/>
	public readonly bool Equals(TextPoint other)
	{
		return
			Fragment == other.Fragment &&
			Offset == other.Offset;
	}

	/// <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is TextPoint other)
			return Equals(other);

		return false;
	}

	/// <inheritdoc/>
	public readonly int CompareTo(TextPoint other)
	{
		if (Fragment != other.Fragment)
			return Fragment.Index.CompareTo(other.Fragment.Index);

		return Offset.CompareTo(other.Offset);
	}

	/// <inheritdoc/>
	public readonly override int GetHashCode() => HashCode.Combine(Fragment, Offset);
	#endregion

	#region Helpers
	private readonly string DebuggerDisplay()
	{
		const string typeName = nameof(TextPoint);
		const string indexName = $"{nameof(Fragment)}.{nameof(Fragment.Index)}";
		const string offsetName = nameof(Offset);

		return $"{typeName} {{ {indexName} = ({Fragment.Index:n0}), {offsetName} = ({Offset:n0}) }}";
	}
	#endregion

	#region Operators
	/// <summary>Compares two values to determine equality.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator ==(TextPoint left, TextPoint right) => left.Equals(right);

	/// <summary>Compares two values to determine inequality.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator !=(TextPoint left, TextPoint right) => left.Equals(right) is false;

	/// <summary>Compares two values to determine which is lesser.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator <(TextPoint left, TextPoint right) => left.CompareTo(right) < 0;

	/// <summary>Compares two values to determine which is greater.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator >(TextPoint left, TextPoint right) => left.CompareTo(right) > 0;

	/// <summary>Compares two values to determine which is lesser or equal.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator <=(TextPoint left, TextPoint right) => left.CompareTo(right) <= 0;

	/// <summary>Compares two values to determine which is greater or equal.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator >=(TextPoint left, TextPoint right) => left.CompareTo(right) >= 0;
	#endregion
}
