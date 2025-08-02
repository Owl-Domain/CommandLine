namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a command group parse operation.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class GroupParseResult : IGroupParseResult
{
	#region Properties
	/// <inheritdoc/>
	public ICommandGroupInfo GroupInfo { get; }

	/// <inheritdoc/>
	public TextToken? Name { get; }

	/// <inheritdoc/>
	public IReadOnlyList<IFlagParseResult> Flags { get; }

	/// <inheritdoc/>
	public IParseResult? CommandOrGroup { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="GroupParseResult"/>.</summary>
	/// <param name="groupInfo">The group that was parsed.</param>
	/// <param name="name">The token for the command group name.</param>
	/// <param name="flags">The parsed flags.</param>
	/// <param name="commandOrGroup">The result for the parsed sub-command or sub-command group.</param>
	public GroupParseResult(
		ICommandGroupInfo groupInfo,
		TextToken? name,
		IReadOnlyList<IFlagParseResult> flags,
	 	IParseResult? commandOrGroup)
	{
		if (commandOrGroup is not null && commandOrGroup is not IGroupParseResult and not ICommandParseResult)
			Throw.New.ArgumentException(nameof(IParseResult), $"The given parse result ({commandOrGroup.GetType()}) was not a command or a group parse result.");

		GroupInfo = groupInfo;
		Name = name;
		Flags = flags;
		CommandOrGroup = commandOrGroup;
	}
	#endregion

	#region Methods
	/// <inheritdoc/>
	public IEnumerable<TextToken> EnumerateTokens()
	{
		IEnumerable<TextToken> tokens = [];

		if (Name is not null)
			tokens = tokens.Append(Name.Value);

		tokens = tokens.Concat(Flags.EnumerateTokens());

		if (CommandOrGroup is not null)
			tokens = tokens.Concat(CommandOrGroup?.EnumerateTokens());

		return tokens.Sort();
	}
	#endregion

	#region Helpers
	private string DebuggerDisplay()
	{
		const string typeName = nameof(GroupParseResult);
		const string groupInfoName = $"{nameof(GroupInfo)}.{nameof(GroupInfo.Name)}";

		return $"{typeName} {{ {groupInfoName} = ({GroupInfo.Name}) }}";
	}
	#endregion
}
