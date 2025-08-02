namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a command engine parse operation.
/// </summary>
public sealed class EngineParseResult : IEngineParseResult
{
	#region Fields
	private readonly IReadOnlyCollection<TextToken> _extraTokens;
	#endregion

	#region Properties
	/// <inheritdoc/>
	public ICommandEngine Engine { get; }

	/// <inheritdoc/>
	public IDiagnosticBag Diagnostics { get; }

	/// <inheritdoc/>
	public IParseResult? CommandOrGroup { get; }

	/// <inheritdoc/>
	public ICommandParseResult? LeafCommand { get; }

	/// <inheritdoc/>
	public IReadOnlyList<IFlagParseResult> Flags { get; }

	/// <inheritdoc/>
	public IReadOnlyList<IArgumentParseResult> Arguments { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="EngineParseResult"/>.</summary>
	/// <param name="engine">The engine that was used for the parsing operation.</param>
	/// <param name="diagnostics">The diagnostics that occurred during the parsing operation.</param>
	/// <param name="commandOrGroup">The result for the parsed command or command group.</param>
	/// <param name="extraTokens">Any extra tokens that were parsed that might not fit into the tree.</param>
	public EngineParseResult(
		ICommandEngine engine,
		IDiagnosticBag diagnostics,
		IParseResult? commandOrGroup,
		IReadOnlyCollection<TextToken> extraTokens)
	{
		if (commandOrGroup is not null && commandOrGroup is not IGroupParseResult and not ICommandParseResult)
			Throw.New.ArgumentException(nameof(IParseResult), $"The given parse result ({commandOrGroup.GetType()}) was not a command or a group parse result.");

		Engine = engine;
		Diagnostics = diagnostics;
		CommandOrGroup = commandOrGroup;
		_extraTokens = extraTokens;

		LeafCommand = GetLeafCommand(commandOrGroup);
		Flags = GetAllFlags(commandOrGroup);
		Arguments = LeafCommand?.Arguments ?? [];
	}
	#endregion

	#region Methods
	/// <inheritdoc/>
	public IEnumerable<TextToken> EnumerateTokens()
	{
		IEnumerable<TextToken> tokens = CommandOrGroup?.EnumerateTokens() ?? [];
		return tokens.Concat(_extraTokens).Sort();
	}
	#endregion

	#region Helpers
	private static ICommandParseResult? GetLeafCommand(IParseResult? result)
	{
		while (result is not null)
		{
			if (result is ICommandParseResult command)
				return command;

			if (result is IGroupParseResult group)
				result = group.CommandOrGroup;
		}

		return null;
	}
	private static List<IFlagParseResult> GetAllFlags(IParseResult? result)
	{
		List<IFlagParseResult> flags = [];

		while (result is not null)
		{
			if (result is ICommandParseResult command)
			{
				flags.AddRange(command.Flags);
				break;
			}

			if (result is IGroupParseResult group)
			{
				flags.AddRange(group.Flags);
				result = group.CommandOrGroup;
			}
		}

		return flags;
	}
	#endregion
}
