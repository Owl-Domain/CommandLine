namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents a parser for commands.
/// </summary>
public sealed class CommandParser : BaseCommandParser
{
	#region Nested types
	private sealed class Context(ICommandEngine engine, ITextParser parser)
	{
		#region Properties
		public ICommandEngine Engine { get; } = engine;
		public ITextParser Parser { get; } = parser;
		public DiagnosticBag Diagnostics { get; } = [];
		public List<TextToken> ExtraTokens { get; } = [];
		#endregion
	}
	#endregion

	#region Methods
	/// <inheritdoc/>
	protected override ICommandParserResult Parse(ICommandEngine engine, ITextParser parser)
	{
		Stopwatch watch = Stopwatch.StartNew();
		Context context = new(engine, parser);
		IParseResult? result = ParseGroup(context, engine.RootGroup);

		CheckForLeftOverInput(context);

		watch.Stop();
		return new CommandParserResult(context.Diagnostics.Any() is false, context.Engine, this, context.Diagnostics, result, context.ExtraTokens, watch.Elapsed);
	}
	private IParseResult ParseGroup(Context context, ICommandGroupInfo group)
	{
		string? groupCommandError = (group.Groups.Any(), group.Commands.Any()) switch
		{
			(true, true) => "group/command",
			(true, false) => "group",
			(false, true) => "command",
			(false, false) => null,
		};

		TextPoint start = context.Parser.Point;
		TextToken? foundName = null;

		if (groupCommandError is not null)
		{
			int fragmentIndex = context.Parser.CurrentFragment.Index, offset = context.Parser.Offset;

			if (TryParseName(context.Parser, out TextToken? nameToken, out string? name))
			{
				foundName = nameToken;
				if (group.Groups.TryGetValue(name, out ICommandGroupInfo? childGroup))
				{
					IParseResult subResult = ParseGroup(context, childGroup);
					GroupParseResult groupResult = new(childGroup, nameToken, [], subResult);

					return groupResult;
				}

				if (group.Commands.TryGetValue(name, out ICommandInfo? command))
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

		TextLocation location = new(start, context.Parser.Point);

		if (groupCommandError is null && group.Parent is not null)
			context.Diagnostics.Add(DiagnosticSource.Parsing, location, "There is nothing that can possibly be parsed because nothing was registered in this command group.");

		if (foundName is not null)
			context.Diagnostics.Add(DiagnosticSource.Parsing, foundName.Value.Location, $"'{foundName.Value.Value}' is not a known {groupCommandError} name.");
		else
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

		ArgumentValueParseContext argumentContext = new(context.Engine, argument);
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
	private static bool TryParseName(ITextParser parser, [NotNullWhen(true)] out TextToken? token, [NotNullWhen(true)] out string? name)
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
