namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents the result of a command engine parse operation.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class CommandParserResult : ICommandParserResult
{
	#region Fields
	private readonly IReadOnlyCollection<TextToken> _extraTokens;
	#endregion

	#region Properties
	/// <inheritdoc/>
	public bool Successful { get; }

	/// <inheritdoc/>
	public DiagnosticSource Stage => DiagnosticSource.Parsing;

	/// <inheritdoc/>
	public ICommandEngine Engine { get; }

	/// <inheritdoc/>
	public ICommandParser Parser { get; }

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

	/// <inheritdoc/>
	public TimeSpan Duration { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="CommandParserResult"/>.</summary>
	/// <param name="successful">Whether the operation was successful.</param>
	/// <param name="engine">The engine that was used for the parsing operation.</param>
	/// <param name="parser">The parser that was used to parse the command.</param>
	/// <param name="diagnostics">The diagnostics that occurred during the parsing operation.</param>
	/// <param name="commandOrGroup">The result for the parsed command or command group.</param>
	/// <param name="extraTokens">Any extra tokens that were parsed that might not fit into the tree.</param>
	/// <param name="duration">The amount of time that the parsing operation took.</param>
	public CommandParserResult(
		bool successful,
		ICommandEngine engine,
		ICommandParser parser,
		IDiagnosticBag diagnostics,
		IParseResult? commandOrGroup,
		IReadOnlyCollection<TextToken> extraTokens,
		TimeSpan duration)
	{
		if (commandOrGroup is not null && commandOrGroup is not IGroupParseResult and not ICommandParseResult)
			Throw.New.ArgumentException(nameof(IParseResult), $"The given parse result ({commandOrGroup.GetType()}) was not a command or a group parse result.");

		Successful = successful;
		Engine = engine;
		Parser = parser;
		Diagnostics = diagnostics;
		CommandOrGroup = commandOrGroup;
		_extraTokens = extraTokens;
		Duration = duration;

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
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string typeName = nameof(CommandParserResult);
		const string successfulName = nameof(Successful);
		const string durationName = nameof(Duration);

		return $"{typeName} {{ {successfulName} = ({Successful}), {durationName} = ({Duration}) }}";
	}
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
