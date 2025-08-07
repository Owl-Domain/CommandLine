namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a toggle flag parse operation.
/// </summary>
public interface IToggleFlagParseResult : IFlagParseResult
{
	#region Properties
	/// <summary>The parsed flag.</summary>
	IFlagInfo FlagInfo { get; }
	#endregion
}
