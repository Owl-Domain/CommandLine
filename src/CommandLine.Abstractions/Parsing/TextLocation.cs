namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents a span across text fragments.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public readonly struct TextLocation
{
	#region Properties
	/// <summary>The start point.</summary>
	public readonly TextPoint Start { get; }

	/// <summary>The end point.</summary>
	public readonly TextPoint End { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="TextLocation"/>.</summary>
	/// <param name="start">The start point of the location.</param>
	/// <param name="end">The end point of the location.</param>
	public TextLocation(TextPoint start, TextPoint end)
	{
		if (start > end)
			Throw.New.ArgumentOutOfRangeException(nameof(end), end, $"The end point of the location cannot be after the start point.");

		Start = start;
		End = end;
	}
	#endregion

	#region Helpers
	private readonly string DebuggerDisplay()
	{
		const string typeName = nameof(TextLocation);

		return $"{typeName} {{ Start = (#{Start.Fragment.Index:n0} - {Start.Offset:n0}), End = (#{End.Fragment.Index:n0} - {End.Offset:n0}) }}";
	}
	#endregion
}
