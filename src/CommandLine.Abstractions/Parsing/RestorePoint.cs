namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents a restore point for the <see cref="ITextParser"/>.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public readonly struct RestorePoint
{
	#region Properties
	/// <summary>The fragment to restore the parser to.</summary>
	public readonly TextFragment Fragment { get; }

	/// <summary>The offset inside of the fragment to restore the parser to.</summary>
	public readonly int Offset { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="RestorePoint"/>.</summary>
	/// <param name="fragment">The fragment to restore the parser to.</param>
	/// <param name="offset">The offset inside of the given <paramref name="fragment"/> restore the parser to.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// 	Thrown if the given <paramref name="offset"/> is not a valid position inside of the given <paramref name="fragment"/>.
	/// </exception>
	public RestorePoint(TextFragment fragment, int offset)
	{
		offset.ThrowIfNotBetween(0, fragment.Length, RangeCheckMode.Inclusive, nameof(offset));

		Fragment = fragment;
		Offset = offset;
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private readonly string DebuggerDisplay()
	{
		const string typeName = nameof(RestorePoint);
		const string indexName = $"{nameof(Fragment)}.{nameof(Fragment.Index)}";
		const string offsetName = nameof(Offset);

		return $"{typeName} {{ {indexName} = ({Fragment.Index:n0}), {offsetName} = ({Offset:n0}) }}";
	}
	#endregion
}
