namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a command group parse operation.
/// </summary>
public interface IGroupParseResult : IParseResult
{
	#region Properties
	/// <summary>The group that was parsed.</summary>
	ICommandGroupInfo GroupInfo { get; }

	/// <summary>The token for the command group name.</summary>
	/// <remarks>The name might be <see langword="null"/> even if parsing was successful.</remarks>
	TextToken? Name { get; }

	/// <summary>The parsed flags.</summary>
	IReadOnlyList<IFlagParseResult> Flags { get; }

	/// <summary>The result for the parsed sub-command or sub-command group.</summary>
	/// <remarks>This might be <see langword="null"/> if the parsing failed.</remarks>
	IParseResult? CommandOrGroup { get; }
	#endregion
}
