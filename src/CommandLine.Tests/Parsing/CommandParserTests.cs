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
	public void Parse_VariousTests_Successful(string[] fragments, bool isLazy)
	{
		// Arrange
		const string groupName = "group";
		const string commandName = "command";
		const string longFlagName = "flag";
		const char shortFlagName = 'f';

		StringValueParser stringParser = new();

		IFlagInfo flag = Substitute.For<IFlagInfo>();
		IArgumentInfo argument = Substitute.For<IArgumentInfo>();
		ICommandInfo command = Substitute.For<ICommandInfo>();
		ICommandGroupInfo group = Substitute.For<ICommandGroupInfo>();
		ICommandGroupInfo rootGroup = Substitute.For<ICommandGroupInfo>();
		ICommandEngine engine = Substitute.For<ICommandEngine>();

		flag.LongName.Returns(longFlagName);
		flag.ShortName.Returns(shortFlagName);
		command.Name.Returns(commandName);
		group.Name.Returns(groupName);

		flag.Parser.Returns(stringParser);
		argument.Parser.Returns(stringParser);

		flag.IsRequired.Returns(false);
		argument.IsRequired.Returns(false);

		command.Flags.Returns([flag]);
		group.SharedFlags.Returns([flag]);
		rootGroup.SharedFlags.Returns([flag]);
		command.Arguments.Returns([argument]);

		group.ImplicitCommand.Returns(command);
		rootGroup.ImplicitCommand.Returns(command);

		rootGroup.Groups.Returns(new Dictionary<string, ICommandGroupInfo>() { { groupName, group } });
		rootGroup.Commands.Returns(new Dictionary<string, ICommandInfo>() { { commandName, command } });
		group.Commands.Returns(new Dictionary<string, ICommandInfo>() { { commandName, command } });

		engine.RootGroup.Returns(rootGroup);

		CommandParser sut = new();

		// Act
		ICommandParserResult result = isLazy ? sut.Parse(engine, fragments[0]) : sut.Parse(engine, fragments);

		// Assert
		CheckFailedResult(result, isLazy, fragments);
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private static void CheckFailedResult(ICommandParserResult result, bool isLazy, string[] fragments)
	{
		if (result.Successful is false)
		{
			string message = isLazy ? $"Lazy parsing failed for the command: {fragments[0]}" : $"Greedy parsing failed for the command: {string.Join("|", fragments)}";
			message += "\n\nDiagnostics:";

			foreach (IDiagnostic diagnostic in result.Diagnostics)
				message += $"\n- [{diagnostic.Location}]: {diagnostic.Message}";

			Assert.That.Fail(message + "\n");
		}
	}

	[ExcludeFromCodeCoverage]
	private static IEnumerable<object?[]> VariousCommandTests()
	{
		string[] flags =
		[
			"",

			"-f=test",
			"-f=|test",
			"-f:test",
			"-f:|test",
			"-f | test",

			"--flag=test",
			"--flag=|test",
			"--flag:test",
			"--flag:|test",
			"--flag | test",
		];

		string[] arguments = ["", "argument"];
		string[] commands = ["", "command"];
		string[] groups = ["", "group"];

		foreach (string group in groups)
			foreach (string command in commands)
				foreach (string flag in flags)
					foreach (string argument in arguments)
					{
						string cmd = group;
						cmd += cmd.Length is 0 || command.Length is 0 ? command : $" | {command}";
						cmd += cmd.Length is 0 || flag.Length is 0 ? flag : $" | {flag}";
						cmd += cmd.Length is 0 || argument.Length is 0 ? argument : $" | {argument}";

						yield return [new string[] { cmd.Replace(" | ", " ").Replace("|", "") }, true];

						string[] fragments = cmd.Split("|", StringSplitOptions.TrimEntries);
						yield return [fragments, false];
					}
	}
	#endregion
}
