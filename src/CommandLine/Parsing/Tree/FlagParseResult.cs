namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a flag parse operation.
/// </summary>
/// <param name="flagInfo">The parsed flag.</param>
/// <param name="prefix">The parsed flag prefix.</param>
/// <param name="name">The name of the parsed flag.</param>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class FlagParseResult(IFlagInfo flagInfo, TextToken prefix, TextToken name) : IFlagParseResult
{
	#region Properties
	/// <inheritdoc/>
	public IFlagInfo FlagInfo { get; } = flagInfo;

	/// <inheritdoc/>
	public TextToken Prefix { get; } = prefix;

	/// <inheritdoc/>
	public TextToken Name { get; } = name;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public IEnumerable<TextToken> EnumerateTokens() => [Prefix, Name];
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string typeName = nameof(FlagParseResult);
		const string nameName = nameof(Name);

		return $"{typeName} {{ {nameName} = ({Name.Value}) }}";
	}
	#endregion
}
