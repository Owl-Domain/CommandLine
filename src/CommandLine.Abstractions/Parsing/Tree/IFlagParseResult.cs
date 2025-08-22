namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a flag parse operation.
/// </summary>
public interface IFlagParseResult : IParseResult
{
	#region Properties
	/// <summary>The parsed flag prefix.</summary>
	TextToken Prefix { get; }

	/// <summary>The name of the parsed flag.</summary>
	TextToken Name { get; }

	/// <summary>The flags that are affected by this parse result.</summary>
	IReadOnlyCollection<IFlagInfo> AffectedFlags { get; }
	#endregion
}
