namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents the result of a repeat flag parse operation.
/// </summary>
public interface IRepeatFlagParseResult : IFlagParseResult
{
	#region Properties
	/// <summary>The parsed flag.</summary>
	IFlagInfo FlagInfo { get; }

	/// <summary>The repetition count of the flag.</summary>
	int Repetition { get; }
	#endregion
}
