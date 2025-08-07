namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a chained toggle flag parse operation.
/// </summary>
/// <param name="flagInfos">The parsed flags.</param>
/// <param name="prefix">The parsed flag prefix.</param>
/// <param name="name">The name of the parsed flag.</param>
public sealed class ChainFlagParseResult(
	IReadOnlyList<IFlagInfo> flagInfos,
	TextToken prefix,
	TextToken name)
	: BaseFlagParseResult(prefix, name), IChainFlagParseResult
{
	#region Properties
	/// <inheritdoc/>
	public IReadOnlyList<IFlagInfo> FlagInfos { get; } = flagInfos;
	#endregion
}
