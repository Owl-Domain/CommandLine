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

		command.Name.Returns(commandName);
		rootGroup.Commands.Returns(new Dictionary<string, ICommandInfo>() { { commandName, command } });

		CommandParser sut = new();
		CommandEngine engine = new(rootGroup, sut);

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

		argument.DefaultValue.Returns(null);
		argument.Parser.Returns(new StringValueParser());

		command.Name.Returns(commandName);
		command.Arguments.Returns([argument]);

		rootGroup.Commands.Returns(new Dictionary<string, ICommandInfo>() { { commandName, command } });

		CommandParser sut = new();
		CommandEngine engine = new(rootGroup, sut);

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

		argument.DefaultValue.Returns(null);
		argument.Parser.Returns(new StringValueParser());

		command.Name.Returns("command");
		command.Arguments.Returns([argument]);

		rootGroup.ImplicitCommand.Returns(command);

		CommandParser sut = new();
		CommandEngine engine = new(rootGroup, sut);

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

		group.Name.Returns(groupName);
		group.Parent.Returns(rootGroup);

		command.Name.Returns(commandName);
		command.Group.Returns(group);

		rootGroup.Groups.Returns(new Dictionary<string, ICommandGroupInfo>() { { groupName, group } });
		group.Commands.Returns(new Dictionary<string, ICommandInfo>() { { commandName, command } });

		CommandParser sut = new();
		CommandEngine engine = new(rootGroup, sut);

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

		command.Name.Returns("command");
		group.Name.Returns("group");

		rootGroup.Groups.Returns(new Dictionary<string, ICommandGroupInfo>() { { "group", group } });
		rootGroup.ImplicitCommand.Returns(command);

		CommandParser sut = new();
		CommandEngine engine = new(rootGroup, sut);

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
	#endregion
}
