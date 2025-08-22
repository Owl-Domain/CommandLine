namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a flag parse operation.
/// </summary>
/// <param name="prefix">The parsed flag prefix.</param>
/// <param name="name">The name of the parsed flag.</param>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public abstract class BaseFlagParseResult(TextToken prefix, TextToken name) : IFlagParseResult
{
	#region Properties
	/// <inheritdoc/>
	public TextToken Prefix { get; } = prefix;

	/// <inheritdoc/>
	public TextToken Name { get; } = name;

	/// <inheritdoc/>
	public abstract IReadOnlyCollection<IFlagInfo> AffectedFlags { get; }
	#endregion

	#region Methods
	/// <inheritdoc/>
	public virtual IEnumerable<TextToken> EnumerateTokens() => [Prefix, Name];
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string typeName = nameof(BaseFlagParseResult);
		const string nameName = nameof(Name);

		return $"{typeName} {{ {nameName} = ({Name.Value}) }}";
	}
	#endregion
}
