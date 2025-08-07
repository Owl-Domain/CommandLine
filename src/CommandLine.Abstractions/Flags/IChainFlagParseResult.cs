namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents the result of a chained toggle flag parse operation.
/// </summary>
public interface IChainFlagParseResult : IFlagParseResult
{
	#region Properties
	/// <summary>The parsed flags.</summary>
	IReadOnlyList<IFlagInfo> FlagInfos { get; }
	#endregion
}
