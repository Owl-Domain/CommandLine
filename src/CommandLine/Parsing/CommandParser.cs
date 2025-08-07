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
	private static IParseResult ParseGroup(Context context, ICommandGroupInfo group)
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

			if (TryParseName(context.Parser, out TextToken nameToken, out string? name))
			{
				foundName = nameToken;
				if (group.Groups.TryGetValue(name, out ICommandGroupInfo? childGroup))
				{
					List<IFlagParseResult> flags = [];
					TryParseFlags(context, group.SharedFlags, flags);

					IParseResult subResult = ParseGroup(context, childGroup);
					GroupParseResult groupResult = new(childGroup, nameToken, flags, subResult);

					return groupResult;
				}

				if (group.Commands.TryGetValue(name, out ICommandInfo? command))
				{
					ICommandParseResult subCommand = ParseCommand(context, command, nameToken);
					return subCommand;
				}
			}

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
	private static ICommandParseResult ParseCommand(Context context, ICommandInfo command, TextToken? nameToken)
	{
		if (nameToken is not null)
			nameToken = new(TextTokenKind.CommandName, nameToken.Value.Location, nameToken.Value.Value);

		List<IFlagParseResult> flags = [];
		List<IArgumentParseResult> arguments = [];

		if (command.Arguments.Count is 0)
			TryParseFlags(context, command.Flags, flags);

		foreach (IArgumentInfo argumentInfo in command.Arguments)
		{
			TryParseFlags(context, command.Flags, flags);

			if (TryParseArgument(context, argumentInfo, out IArgumentParseResult? result) is false)
				break;

			arguments.Add(result);
		}

		return new CommandParseResult(command, nameToken, flags, arguments);
	}
	private static bool TryParseArgument(Context context, IArgumentInfo argument, [NotNullWhen(true)] out IArgumentParseResult? result)
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
	private static void TryParseFlags(Context context, IReadOnlyCollection<IFlagInfo> availableFlags, List<IFlagParseResult> container)
	{
		do
		{
			int fragmentIndex = context.Parser.CurrentFragment.Index, offset = context.Parser.Offset;
			if (TryParseFlag(context, availableFlags, out IFlagParseResult? flag) is false)
			{
				context.Parser.Restore(fragmentIndex, offset);
				break;
			}

			container.Add(flag);
			context.Parser.SkipTrivia();
		}
		while (true);
	}
	private static bool TryParseFlag(Context context, IReadOnlyCollection<IFlagInfo> availableFlags, [NotNullWhen(true)] out IFlagParseResult? result)
	{
		if (context.Parser.Current is '-' && context.Parser.Next is '-')
			return TryParseLongFlag(context, availableFlags, out result);

		if (context.Parser.Current is '-')
			return TryParseShortFlag(context, availableFlags, out result);

		result = default;
		return false;
	}
	private static bool TryParseLongFlag(Context context, IReadOnlyCollection<IFlagInfo> availableFlags, [NotNullWhen(true)] out IFlagParseResult? result)
	{
		TextPoint start = context.Parser.Point;

		Debug.Assert(context.Parser.Current is '-' && context.Parser.Next is '-');
		context.Parser.Advance(2);

		TextToken prefix = new(TextTokenKind.Symbol, new(start, context.Parser.Point), "--");

		result = default;

		if (TryParseFlagName(context.Parser, out TextToken nameToken, out string? name) is false)
		{
			context.Diagnostics.Add(DiagnosticSource.Parsing, new(context.Parser.Point, context.Parser.Point), "Couldn't parse the flag name.");
			return false;
		}

		IFlagInfo? flag = availableFlags.SingleOrDefault(f => f.LongName == name);

		if (flag is null)
		{
			context.Diagnostics.Add(DiagnosticSource.Parsing, nameToken.Location, $"Couldn't find a flag with the long name '{name}'.");
			return false;
		}

		if (TryParseFlagValue(context, prefix, nameToken, flag, out result))
			return true;

		context.Diagnostics.Add(DiagnosticSource.Parsing, nameToken.Location, $"Couldn't parse the value for the '--{name}' flag.");
		return false;
	}
	private static bool TryParseShortFlag(Context context, IReadOnlyCollection<IFlagInfo> availableFlags, [NotNullWhen(true)] out IFlagParseResult? result)
	{
		TextPoint start = context.Parser.Point;

		Debug.Assert(context.Parser.Current is '-');
		context.Parser.Advance();

		TextToken prefix = new(TextTokenKind.Symbol, new(start, context.Parser.Point), "-");

		result = default;

		if (TryParseFlagName(context.Parser, out TextToken nameToken, out string? name) is false)
		{
			context.Diagnostics.Add(DiagnosticSource.Parsing, new(context.Parser.Point, context.Parser.Point), "Couldn't parse the flag name.");
			return false;
		}

		Debug.Assert(name.Length > 0);

		if (name.Length is 1)
			return TryParseSimpleFlag(context, availableFlags, prefix, nameToken, name, out result);

		if (AllTheSame(name))
			return TryParseRepeatFlag(context, availableFlags, prefix, nameToken, name, out result);

		if (AllUnique(name))
			return TryParseChainFlag(context, availableFlags, prefix, nameToken, name, out result);

		return false;
	}
	private static bool TryParseSimpleFlag(
		Context context,
		IReadOnlyCollection<IFlagInfo> availableFlags,
		TextToken prefix,
		TextToken nameToken,
		string name,
		[NotNullWhen(true)] out IFlagParseResult? result)
	{
		IFlagInfo? flag = availableFlags.SingleOrDefault(f => f.ShortName == name[0]);
		if (flag is null)
		{
			context.Diagnostics.Add(DiagnosticSource.Parsing, nameToken.Location, $"Couldn't find a flag with the short name '{name[0]}'.");

			result = default;
			return false;
		}

		if (TryParseFlagValue(context, prefix, nameToken, flag, out result))
			return true;

		context.Diagnostics.Add(DiagnosticSource.Parsing, nameToken.Location, $"Couldn't parse the value for the '-{name[0]}' flag.");

		result = default;
		return false;
	}
	private static bool TryParseChainFlag(
		Context context,
		IReadOnlyCollection<IFlagInfo> availableFlags,
		TextToken prefix,
		TextToken nameToken,
		string name,
		[NotNullWhen(true)] out IFlagParseResult? result)
	{
		List<IFlagInfo> flags = [];
		bool successful = true;

		for (int i = 0; i < name.Length; i++)
		{
			IFlagInfo? flag = availableFlags.SingleOrDefault(f => f.ShortName == name[i]);

			if (flag is null)
			{
				Debug.Assert(nameToken.Location.Start.Fragment == nameToken.Location.End.Fragment);

				TextLocation location = new(nameToken.Location.Start, new(nameToken.Location.Start.Fragment, nameToken.Location.Start.Offset + i + 1));
				context.Diagnostics.Add(DiagnosticSource.Parsing, location, $"The flag -{name[i]} was not recognised.");

				successful = false;
				continue;
			}

			if (flag.Kind is not FlagKind.Repeat and not FlagKind.Toggle)
			{
				Debug.Assert(nameToken.Location.Start.Fragment == nameToken.Location.End.Fragment);

				TextLocation location = new(nameToken.Location.Start, new(nameToken.Location.Start.Fragment, nameToken.Location.Start.Offset + i + 1));
				context.Diagnostics.Add(DiagnosticSource.Parsing, location, $"The flag -{name[i]} was not a repeat or toggle flag and it must specify a value.");

				successful = false;
				continue;
			}

			flags.Add(flag);
		}

		if (successful is false)
		{
			result = default;
			return false;
		}

		result = new ChainFlagParseResult(flags, prefix, nameToken);
		return true;
	}
	private static bool TryParseRepeatFlag(Context context,
		IReadOnlyCollection<IFlagInfo> availableFlags,
		TextToken prefix,
		TextToken nameToken,
		string name,
		[NotNullWhen(true)] out IFlagParseResult? result)
	{
		IFlagInfo? flag = availableFlags.SingleOrDefault(f => f.ShortName == name[0]);
		if (flag is null)
		{
			context.Diagnostics.Add(DiagnosticSource.Parsing, nameToken.Location, $"The flag -{name[0]} was not recognised.");

			result = default;
			return false;
		}

		if (flag.Kind is not FlagKind.Repeat)
		{
			context.Diagnostics.Add(DiagnosticSource.Parsing, nameToken.Location, $"The flag -{name[0]} was not a repeat style flag and doesn't support this syntax.");

			result = default;
			return false;
		}

		result = new RepeatFlagParseResult(flag, prefix, nameToken, name.Length);
		return true;
	}
	private static bool TryParseFlagValue(
		Context context,
		TextToken prefix,
		TextToken name,
		IFlagInfo flag,
		[NotNullWhen(true)] out IFlagParseResult? result)
	{
		if (TryParseFlagValueSeparator(context, flag, out TextToken? separator) is false)
		{
			if (flag.Kind is FlagKind.Toggle)
			{
				result = new ToggleFlagParseResult(flag, prefix, name);
				return true;
			}

			if (flag.Kind is FlagKind.Repeat)
			{
				result = new RepeatFlagParseResult(flag, prefix, name, 1);
				return true;
			}

			result = default;
			return false;
		}

		TextLocation location = new(context.Parser.Point, context.Parser.Point);

		FlagValueParseContext flagContext = new(context.Engine, flag);
		IValueParseResult value = flag.Parser.Parse(flagContext, context.Parser);

		if (value.Error is null)
		{
			context.Parser.SkipTrivia();

			result = new ValueFlagParseResult(flag, prefix, name, separator, value);
			return true;
		}

		result = default;

		if (flag.IsRequired)
		{
			Debug.Assert(prefix.Value is not null);

			if (value.Error == string.Empty)
				context.Diagnostics.Add(DiagnosticSource.Parsing, location, $"Expected value for the '{prefix.Value}{name.Value}' flag.");
			else
				context.Diagnostics.Add(DiagnosticSource.Parsing, value.Location, value.Error);
		}
		else if (value.Error != string.Empty)
			context.Diagnostics.Add(DiagnosticSource.Parsing, value.Location, value.Error);

		return false;
	}
	private static bool TryParseFlagValueSeparator(Context context, IFlagInfo flag, out TextToken? separator)
	{
		separator = default;
		TextPoint start = context.Parser.Point;

		if (context.Parser.Current is '=' or ':')
		{
			string text = context.Parser.Current.ToString();

			context.Parser.Advance();
			TextPoint end = context.Parser.Point;
			context.Parser.SkipTrivia();

			separator = new(TextTokenKind.Symbol, new(start, end), text);
			return true;
		}

		if (char.IsWhiteSpace(context.Parser.Current) || ((context.Parser.IsLazy is false) && context.Parser.IsAtEnd))
		{
			if (flag.Kind is FlagKind.Toggle or FlagKind.Repeat)
				return false;

			context.Parser.SkipTrivia();
			return true;
		}

		if (flag.Kind is FlagKind.Regular)
			context.Diagnostics.Add(DiagnosticSource.Parsing, new(start, start), $"Unknown flag value separator, use either '=' or ':' symbols.");

		return false;
	}
	#endregion

	#region Helpers
	private static bool AllTheSame(string name)
	{
		Debug.Assert(name.Length > 0);

		char first = name[0];

		for (int i = 1; i < name.Length; i++)
		{
			if (first != name[i])
				return false;
		}

		return true;
	}
	private static bool AllUnique(string name)
	{
		HashSet<char> chars = [];

		foreach (char ch in name)
		{
			if (chars.Add(ch) is false)
				return false;
		}

		return true;
	}
	private static void CheckForLeftOverInput(Context context)
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
	private static bool TryParseFlagName(ITextParser parser, out TextToken token, [NotNullWhen(true)] out string? name)
	{
		TextPoint start = parser.Point;
		name = parser.AdvanceWhile(c => c is '-' or '_' || char.IsLetterOrDigit(c));
		TextPoint end = parser.Point;

		if (string.IsNullOrEmpty(name))
		{
			token = default;
			name = default;

			return false;
		}

		token = new(TextTokenKind.FlagName, new(start, end), name);
		return true;
	}
	private static bool TryParseName(ITextParser parser, out TextToken token, [NotNullWhen(true)] out string? name)
	{
		TextPoint start = parser.Point;
		name = parser.AdvanceWhile(c => c is '-' or '_' || char.IsLetterOrDigit(c));
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
