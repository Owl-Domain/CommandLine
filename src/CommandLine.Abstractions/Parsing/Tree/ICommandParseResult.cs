namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a command group parse operation.
/// </summary>
public interface ICommandParseResult : IParseResult
{
	#region Properties
	/// <summary>The command that was parsed.</summary>
	ICommandInfo CommandInfo { get; }

	/// <summary>The token for the command name.</summary>
	/// <remarks>The name might be <see langword="null"/> even if parsing was successful.</remarks>
	TextToken? Name { get; }

	/// <summary>The parsed flags.</summary>
	IReadOnlyList<IFlagParseResult> Flags { get; }

	/// <summary>The parsed arguments.</summary>
	IReadOnlyList<IArgumentParseResult> Arguments { get; }
	#endregion
}