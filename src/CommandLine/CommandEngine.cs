namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents the command line engine.
/// </summary>
/// <param name="rootGroup">The root command group.</param>
/// <param name="parser">The parser to use for parsing the commands.</param>
/// <param name="validator">The validator to use for validating the parsed commands.</param>
/// <param name="executor">The executor to use for executing the validated commands.</param>
public sealed class CommandEngine(
	ICommandGroupInfo rootGroup,
	ICommandParser parser,
	ICommandValidator validator,
	ICommandExecutor executor)
	: ICommandEngine
{
	#region Properties
	/// <inheritdoc/>
	public ICommandGroupInfo RootGroup { get; } = rootGroup;

	/// <inheritdoc/>
	public ICommandParser Parser { get; } = parser;

	/// <inheritdoc/>
	public ICommandValidator Validator { get; } = validator;

	/// <inheritdoc/>
	public ICommandExecutor Executor { get; } = executor;
	#endregion

	#region Functions
	/// <summary>Creates a builder for a new command engine.</summary>
	/// <returns>The builder which can be used to customise the built command engine.</returns>
	public static ICommandEngineBuilder New() => new CommandEngineBuilder();
	#endregion

	#region Methods
	/// <inheritdoc/>
	public ICommandParserResult Parse(string[] fragments) => Parser.Parse(this, fragments);

	/// <inheritdoc/>
	public ICommandParserResult Parse(string command) => Parser.Parse(this, command);

	/// <inheritdoc/>
	public ICommandValidatorResult Validate(ICommandParserResult parserResult) => Validator.Validate(parserResult);

	/// <inheritdoc/>
	public ICommandExecutorResult Execute(ICommandValidatorResult validatorResult) => Executor.Execute(validatorResult);
	#endregion
}
