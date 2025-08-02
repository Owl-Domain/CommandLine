namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents the command line engine.
/// </summary>
/// <param name="rootGroup">The root command group.</param>
public sealed class CommandEngine(ICommandGroupInfo rootGroup) : ICommandEngine
{
	#region Nested types
	private sealed class Context(TextParser parser)
	{
		#region Properties
		public TextParser Parser { get; } = parser;
		public DiagnosticBag Diagnostics { get; } = [];
		public List<TextToken> ExtraTokens { get; } = [];
		#endregion
	}
	#endregion

	#region Properties
	/// <inheritdoc/>
	public ICommandGroupInfo RootGroup { get; } = rootGroup;
	#endregion

	#region Functions
	/// <summary>Creates a builder for a new command engine.</summary>
	/// <returns>The builder which can be used to customise the built command engine.</returns>
	public static ICommandEngineBuilder New() => new CommandEngineBuilder();
	#endregion

	#region Methods
	/// <inheritdoc/>
	public IEngineParseResult Parse(string[] fragments)
	{
		TextParser parser = new(fragments, false);
		return Parse(parser);
	}

	/// <inheritdoc/>
	public IEngineParseResult Parse(string text)
	{
		TextParser parser = new([text], true);
		return Parse(parser);
	}
	private IEngineParseResult Parse(TextParser parser)
	{
		Context context = new(parser);
		IParseResult? result = ParseGroup(context, RootGroup);

		CheckForLeftOverInput(context);

		return new EngineParseResult(this, context.Diagnostics, result, context.ExtraTokens);
	}

	/// <inheritdoc/>
	public IEngineValidationResult Validate(IEngineParseResult parseResult)
	{
		if (parseResult.Diagnostics.Any())
			Throw.New.ArgumentException(nameof(parseResult), $"Validation cannot be performed if there were parsing errors.");

		DiagnosticBag diagnostics = [];
		EngineValidationResult result = new(parseResult, diagnostics);

		return result;
	}

	/// <inheritdoc/>
	public IEngineExecutionResult Execute(IEngineValidationResult validationResult)
	{
		if (validationResult.Diagnostics.Any())
			Throw.New.ArgumentException(nameof(validationResult), $"Execution cannot be performed if there were validation errors.");

		if (validationResult.ParseResult.LeafCommand is not ICommandParseResult command)
		{
			Throw.New.ArgumentException(nameof(validationResult), $"The given validation result did not have a parsed command to execute.");
			return default; // Note(Nightowl): Never happens, needed for analysis to know the 'command' variable is always assigned later on;
		}

		DiagnosticBag diagnostics = [];

		Debug.Assert(command.Arguments.Count == command.CommandInfo.Arguments.Count);

		if (command.CommandInfo is IMethodCommandInfo methodCommand)
		{
			object? container = null;
			if (methodCommand.Method.IsStatic is false)
			{
				container = Activator.CreateInstance(methodCommand.Method.ReflectedType);
				Debug.Assert(container is not null);
			}

			object?[] parameters = new object?[methodCommand.Method.GetParameters().Length];

			foreach (IArgumentParseResult argument in command.Arguments)
				parameters[argument.ArgumentInfo.Position] = argument.Value.Value;

			_ = methodCommand.Method.Invoke(container, parameters);
		}
		else if (command.CommandInfo is IVirtualCommandInfo)
			Throw.New.NotImplementedException($"Executing virtual commands has not been implemented yet.");
		else
			Throw.New.InvalidOperationException($"Unknown command type ({command.CommandInfo?.GetType()}).");

		EngineExecutionResult result = new(validationResult, diagnostics);

		return result;
	}
	#endregion

	#region Parse helpers
	private void CheckForLeftOverInput(Context context)
	{
		context.Parser.SkipTrivia();
		bool hasLeftOver = (context.Parser.IsAtEnd && context.Parser.IsLastFragment) is false;

		if (hasLeftOver)
		{
			if (context.Parser.IsAtEnd)
				context.Parser.NextFragment();

			TextPoint start = context.Parser.Point;
			string leftOver = context.Parser.SkipToEnd();
			TextPoint end = context.Parser.Point;

			TextLocation location = new(start, end);

			TextToken token;
			if (context.Diagnostics.Any())
			{
				// Note(Nightowl): Has other diagnostics, likely a carry-over error from early quitting;
				token = new(TextTokenKind.Unprocessed, location, leftOver);
			}
			else
			{
				token = new(TextTokenKind.Error, location, leftOver);
				context.Diagnostics.Add(DiagnosticSource.Parsing, location, $"Not all of the input was parsed.");
			}

			context.ExtraTokens.Add(token);
		}
	}
	private IParseResult ParseGroup(Context context, ICommandGroupInfo group)
	{
		string? groupCommandError = (group.Groups.Count > 0, group.Commands.Count > 0) switch
		{
			(true, true) => "group/command",
			(true, false) => "group",
			(false, true) => "command",
			(false, false) => null,
		};

		int fragmentIndex = context.Parser.CurrentFragment.Index, offset = context.Parser.Offset;

		if (groupCommandError is not null)
		{
			if (TryParseName(context.Parser, out TextToken? nameToken, out string? name))
			{
				if (group.Groups.TryGetValue(name, out ICommandGroupInfo? childGroup))
				{
					IParseResult subResult = ParseGroup(context, childGroup);

					return subResult;
				}
				else if (group.Commands.TryGetValue(name, out ICommandInfo? command))
				{
					ICommandParseResult subCommand = ParseCommand(context, command, nameToken);

					return subCommand;
				}
			}
			else
				context.Parser.Restore(fragmentIndex, offset);
		}

		if (group.ImplicitCommand is not null)
			return ParseCommand(context, group.ImplicitCommand, null);

		TextLocation location = new(context.Parser.Point, context.Parser.Point);

		if (groupCommandError is null && group.Parent is not null)
			context.Diagnostics.Add(DiagnosticSource.Parsing, location, "There is nothing that can possibly be parsed because nothing was registered in this command group.");

		context.Diagnostics.Add(DiagnosticSource.Parsing, location, $"Expected the {groupCommandError} name.");

		return new GroupParseResult(group, null, [], null);
	}
	private ICommandParseResult ParseCommand(Context context, ICommandInfo command, TextToken? nameToken)
	{
		if (nameToken is not null)
			nameToken = new(TextTokenKind.CommandName, nameToken.Value.Location, nameToken.Value.Value);

		List<IFlagParseResult> flags = [];
		List<IArgumentParseResult> arguments = [];

		foreach (IArgumentInfo argumentInfo in command.Arguments)
		{
			if (TryParseArgument(context, argumentInfo, out IArgumentParseResult? result) is false)
				break;

			arguments.Add(result);
		}

		return new CommandParseResult(command, nameToken, flags, arguments);
	}
	private bool TryParseArgument(Context context, IArgumentInfo argument, [NotNullWhen(true)] out IArgumentParseResult? result)
	{
		TextLocation location = new(context.Parser.Point, context.Parser.Point);

		ArgumentValueParseContext argumentContext = new(this, argument);
		IValueParseResult value = argument.Parser.Parse(argumentContext, context.Parser);

		if (value.Error is null)
		{
			context.Parser.SkipTrivia();

			result = new ArgumentParseResult(argument, value);
			return true;
		}

		result = default;

		if (argument.IsRequired)
		{
			if (value.Error == string.Empty)
				context.Diagnostics.Add(DiagnosticSource.Parsing, location, $"Expected value for the '{argument.Name}' argument.");
			else
				context.Diagnostics.Add(DiagnosticSource.Parsing, value.Location, value.Error);
		}
		else if (value.Error != string.Empty)
			context.Diagnostics.Add(DiagnosticSource.Parsing, value.Location, value.Error);

		return false;
	}
	#endregion

	#region Helpers
	private static bool TryParseName(TextParser parser, [NotNullWhen(true)] out TextToken? token, [NotNullWhen(true)] out string? name)
	{
		TextPoint start = parser.Point;
		name = parser.AdvanceUntilBreak();
		TextPoint end = parser.Point;

		if (string.IsNullOrEmpty(name))
		{
			token = default;
			name = default;

			return false;
		}

		parser.SkipTrivia();

		token = new(TextTokenKind.GroupName, new(start, end), name);
		return true;
	}
	#endregion
}
