namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a toggle flag parse operation.
/// </summary>
/// <param name="flagInfo">The parsed flag.</param>
/// <param name="prefix">The parsed flag prefix.</param>
/// <param name="name">The name of the parsed flag.</param>
public sealed class ToggleFlagParseResult(
	IFlagInfo flagInfo,
	TextToken prefix,
	TextToken name)
	: BaseFlagParseResult(prefix, name), IToggleFlagParseResult
{
	#region Properties
	/// <inheritdoc/>
	public IFlagInfo FlagInfo { get; } = flagInfo;
	#endregion
}