namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a flag parse operation.
/// </summary>
/// <param name="flagInfo">The parsed flag.</param>
/// <param name="prefix">The parsed flag prefix.</param>
/// <param name="name">The name of the parsed flag.</param>
/// <param name="separator">The token for the separator between the flag name and the flag value.</param>
/// <param name="value">The value parsed for the flag.</param>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class ValueFlagParseResult(
	IFlagInfo flagInfo,
	TextToken prefix,
	TextToken name,
	TextToken? separator,
	IValueParseResult value)
	: IValueFlagParseResult
{
	#region Properties
	/// <inheritdoc/>
	public IFlagInfo FlagInfo { get; } = flagInfo;

	/// <inheritdoc/>
	public TextToken Prefix { get; } = prefix;

	/// <inheritdoc/>
	public TextToken Name { get; } = name;

	/// <inheritdoc/>
	public TextToken? Separator { get; } = separator;

	/// <inheritdoc/>
	public IValueParseResult Value { get; } = value;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public IEnumerable<TextToken> EnumerateTokens()
	{
		IEnumerable<TextToken> tokens = [Prefix, Name];

		if (Separator is not null)
			tokens = tokens.Append(Separator.Value);

		tokens = tokens.Concat(Value.EnumerateTokens());

		return tokens.Sort();
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string typeName = nameof(ValueFlagParseResult);
		const string nameName = nameof(Name);

		return $"{typeName} {{ {nameName} = ({Name.Value}) }}";
	}
	#endregion
}
