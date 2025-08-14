using OwlDomain.CommandLine.Parsing.Values.Collections;
using OwlDomain.CommandLine.Parsing.Values.Primitives;

namespace OwlDomain.CommandLine.Tests.Parsing;

[TestClass]
public sealed class CommandParserTests
{
	#region Tests
	[DataRow(true, DisplayName = "Lazy")]
	[DataRow(false, DisplayName = "Greedy")]
	[TestMethod]
	public void Parse_SimpleCommand_Successful(bool isLazy)
	{
		// Arrange
		const string commandName = "command";

		ICommandInfo command = Substitute.For<ICommandInfo>();
		ICommandGroupInfo rootGroup = Substitute.For<ICommandGroupInfo>();
		ICommandEngine engine = Substitute.For<ICommandEngine>();

		command.Name.Returns(commandName);
		rootGroup.Commands.Returns(new Dictionary<string, ICommandInfo>() { { commandName, command } });

		engine.Settings.Returns(SetupSettings());
		engine.RootGroup.Returns(rootGroup);

		CommandParser sut = new();

		// Act
		ICommandParserResult result = isLazy ? sut.Parse(engine, commandName) : sut.Parse(engine, [commandName]);
		ICommandParseResult? commandResult = result.LeafCommand;

		// Assert
		Assert.That
			.AreSameInstance(result.Engine, engine)
			.AreSameInstance(result.Parser, sut)
			.AreEqual(result.Stage, DiagnosticSource.Parsing)
			.AreEqual(result.Diagnostics.Count, 0)
			.AreEqual(result.Arguments.Count, 0)
			.AreEqual(result.Flags.Count, 0)
			.AreSameInstance(result.CommandOrGroup, commandResult);

		Assert.That
			.IsNotNull(commandResult)
			.AreSameInstance(commandResult.CommandInfo, command)
			.IsNotNull(commandResult.Name)
			.AreEqual(commandResult.Name.Value.Value, commandName)
			.AreEqual(commandResult.Arguments.Count, 0)
			.AreEqual(commandResult.Flags.Count, 0);
	}

	[DataRow(true, DisplayName = "Lazy")]
	[DataRow(false, DisplayName = "Greedy")]
	[TestMethod]
	public void Parse_CommandWithArgument_Successful(bool isLazy)
	{
		// Arrange
		const string commandName = "command";
		const string argumentValue = "value";

		IArgumentInfo argument = Substitute.For<IArgumentInfo>();
		ICommandInfo command = Substitute.For<ICommandInfo>();
		ICommandGroupInfo rootGroup = Substitute.For<ICommandGroupInfo>();
		ICommandEngine engine = Substitute.For<ICommandEngine>();

		argument.DefaultValue.Returns(null);
		argument.Parser.Returns(new StringValueParser());

		command.Name.Returns(commandName);
		command.Arguments.Returns([argument]);

		rootGroup.Commands.Returns(new Dictionary<string, ICommandInfo>() { { commandName, command } });

		engine.Settings.Returns(SetupSettings());
		engine.RootGroup.Returns(rootGroup);

		CommandParser sut = new();

		// Act
		ICommandParserResult result = isLazy ? sut.Parse(engine, $"{commandName} {argumentValue}") : sut.Parse(engine, [commandName, argumentValue]);
		ICommandParseResult? commandResult = result.LeafCommand;

		// Assert
		Assert.That
			.AreSameInstance(result.Engine, engine)
			.AreSameInstance(result.Parser, sut)
			.AreEqual(result.Stage, DiagnosticSource.Parsing)
			.AreEqual(result.Diagnostics.Count, 0)
			.AreEqual(result.Arguments.Count, 1)
			.AreEqual(result.Flags.Count, 0)
			.AreSameInstance(result.CommandOrGroup, commandResult);

		Assert.That
			.IsNotNull(commandResult)
			.AreSameInstance(commandResult.CommandInfo, command)
			.IsNotNull(commandResult.Name)
			.AreEqual(commandResult.Name.Value.Value, commandName)
			.AreEqual(commandResult.Arguments.Count, 1)
			.AreEqual(commandResult.Flags.Count, 0);

		IArgumentParseResult argumentResult = commandResult.Arguments[0];

		Assert.That
			.AreSameInstance(argumentResult.ArgumentInfo, argument)
			.IsNull(argumentResult.Value.Error)
			.AreEqual(argumentResult.Value.Value, argumentValue);
	}

	[DataRow(true, DisplayName = "Lazy")]
	[DataRow(false, DisplayName = "Greedy")]
	[TestMethod]
	public void Parse_ImplicitCommandWithArgument_Successful(bool isLazy)
	{
		// Arrange
		const string argumentValue = "value";

		IArgumentInfo argument = Substitute.For<IArgumentInfo>();
		ICommandInfo command = Substitute.For<ICommandInfo>();
		ICommandGroupInfo rootGroup = Substitute.For<ICommandGroupInfo>();
		ICommandEngine engine = Substitute.For<ICommandEngine>();

		argument.DefaultValue.Returns(null);
		argument.Parser.Returns(new StringValueParser());

		command.Name.Returns("command");
		command.Arguments.Returns([argument]);

		rootGroup.ImplicitCommand.Returns(command);

		engine.Settings.Returns(SetupSettings());
		engine.RootGroup.Returns(rootGroup);

		CommandParser sut = new();

		// Act
		ICommandParserResult result = isLazy ? sut.Parse(engine, argumentValue) : sut.Parse(engine, [argumentValue]);
		ICommandParseResult? commandResult = result.LeafCommand;

		// Assert
		Assert.That
			.AreSameInstance(result.Engine, engine)
			.AreSameInstance(result.Parser, sut)
			.AreEqual(result.Stage, DiagnosticSource.Parsing)
			.AreEqual(result.Diagnostics.Count, 0)
			.AreEqual(result.Arguments.Count, 1)
			.AreEqual(result.Flags.Count, 0)
			.AreSameInstance(result.CommandOrGroup, commandResult);

		Assert.That
			.IsNotNull(commandResult)
			.AreSameInstance(commandResult.CommandInfo, command)
			.AreEqual(commandResult.Arguments.Count, 1)
			.AreEqual(commandResult.Flags.Count, 0);

		IArgumentParseResult argumentResult = commandResult.Arguments[0];

		Assert.That
			.AreSameInstance(argumentResult.ArgumentInfo, argument)
			.IsNull(argumentResult.Value.Error)
			.AreEqual(argumentResult.Value.Value, argumentValue);
	}

	[DataRow(true, DisplayName = "Lazy")]
	[DataRow(false, DisplayName = "Greedy")]
	[TestMethod]
	public void Parse_NestedCommand_Successful(bool isLazy)
	{
		// Arrange
		const string groupName = "group";
		const string commandName = "command";

		ICommandGroupInfo group = Substitute.For<ICommandGroupInfo>();
		ICommandInfo command = Substitute.For<ICommandInfo>();
		ICommandGroupInfo rootGroup = Substitute.For<ICommandGroupInfo>();
		ICommandEngine engine = Substitute.For<ICommandEngine>();

		group.Name.Returns(groupName);
		group.Parent.Returns(rootGroup);

		command.Name.Returns(commandName);
		command.Group.Returns(group);

		rootGroup.Groups.Returns(new Dictionary<string, ICommandGroupInfo>() { { groupName, group } });
		group.Commands.Returns(new Dictionary<string, ICommandInfo>() { { commandName, command } });

		engine.Settings.Returns(SetupSettings());
		engine.RootGroup.Returns(rootGroup);

		CommandParser sut = new();

		// Act
		ICommandParserResult result = isLazy ? sut.Parse(engine, $"{groupName} {commandName}") : sut.Parse(engine, [groupName, commandName]);
		IGroupParseResult? groupResult = result.CommandOrGroup as IGroupParseResult;
		ICommandParseResult? commandResult = result.LeafCommand;

		// Assert
		Assert.That
			.AreSameInstance(result.Engine, engine)
			.AreSameInstance(result.Parser, sut)
			.AreEqual(result.Stage, DiagnosticSource.Parsing)
			.AreEqual(result.Diagnostics.Count, 0)
			.AreEqual(result.Arguments.Count, 0)
			.AreEqual(result.Flags.Count, 0);

		Assert.That
			.IsNotNull(groupResult)
			.AreSameInstance(groupResult.GroupInfo, group)
			.IsNotNull(groupResult.Name)
			.AreEqual(groupResult.Name.Value.Value, groupName);

		Assert.That
			.IsNotNull(commandResult)
			.AreSameInstance(commandResult.CommandInfo, command)
			.IsNotNull(commandResult.Name)
			.AreEqual(commandResult.Name.Value.Value, commandName)
			.AreEqual(commandResult.Arguments.Count, 0)
			.AreEqual(commandResult.Flags.Count, 0);
	}

	[DataRow(true, DisplayName = "Lazy")]
	[DataRow(false, DisplayName = "Greedy")]
	[TestMethod]
	public void Parse_ImplicitCommand_WithUnusedGroup_Successful(bool isLazy)
	{
		// Arrange
		ICommandInfo command = Substitute.For<ICommandInfo>();
		ICommandGroupInfo group = Substitute.For<ICommandGroupInfo>();
		ICommandGroupInfo rootGroup = Substitute.For<ICommandGroupInfo>();
		ICommandEngine engine = Substitute.For<ICommandEngine>();

		command.Name.Returns("command");
		group.Name.Returns("group");

		rootGroup.Groups.Returns(new Dictionary<string, ICommandGroupInfo>() { { "group", group } });
		rootGroup.ImplicitCommand.Returns(command);

		engine.Settings.Returns(SetupSettings());
		engine.RootGroup.Returns(rootGroup);

		CommandParser sut = new();

		// Act
		ICommandParserResult result = isLazy ? sut.Parse(engine, "") : sut.Parse(engine, [""]);
		ICommandParseResult? commandResult = result.LeafCommand;

		// Assert
		Assert.That
			.AreSameInstance(result.Engine, engine)
			.AreSameInstance(result.Parser, sut)
			.AreEqual(result.Stage, DiagnosticSource.Parsing)
			.AreEqual(result.Diagnostics.Count, 0)
			.AreEqual(result.Arguments.Count, 0)
			.AreEqual(result.Flags.Count, 0)
			.AreSameInstance(result.CommandOrGroup, commandResult);

		Assert.That
			.IsNotNull(commandResult)
			.AreSameInstance(commandResult.CommandInfo, command)
			.AreEqual(commandResult.Arguments.Count, 0)
			.AreEqual(commandResult.Flags.Count, 0);
	}

	[DynamicData(nameof(VariousCommandTests), DynamicDataSourceType.Method)]
	[TestMethod]
	public void Parse_VariousTests_Successful(string[] fragments, TextTokenKind[] expectedTokens, bool isLazy)
	{
		// Arrange
		const string groupName = "group";
		const string commandName = "command";

		StringValueParser stringParser = new();

		IFlagInfo valueFlag = Substitute.For<IFlagInfo>();
		IFlagInfo repeatFlag = Substitute.For<IFlagInfo>();
		IFlagInfo toggleFlag = Substitute.For<IFlagInfo>();
		IArgumentInfo argument = Substitute.For<IArgumentInfo>();
		ICommandInfo command = Substitute.For<ICommandInfo>();
		ICommandGroupInfo group = Substitute.For<ICommandGroupInfo>();
		ICommandGroupInfo rootGroup = Substitute.For<ICommandGroupInfo>();
		ICommandEngine engine = Substitute.For<ICommandEngine>();

		valueFlag.LongName.Returns("flag");
		valueFlag.ShortName.Returns('f');

		repeatFlag.LongName.Returns("repeat");
		repeatFlag.ShortName.Returns('r');

		toggleFlag.LongName.Returns("toggle");
		toggleFlag.ShortName.Returns('t');

		command.Name.Returns(commandName);
		group.Name.Returns(groupName);

		valueFlag.Parser.Returns(stringParser);
		argument.Parser.Returns(stringParser);
		repeatFlag.Parser.Returns(new ParsableValueParser<int>());
		toggleFlag.Parser.Returns(new BooleanValueParser());

		valueFlag.Kind.Returns(FlagKind.Regular);
		repeatFlag.Kind.Returns(FlagKind.Repeat);
		toggleFlag.Kind.Returns(FlagKind.Toggle);

		valueFlag.IsRequired.Returns(false);
		repeatFlag.IsRequired.Returns(false);
		toggleFlag.IsRequired.Returns(false);
		argument.IsRequired.Returns(false);

		IFlagInfo[] flags = [valueFlag, repeatFlag, toggleFlag];

		command.Flags.Returns(flags);
		group.SharedFlags.Returns(flags);
		rootGroup.SharedFlags.Returns(flags);
		command.Arguments.Returns([argument]);

		group.ImplicitCommand.Returns(command);
		rootGroup.ImplicitCommand.Returns(command);

		rootGroup.Groups.Returns(new Dictionary<string, ICommandGroupInfo>() { { groupName, group } });
		rootGroup.Commands.Returns(new Dictionary<string, ICommandInfo>() { { commandName, command } });
		group.Commands.Returns(new Dictionary<string, ICommandInfo>() { { commandName, command } });

		engine.Settings.Returns(SetupSettings());
		engine.RootGroup.Returns(rootGroup);

		CommandParser sut = new();

		// Act
		ICommandParserResult result = isLazy ? sut.Parse(engine, fragments[0]) : sut.Parse(engine, fragments);

		// Assert
		CheckFailedResult(result, isLazy, fragments, expectedTokens);
	}


	[DynamicData(nameof(ArrayCollectionValueTests), DynamicDataSourceType.Method)]
	[TestMethod]
	public void Parse_ArrayCollectionTests_Successful(string[] fragments, TextTokenKind[] expectedTokens, bool isLazy)
	{
		// Arrange
		IValueParser<string> stringParser = new StringValueParser();
		ArrayCollectionValueParser<string> arrayParser = new(stringParser);

		IArgumentInfo argument = Substitute.For<IArgumentInfo>();
		ICommandInfo command = Substitute.For<ICommandInfo>();
		ICommandGroupInfo rootGroup = Substitute.For<ICommandGroupInfo>();
		ICommandEngine engine = Substitute.For<ICommandEngine>();

		argument.IsRequired.Returns(true);
		argument.Parser.Returns(arrayParser);

		command.Arguments.Returns([argument]);
		rootGroup.ImplicitCommand.Returns(command);

		engine.Settings.Returns(SetupSettings());
		engine.RootGroup.Returns(rootGroup);

		CommandParser sut = new();

		// Act
		ICommandParserResult result = isLazy ? sut.Parse(engine, fragments[0]) : sut.Parse(engine, fragments);

		// Assert
		CheckFailedResult(result, isLazy, fragments, expectedTokens);
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private static void CheckFailedResult(ICommandParserResult result, bool isLazy, string[] fragments, TextTokenKind[] expectedTokens)
	{
		if (result.Successful is false)
		{
			string message = isLazy ? $"Lazy parsing failed for the command: {fragments[0]}" : $"Greedy parsing failed for the command: {string.Join("|", fragments)}";
			message += "\n\nDiagnostics:";

			foreach (IDiagnostic diagnostic in result.Diagnostics)
				message += $"\n- [{diagnostic.Location}]: {diagnostic.Message}";

			Assert.That.Fail(message + "\n");
		}

		TextToken[] tokens = [.. result.EnumerateTokens()];
		TextTokenKind[] resultTokens = [.. tokens.Select(t => t.Kind)];
		if (resultTokens.Length != expectedTokens.Length || (resultTokens.SequenceEqual(expectedTokens) is false))
		{
			string message = isLazy ? $"Lazy parsing failed for the command: {fragments[0]}" : $"Greedy parsing failed for the command: {string.Join("|", fragments)}";
			message += $"\n\nExpected tokens:\n{string.Join(' ', expectedTokens)}";
			message += $"\n\nResult tokens:\n{string.Join(' ', resultTokens)}";

			Assert.That.Fail(message + "\n");
		}
	}

	[ExcludeFromCodeCoverage]
	private static IEnumerable<object?[]> ArrayCollectionValueTests()
	{
		(string, TextTokenKind[])[] arguments = [
			("[|]", [TextTokenKind.Symbol, TextTokenKind.Symbol]),

			("[ | a | ]", [TextTokenKind.Symbol, TextTokenKind.Value, TextTokenKind.Symbol]),
			("[ | \"a\" | ]", [TextTokenKind.Symbol, TextTokenKind.Value, TextTokenKind.Symbol]),
			("[ | a | , | b]", [TextTokenKind.Symbol, TextTokenKind.Value, TextTokenKind.Symbol, TextTokenKind.Value, TextTokenKind.Symbol]),
			("[ | \"a\" | , | \"b\" | ]", [TextTokenKind.Symbol, TextTokenKind.Value, TextTokenKind.Symbol, TextTokenKind.Value, TextTokenKind.Symbol]),
			("[ | \"a a\" | , | \"b b\" | ]", [TextTokenKind.Symbol, TextTokenKind.Value, TextTokenKind.Symbol, TextTokenKind.Value, TextTokenKind.Symbol]),

			("a", [TextTokenKind.Value]),
			("a | , | b", [TextTokenKind.Value, TextTokenKind.Symbol, TextTokenKind.Value]),
		];

		foreach ((string argument, TextTokenKind[] tokens) pair in arguments)
		{
			string cmd = pair.argument;

			List<TextTokenKind> tokenList = [];
			tokenList.AddRange(pair.tokens);

			TextTokenKind[] allTokens = [.. tokenList];

			yield return
			[
				new string[] { cmd.Replace(" | ", "").Replace("|", "") },
				allTokens,
				true
			];

			yield return
			[
				new string[] { cmd.Replace(" | ", " ").Replace("|", " ") },
				allTokens,
				true
			];

			string[] fragments = cmd.Split("|", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

			yield return
			[
				fragments,
				allTokens,
				false
			];
		}
	}

	[ExcludeFromCodeCoverage]
	private static IEnumerable<object?[]> VariousCommandTests()
	{
		(string, TextTokenKind[])[] flags =
		[
			("", []),

			("-f=test", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),
			("-f=|test", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),
			("-f:test", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),
			("-f:|test", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),
			("-f | test", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Value]),

			("--flag=test", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),
			("--flag=|test", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),
			("--flag:test", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),
			("--flag:|test", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),
			("--flag | test", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Value]),


			("-t", [TextTokenKind.Symbol, TextTokenKind.FlagName]),
			("-t=true", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),
			("-t=|true", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),
			("-t:true", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),
			("-t:|true", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),

			("-t=true", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),
			("--toggle=|true", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),
			("--toggle:true", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),
			("--toggle:|true", [TextTokenKind.Symbol, TextTokenKind.FlagName, TextTokenKind.Symbol, TextTokenKind.Value]),

			("-tr", [TextTokenKind.Symbol, TextTokenKind.FlagName]),
			("-rr", [TextTokenKind.Symbol, TextTokenKind.FlagName]),
			("--toggle", [TextTokenKind.Symbol, TextTokenKind.FlagName]),
			("--repeat", [TextTokenKind.Symbol, TextTokenKind.FlagName]),
		];

		string[] arguments = ["", "argument", "--argument"];
		string[] commands = ["", "command"];
		string[] groups = ["", "group"];

		foreach (string group in groups)
			foreach (string command in commands)
				foreach ((string flag, TextTokenKind[] tokens) pair in flags)
					foreach (string argument in arguments)
					{
						string cmd = group;
						cmd += cmd.Length is 0 || command.Length is 0 ? command : $" | {command}";
						cmd += cmd.Length is 0 || pair.flag.Length is 0 ? pair.flag : $" | {pair.flag}";

						if (argument.StartsWith("--"))
						{
							if (cmd.Length is 0)
								cmd += "--";
							else
								cmd += " | --";
						}
						cmd += cmd.Length is 0 || argument.Length is 0 ? argument : $" | {argument}";

						List<TextTokenKind> tokenList = [];
						if (group is not "") tokenList.Add(TextTokenKind.GroupName);
						if (command is not "") tokenList.Add(TextTokenKind.CommandName);
						tokenList.AddRange(pair.tokens);
						if (argument.StartsWith("--")) tokenList.Add(TextTokenKind.Symbol);
						if (argument is not "") tokenList.Add(TextTokenKind.Value);

						TextTokenKind[] allTokens = [.. tokenList];

						yield return
						[
							new string[] { cmd.Replace(" | ", " ").Replace("|", "") },
							allTokens,
							true
						];

						string[] fragments = cmd.Split("|", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

						if (fragments.Length is 0)
							continue; // Note(Nightowl): No fragments means nothing to parse in greedy mode;

						yield return
						[
							fragments,
							allTokens,
							false
						];
					}
	}

	private static IEngineSettings SetupSettings()
	{
		BuilderSettings settings = new();
		return EngineSettings.From(settings);
	}
	#endregion
}
