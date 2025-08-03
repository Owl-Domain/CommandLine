namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents a parsed text token.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public readonly struct TextToken
{
	#region Properties
	/// <summary>The kind of the text token.</summary>
	public readonly TextTokenKind Kind { get; }

	/// <summary>The location of the parsed token.</summary>
	public readonly TextLocation Location { get; }

	/// <summary>An optional value associated with the token.</summary>
	public readonly object? Value { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="TextToken"/>.</summary>
	/// <param name="kind">The kind of the text token.</param>
	/// <param name="location">The location of the parsed token.</param>
	/// <param name="value">An optional value associated with the token.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// 	Thrown if the given <paramref name="kind"/> is not defined in the <see cref="TextTokenKind"/> <see langword="enum"/>.
	/// </exception>
	public TextToken(TextTokenKind kind, TextLocation location, object? value = null)
	{
		kind.ThrowIfNotDefined(nameof(kind));

		Kind = kind;
		Location = location;
		Value = value;
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private readonly string DebuggerDisplay()
	{
		const string typeName = nameof(TextToken);
		const string kindName = nameof(Kind);
		const string valueName = nameof(Value);

		return $"{typeName} {{ {kindName} = ({Kind}), {valueName} = ({Value}) }}";
	}
	#endregion
}
