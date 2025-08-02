namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a command group parse operation.
/// </summary>
/// <param name="commandInfo">The command that was parsed.</param>
/// <param name="name">The token for the command name.</param>
/// <param name="flags">The parsed flags.</param>
/// <param name="arguments">The parsed arguments.</param>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class CommandParseResult(
	ICommandInfo commandInfo,
	TextToken? name,
	IReadOnlyList<IFlagParseResult> flags,
	IReadOnlyList<IArgumentParseResult> arguments)
	: ICommandParseResult
{
	#region Properties
	/// <inheritdoc/>
	public ICommandInfo CommandInfo { get; } = commandInfo;

	/// <inheritdoc/>
	public TextToken? Name { get; } = name;

	/// <inheritdoc/>
	public IReadOnlyList<IFlagParseResult> Flags { get; } = flags;

	/// <inheritdoc/>
	public IReadOnlyList<IArgumentParseResult> Arguments { get; } = arguments;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public IEnumerable<TextToken> EnumerateTokens()
	{
		IEnumerable<TextToken> tokens = [];

		if (Name is not null)
			tokens = tokens.Append(Name.Value);

		tokens = tokens
			.Concat(Flags.EnumerateTokens())
			.Concat(Arguments.EnumerateTokens());

		return tokens.Sort();
	}
	#endregion

	#region Helpers
	private string DebuggerDisplay()
	{
		const string typeName = nameof(CommandParseResult);
		const string commandInfoName = $"{nameof(CommandInfo)}.{nameof(CommandInfo.Name)}";

		return $"{typeName} {{ {commandInfoName} = ({CommandInfo.Name}) }}";
	}
	#endregion
}
