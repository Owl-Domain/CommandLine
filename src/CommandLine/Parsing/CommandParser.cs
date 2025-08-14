namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents a parser for commands.
/// </summary>
public sealed class CommandParser : BaseCommandParser
{
	#region Nested types
	private readonly struct Context(ICommandEngine engine, ITextParser parser, CancellationToken cancellationToken)
	{
		#region Properties
		public ICommandEngine Engine { get; } = engine;
		public ITextParser Parser { get; } = parser;
		public CancellationToken CancellationToken { get; } = cancellationToken;
		public DiagnosticBag Diagnostics { get; } = [];
		public List<TextToken> ExtraTokens { get; } = [];
		#endregion
	}
	private readonly struct FlagContext(Context context, IReadOnlyCollection<IFlagInfo> availableFlags)
	{
		#region Properties
		public ICommandEngine Engine { get; } = context.Engine;
		public ITextParser Parser { get; } = context.Parser;
		public CancellationToken CancellationToken { get; } = context.CancellationToken;
		public DiagnosticBag Diagnostics { get; } = context.Diagnostics;
		public List<TextToken> ExtraTokens { get; } = context.ExtraTokens;
		public IReadOnlyCollection<IFlagInfo> AvailableFlags { get; } = availableFlags;
		#endregion
	}
	private readonly struct FlagPrefixContext(FlagContext context, TextToken prefix)
	{
		#region Properties
		public ICommandEngine Engine { get; } = context.Engine;
		public ITextParser Parser { get; } = context.Parser;
		public CancellationToken CancellationToken { get; } = context.CancellationToken;
		public DiagnosticBag Diagnostics { get; } = context.Diagnostics;
		public List<TextToken> ExtraTokens { get; } = context.ExtraTokens;
		public IReadOnlyCollection<IFlagInfo> AvailableFlags { get; } = context.AvailableFlags;
		public TextToken Prefix { get; } = prefix;
		#endregion
	}
	private readonly struct NamedFlagContext(FlagPrefixContext context, TextToken nameToken, string name)
	{
		#region Properties
		public ICommandEngine Engine { get; } = context.Engine;
		public ITextParser Parser { get; } = context.Parser;
		public CancellationToken CancellationToken { get; } = context.CancellationToken;
		public DiagnosticBag Diagnostics { get; } = context.Diagnostics;
		public List<TextToken> ExtraTokens { get; } = context.ExtraTokens;
		public IReadOnlyCollection<IFlagInfo> AvailableFlags { get; } = context.AvailableFlags;
		public TextToken Prefix { get; } = context.Prefix;
		public TextToken NameToken { get; } = nameToken;
		public string Name { get; } = name;
		#endregion
	}
	private readonly struct SingleFlagContext(NamedFlagContext context, IFlagInfo flag)
	{
		#region Properties
		public ICommandEngine Engine { get; } = context.Engine;
		public ITextParser Parser { get; } = context.Parser;
		public CancellationToken CancellationToken { get; } = context.CancellationToken;
		public DiagnosticBag Diagnostics { get; } = context.Diagnostics;
		public List<TextToken> ExtraTokens { get; } = context.ExtraTokens;
		public IReadOnlyCollection<IFlagInfo> AvailableFlags { get; } = context.AvailableFlags;
		public TextToken Prefix { get; } = context.Prefix;
		public TextToken NameToken { get; } = context.NameToken;
		public string Name { get; } = context.Name;
		public IFlagInfo Flag { get; } = flag;
		#endregion
	}
	#endregion

	#region Methods
	/// <inheritdoc/>
	protected override ICommandParserResult Parse(ICommandEngine engine, ITextParser parser, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		Stopwatch watch = Stopwatch.StartNew();
		Context context = new(engine, parser, cancellationToken);

		IParseResult? result = ParseGroup(context, engine.RootGroup, false);

		CheckForLeftOverInput(context);

		watch.Stop();
		return new CommandParserResult(context.Diagnostics.Count is 0, false, context.Engine, this, context.Diagnostics, result, context.ExtraTokens, watch.Elapsed);
	}
	private static IParseResult ParseGroup(Context context, ICommandGroupInfo group, bool flagArgumentSeparatorReached)
	{
		context.CancellationToken.ThrowIfCancellationRequested();

		string? groupCommandError = (group.Groups.Any(), group.Commands.Any()) switch
		{
			(true, true) => "group/command",
			(true, false) => "group",
			(false, true) => "command",
			(false, false) => null,
		};

		TextPoint start = context.Parser.Point;
		TextToken? foundName = null;

		if (flagArgumentSeparatorReached is false)
		{
			if (IsFlagArgumentSeparator(context.Engine, context.Parser, out TextToken token))
			{
				context.ExtraTokens.Add(token);
				flagArgumentSeparatorReached = true;
			}
		}

		FlagContext flagContext = new(context, group.SharedFlags);
		List<IFlagParseResult> groupFlags = [];

		if (group.SharedFlags.Count > 0)
			TryParseFlags(flagContext, groupFlags, ref flagArgumentSeparatorReached);

		if (flagArgumentSeparatorReached is false && groupCommandError is not null)
		{
			RestorePoint restorePoint = context.Parser.GetRestorePoint();

			if (TryParseName(context.Parser, out TextToken nameToken, out string? name))
			{
				foundName = nameToken;
				if (group.Groups.TryGetValue(name, out ICommandGroupInfo? childGroup))
				{
					List<IFlagParseResult> flags = [];

					TryParseFlags(new(context, group.SharedFlags), flags, ref flagArgumentSeparatorReached);

					IParseResult subResult = ParseGroup(context, childGroup, flagArgumentSeparatorReached);
					GroupParseResult groupResult = new(childGroup, nameToken, flags, subResult);

					return groupResult;
				}

				if (group.Commands.TryGetValue(name, out ICommandInfo? command))
				{
					ICommandParseResult subCommand = ParseCommand(context, command, nameToken, ref flagArgumentSeparatorReached);
					return subCommand;
				}
			}

			context.Parser.Restore(restorePoint);
		}

		if (group.SharedFlags.Count > 0)
			TryParseFlags(flagContext, groupFlags, ref flagArgumentSeparatorReached);

		if (group.ImplicitCommand is not null)
		{
			ICommandParseResult command = ParseCommand(context, group.ImplicitCommand, null, ref flagArgumentSeparatorReached);
			if (groupFlags.Count is 0)
				return command;

			return new GroupParseResult(group, null, groupFlags, command);
		}

		if (groupFlags.Count > 0)
			return new GroupParseResult(group, null, groupFlags, null);

		TextLocation location = new(start, context.Parser.Point);

		if (groupCommandError is null && group.Parent is not null)
			context.Diagnostics.Add(DiagnosticSource.Parsing, location, "There is nothing that can possibly be parsed because nothing was registered in this command group.");

		if (foundName is not null)
			context.Diagnostics.Add(DiagnosticSource.Parsing, foundName.Value.Location, $"'{foundName.Value.Value}' is not a known {groupCommandError} name.");
		else
			context.Diagnostics.Add(DiagnosticSource.Parsing, location, $"Expected the {groupCommandError} name.");

		return new GroupParseResult(group, null, [], null);
	}
	private static ICommandParseResult ParseCommand(Context context, ICommandInfo command, TextToken? nameToken, ref bool flagArgumentSeparatorReached)
	{
		context.CancellationToken.ThrowIfCancellationRequested();

		if (nameToken is not null)
			nameToken = new(TextTokenKind.CommandName, nameToken.Value.Location, nameToken.Value.Value);

		List<IFlagParseResult> flags = [];
		List<IArgumentParseResult> arguments = [];

		if (command.Arguments.Count is 0)
			TryParseFlags(new(context, command.Flags), flags, ref flagArgumentSeparatorReached);

		foreach (IArgumentInfo argumentInfo in command.Arguments)
		{
			TryParseFlags(new(context, command.Flags), flags, ref flagArgumentSeparatorReached);

			if (TryParseArgument(context, argumentInfo, out IArgumentParseResult? result) is false)
				break;

			arguments.Add(result);
		}

		return new CommandParseResult(command, nameToken, flags, arguments);
	}
	private static bool TryParseArgument(Context context, IArgumentInfo argument, [NotNullWhen(true)] out IArgumentParseResult? result)
	{
		context.CancellationToken.ThrowIfCancellationRequested();

		TextLocation location = new(context.Parser.Point, context.Parser.Point);

		ArgumentValueParseContext argumentContext = new(context.Engine, argument, context.CancellationToken);
		IValueParseResult value = argument.Parser.Parse(argumentContext, context.Parser);

		if (value.Successful)
		{
			context.Parser.SkipTrivia();

			result = new ArgumentParseResult(argument, value);
			return true;
		}

		Debug.Assert(value.Error is not null);
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

	#region Flag methods
	private static void TryParseFlags(FlagContext context, List<IFlagParseResult> container, ref bool flagArgumentSeparatorReached)
	{
		if (flagArgumentSeparatorReached)
			return;

		do
		{
			if (IsFlagArgumentSeparator(context.Engine, context.Parser, out TextToken separator))
			{
				flagArgumentSeparatorReached = true;
				context.ExtraTokens.Add(separator);

				return;
			}

			context.CancellationToken.ThrowIfCancellationRequested();

			RestorePoint restorePoint = context.Parser.GetRestorePoint();
			if (TryParseFlag(context, out IFlagParseResult? flag) is false)
			{
				context.Parser.Restore(restorePoint);
				break;
			}

			container.Add(flag);
			context.Parser.SkipTrivia();
		}
		while (true);
	}
	private static bool TryParseFlag(FlagContext context, [NotNullWhen(true)] out IFlagParseResult? result)
	{
		context.CancellationToken.ThrowIfCancellationRequested();

		if (context.Engine.Settings.MergeLongAndShortFlags)
			return TryParseMergedFlag(context, out result);

		string longPrefix = context.Engine.Settings.LongFlagPrefix;
		string shortPrefix = context.Engine.Settings.ShortFlagPrefix;

		if (longPrefix.Length >= shortPrefix.Length)
		{
			if (context.Parser.Match(longPrefix, TextTokenKind.Symbol, out TextToken longToken))
				return TryParseLongFlag(new(context, longToken), out result);

			if (context.Parser.Match(shortPrefix, TextTokenKind.Symbol, out TextToken shortToken))
				return TryParseShortFlag(new(context, shortToken), out result);
		}
		else
		{
			if (context.Parser.Match(shortPrefix, TextTokenKind.Symbol, out TextToken shortToken))
				return TryParseShortFlag(new(context, shortToken), out result);

			if (context.Parser.Match(longPrefix, TextTokenKind.Symbol, out TextToken longToken))
				return TryParseLongFlag(new(context, longToken), out result);
		}

		result = default;
		return false;
	}
	private static bool TryParseMergedFlag(FlagContext context, [NotNullWhen(true)] out IFlagParseResult? result)
	{
		context.CancellationToken.ThrowIfCancellationRequested();
		string prefix = context.Engine.Settings.LongFlagPrefix;

		result = default;

		if (context.Parser.Match(prefix, TextTokenKind.Symbol, out TextToken prefixToken) is false)
			return false;

		if (TryParseFlagName(context.Engine, context.Parser, out TextToken nameToken, out string? name) is false)
		{
			context.Diagnostics.Add(DiagnosticSource.Parsing, new(context.Parser.Point, context.Parser.Point), "Couldn't parse the flag name.");
			return false;
		}

		IFlagInfo? flag = context.AvailableFlags.SingleOrDefault(f => f.LongName == name || f.ShortName == name[0]);
		if (flag is null)
		{
			context.Diagnostics.Add(DiagnosticSource.Parsing, nameToken.Location, $"Couldn't find the {prefix}{name} flag.");
			return false;
		}

		FlagPrefixContext prefixContext = new(context, prefixToken);
		NamedFlagContext namedContext = new(prefixContext, nameToken, name);

		if (TryParseFlagValue(new(namedContext, flag), out result))
			return true;

		context.Diagnostics.Add(DiagnosticSource.Parsing, nameToken.Location, $"Couldn't parse the value for the {prefix}{name} flag.");
		return false;
	}
	private static bool TryParseLongFlag(FlagPrefixContext context, [NotNullWhen(true)] out IFlagParseResult? result)
	{
		context.CancellationToken.ThrowIfCancellationRequested();
		string longPrefix = context.Engine.Settings.LongFlagPrefix;

		result = default;

		if (TryParseFlagName(context.Engine, context.Parser, out TextToken nameToken, out string? name) is false)
		{
			context.Diagnostics.Add(DiagnosticSource.Parsing, new(context.Parser.Point, context.Parser.Point), "Couldn't parse the flag name.");
			return false;
		}

		IFlagInfo? flag = context.AvailableFlags.SingleOrDefault(f => f.LongName == name);
		if (flag is null)
		{
			context.Diagnostics.Add(DiagnosticSource.Parsing, nameToken.Location, $"Couldn't find the {longPrefix}{name} flag.");
			return false;
		}

		if (TryParseFlagValue(new(new(context, nameToken, name), flag), out result))
			return true;

		context.Diagnostics.Add(DiagnosticSource.Parsing, nameToken.Location, $"Couldn't parse the value for the {longPrefix}{name} flag.");
		return false;
	}
	private static bool TryParseShortFlag(FlagPrefixContext context, [NotNullWhen(true)] out IFlagParseResult? result)
	{
		context.CancellationToken.ThrowIfCancellationRequested();
		result = default;

		if (TryParseFlagName(context.Engine, context.Parser, out TextToken nameToken, out string? name) is false)
		{
			context.Diagnostics.Add(DiagnosticSource.Parsing, new(context.Parser.Point, context.Parser.Point), "Couldn't parse the flag name.");
			return false;
		}

		Debug.Assert(name.Length > 0);

		NamedFlagContext newContext = new(context, nameToken, name);

		if (name.Length is 1)
			return TryParseSimpleFlag(newContext, out result);

		if (AllTheSame(name))
			return TryParseRepeatFlag(newContext, out result);

		if (AllUnique(name))
			return TryParseChainFlag(newContext, out result);

		return false;
	}
	private static bool TryParseSimpleFlag(NamedFlagContext context, [NotNullWhen(true)] out IFlagParseResult? result)
	{
		context.CancellationToken.ThrowIfCancellationRequested();
		string shortPrefix = context.Engine.Settings.ShortFlagPrefix;

		IFlagInfo? flag = context.AvailableFlags.SingleOrDefault(f => f.ShortName == context.Name[0]);
		if (flag is null)
		{
			context.Diagnostics.Add(DiagnosticSource.Parsing, context.NameToken.Location, $"Couldn't find a {shortPrefix}{context.Name[0]} flag.");

			result = default;
			return false;
		}

		if (TryParseFlagValue(new(context, flag), out result))
			return true;

		context.Diagnostics.Add(DiagnosticSource.Parsing, context.NameToken.Location, $"Couldn't parse the value for the {shortPrefix}{context.Name[0]} flag.");

		result = default;
		return false;
	}
	private static bool TryParseChainFlag(NamedFlagContext context, [NotNullWhen(true)] out IFlagParseResult? result)
	{
		context.CancellationToken.ThrowIfCancellationRequested();
		string shortPrefix = context.Engine.Settings.ShortFlagPrefix;

		List<IFlagInfo> flags = [];
		bool successful = true;

		for (int i = 0; i < context.Name.Length; i++)
		{
			IFlagInfo? flag = context.AvailableFlags.SingleOrDefault(f => f.ShortName == context.Name[i]);

			if (flag is null)
			{
				Debug.Assert(context.NameToken.Location.Start.Fragment == context.NameToken.Location.End.Fragment);

				TextLocation location = new(context.NameToken.Location.Start, new(context.NameToken.Location.Start.Fragment, context.NameToken.Location.Start.Offset + i + 1));
				context.Diagnostics.Add(DiagnosticSource.Parsing, location, $"The flag {shortPrefix}{context.Name[i]} was not recognised.");

				successful = false;
				continue;
			}

			if (flag.Kind is not FlagKind.Repeat and not FlagKind.Toggle)
			{
				Debug.Assert(context.NameToken.Location.Start.Fragment == context.NameToken.Location.End.Fragment);

				TextLocation location = new(context.NameToken.Location.Start, new(context.NameToken.Location.Start.Fragment, context.NameToken.Location.Start.Offset + i + 1));
				context.Diagnostics.Add(DiagnosticSource.Parsing, location, $"The flag {shortPrefix}{context.Name[i]} was not a repeat or toggle flag and it must specify a value.");

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

		result = new ChainFlagParseResult(flags, context.Prefix, context.NameToken);
		return true;
	}
	private static bool TryParseRepeatFlag(NamedFlagContext context, [NotNullWhen(true)] out IFlagParseResult? result)
	{
		context.CancellationToken.ThrowIfCancellationRequested();
		string shortPrefix = context.Engine.Settings.ShortFlagPrefix;

		IFlagInfo? flag = context.AvailableFlags.SingleOrDefault(f => f.ShortName == context.Name[0]);
		if (flag is null)
		{
			context.Diagnostics.Add(DiagnosticSource.Parsing, context.NameToken.Location, $"The flag {shortPrefix}{context.Name[0]} was not recognised.");

			result = default;
			return false;
		}

		if (flag.Kind is not FlagKind.Repeat)
		{
			context.Diagnostics.Add(DiagnosticSource.Parsing, context.NameToken.Location, $"The flag {shortPrefix}{context.Name[0]} was not a repeat style flag and doesn't support this syntax.");

			result = default;
			return false;
		}

		result = new RepeatFlagParseResult(flag, context.Prefix, context.NameToken, context.Name.Length);
		return true;
	}
	private static bool TryParseFlagValue(SingleFlagContext context, [NotNullWhen(true)] out IFlagParseResult? result)
	{
		context.CancellationToken.ThrowIfCancellationRequested();

		if (TryParseFlagValueSeparator(context, out TextToken? separator) is false)
		{
			if (context.Flag.Kind is FlagKind.Toggle)
			{
				result = new ToggleFlagParseResult(context.Flag, context.Prefix, context.NameToken);
				return true;
			}

			if (context.Flag.Kind is FlagKind.Repeat)
			{
				result = new RepeatFlagParseResult(context.Flag, context.Prefix, context.NameToken, 1);
				return true;
			}

			result = default;
			return false;
		}

		TextLocation location = new(context.Parser.Point, context.Parser.Point);

		FlagValueParseContext flagContext = new(context.Engine, context.Flag, context.CancellationToken);
		IValueParseResult value = context.Flag.Parser.Parse(flagContext, context.Parser);

		if (value.Error is null)
		{
			context.Parser.SkipTrivia();

			result = new ValueFlagParseResult(context.Flag, context.Prefix, context.NameToken, separator, value);
			return true;
		}

		result = default;

		if (context.Flag.IsRequired)
		{
			Debug.Assert(context.Prefix.Value is not null);

			if (value.Error == string.Empty)
				context.Diagnostics.Add(DiagnosticSource.Parsing, location, $"Expected value for the {context.Prefix.Value}{context.Name} flag.");
			else
				context.Diagnostics.Add(DiagnosticSource.Parsing, value.Location, value.Error);
		}
		else if (value.Error != string.Empty)
			context.Diagnostics.Add(DiagnosticSource.Parsing, value.Location, value.Error);

		return false;
	}
	private static bool TryParseFlagValueSeparator(SingleFlagContext context, out TextToken? separator)
	{
		context.CancellationToken.ThrowIfCancellationRequested();

		// Todo(Nightowl): Add flag value separator setting;
		if (context.Parser.MatchAny(context.Engine.Settings.FlagValueSeparators.Where(s => s is not " "), TextTokenKind.Symbol, out TextToken sep))
		{
			context.Parser.SkipTrivia();
			separator = sep;
			return true;
		}

		bool allowsWhitespace = context.Engine.Settings.FlagValueSeparators.Contains(" ");

		if (allowsWhitespace && (char.IsWhiteSpace(context.Parser.Current) || (context.Parser.IsGreedy && context.Parser.IsAtEnd)))
		{
			if (context.Flag.Kind is FlagKind.Toggle or FlagKind.Repeat)
			{
				separator = default;
				return false;
			}

			context.Parser.SkipTrivia();

			separator = null;
			return true;
		}

		if (context.Flag.Kind is FlagKind.Regular)
		{
			TextLocation location = new(context.Parser.Point, context.Parser.Point);

			IReadOnlyList<string> seps = [.. context.Engine.Settings.FlagValueSeparators.Select(sep => sep is " " ? "whitespace" : $"'{sep}'")];

			string error = "Unknown flag value separator, use ";

			error += seps.Count switch
			{
				1 => $"the {seps[0]} symbol.",
				2 => $"either the {seps[0]} or {seps[1]} symbols.",
				_ => $"either the {string.Join(", ", seps.Take(seps.Count - 1))} or the {seps[seps.Count - 1]} symbols."
			};

			context.Diagnostics.Add(DiagnosticSource.Parsing, location, error);
		}

		separator = default;
		return false;
	}
	#endregion

	#region Helpers
	private static bool IsFlagArgumentSeparator(ICommandEngine engine, ITextParser parser, out TextToken token)
	{
		string separator = engine.Settings.FlagArgumentSeparator;

		if (parser.TextUntilBreak.Equals(separator, StringComparison.OrdinalIgnoreCase))
		{
			TextPoint start = parser.Point;

			parser.Advance(separator.Length);
			TextPoint end = parser.Point;

			parser.SkipTrivia();

			token = new(TextTokenKind.Symbol, new(start, end), separator);
			return true;
		}

		token = default;
		return false;
	}
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
		context.CancellationToken.ThrowIfCancellationRequested();

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
	private static bool TryParseFlagName(ICommandEngine engine, ITextParser parser, out TextToken token, [NotNullWhen(true)] out string? name)
	{
		bool oldIsLazy = parser.IsLazy;
		try
		{
			parser.IsLazy = true;
			using (parser.WithBreakCharacters([.. engine.Settings.FlagValueSeparators.Select(s => s[0])]))
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

				token = new(TextTokenKind.FlagName, new(start, end), name);
				return true;
			}
		}
		finally
		{
			parser.IsLazy = oldIsLazy;
		}
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
