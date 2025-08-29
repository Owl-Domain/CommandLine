namespace OwlDomain.CommandLine.Tests.Engine;

[TestClass]
public sealed class CommandEngineBuilderTests
{
	#region Nested types
	[ExcludeFromCodeCoverage]
	private sealed class SimpleCommand
	{
		public required int Flag { get; init; }

		public int Foo(int argument) => Flag + argument;
	}
	#endregion

	#region Tests
	[TestMethod]
	public void Build_WithNoClasses_Successful()
	{
		// Arrange
		CommandEngineBuilder sut = new();

		// Act
		void Act() => sut.Build();

		// Assert
		Assert.That.DoesNotThrowAnyException(Act);
	}

	[TestMethod]
	public void Build_WithSimpleClass_Successful()
	{
		// Arrange
		const string expectedLongFlagName = "flag";
		const char expectedShortFlagName = 'f';
		const string expectedCommandName = "foo";
		const string expectedArgumentName = "argument";
		Type expectedValueType = typeof(int);

		CommandEngineBuilder sut = new();

		sut.Customise(settings => settings.WithoutHelpCommand().WithoutHelpFlag().WithoutVersionCommand());
		sut.From<SimpleCommand>();

		// Act
		ICommandEngine Act() => sut.Build();

		// Assert
		Assert.That
			.DoesNotThrowAnyException(Act, out ICommandEngine engine)
			.AreEqual(engine.RootGroup.SharedFlags.Count, 1)
			.AreEqual(engine.RootGroup.Groups.Count, 0)
			.AreEqual(engine.RootGroup.Commands.Count, 1);

		IFlagInfo resultFlag = engine.RootGroup.SharedFlags.Single();

		Assert.That
			.AreEqual(resultFlag.LongName, expectedLongFlagName)
			.AreEqual(resultFlag.ShortName, expectedShortFlagName)
			.AreEqual(resultFlag.ValueInfo.Type, expectedValueType)
			.IsTrue(resultFlag.ValueInfo.IsRequired)
			.IsFalse(resultFlag.ValueInfo.IsNullable)
			.IsNull(resultFlag.DefaultValueInfo)
			.IsNull(resultFlag.Documentation);

		ICommandInfo resultCommand = engine.RootGroup.Commands.Single().Value;

		Assert.That
			.AreEqual(resultCommand.Name, expectedCommandName)
			.IsTrue(resultCommand.HasResultValue)
			.IsTrue(resultCommand.Flags.Contains(resultFlag))
			.AreEqual(resultCommand.Arguments.Count, 1);

		IArgumentInfo resultArgument = resultCommand.Arguments.Single();

		Assert.That
			.AreEqual(resultArgument.Name, expectedArgumentName)
			.AreEqual(resultArgument.Position, 0)
			.AreEqual(resultArgument.ValueInfo.Type, expectedValueType)
			.IsTrue(resultArgument.ValueInfo.IsRequired)
			.IsFalse(resultArgument.ValueInfo.IsNullable)
			.IsNull(resultArgument.DefaultValueInfo)
			.IsNull(resultArgument.Documentation);
	}
	#endregion
}
