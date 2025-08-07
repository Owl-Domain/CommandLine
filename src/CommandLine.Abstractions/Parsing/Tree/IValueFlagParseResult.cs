namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a value flag parse operation.
/// </summary>
public interface IValueFlagParseResult : IFlagParseResult
{
	#region Properties
	/// <summary>The parsed flag.</summary>
	IFlagInfo FlagInfo { get; }

	/// <summary>The token for the separator between the flag name and the flag value.</summary>
	TextToken? Separator { get; }

	/// <summary>The value parsed for the flag.</summary>
	IValueParseResult Value { get; }
	#endregion
}
