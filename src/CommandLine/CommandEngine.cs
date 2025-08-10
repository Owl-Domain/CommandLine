
namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents the command line engine.
/// </summary>
/// <param name="settings">The settings for the engine.</param>
/// <param name="rootGroup">The root command group.</param>
/// <param name="parser">The parser to use for parsing the commands.</param>
/// <param name="validator">The validator to use for validating the parsed commands.</param>
/// <param name="executor">The executor to use for executing the validated commands.</param>
/// <param name="documentationPrinter">The documentation printer for the engine.</param>
/// <param name="virtualCommands">The known virtual commands that have been added to the engine.</param>
/// <param name="virtualFlags">The known virtual flags that have been added to the engine.</param>
public sealed class CommandEngine(
	IEngineSettings settings,
	ICommandGroupInfo rootGroup,
	ICommandParser parser,
	ICommandValidator validator,
	ICommandExecutor executor,
	IDocumentationPrinter documentationPrinter,
	IVirtualCommands virtualCommands,
	IVirtualFlags virtualFlags)
	: ICommandEngine
{
	#region Properties
	/// <inheritdoc/>
	public IEngineSettings Settings { get; } = settings;

	/// <inheritdoc/>
	public ICommandGroupInfo RootGroup { get; } = rootGroup;

	/// <inheritdoc/>
	public ICommandParser Parser { get; } = parser;

	/// <inheritdoc/>
	public ICommandValidator Validator { get; } = validator;

	/// <inheritdoc/>
	public ICommandExecutor Executor { get; } = executor;

	/// <inheritdoc/>
	public IDocumentationPrinter DocumentationPrinter { get; } = documentationPrinter;

	/// <inheritdoc/>
	public IVirtualCommands VirtualCommands { get; } = virtualCommands;

	/// <inheritdoc/>
	public IVirtualFlags VirtualFlags { get; } = virtualFlags;
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
	public ICommandExecutorResult Execute(ICommandValidatorResult validatorResult, CommandExecutionDelegate? callback = null)
	{
		return Executor.Execute(validatorResult, callback);
	}

	/// <inheritdoc/>
	public ICommandRunResult Run(string[] fragments, CommandExecutionDelegate? callback = null)
	{
		Stopwatch watch = Stopwatch.StartNew();
		ICommandParserResult parserResult = Parser.Parse(this, fragments);

		return Run(watch, parserResult, callback);
	}

	/// <inheritdoc/>
	public ICommandRunResult Run(string command, CommandExecutionDelegate? callback = null)
	{
		Stopwatch watch = Stopwatch.StartNew();
		ICommandParserResult parserResult = Parser.Parse(this, command);

		return Run(watch, parserResult, callback);
	}

	private ICommandRunResult Run(Stopwatch watch, ICommandParserResult parserResult, CommandExecutionDelegate? callback = null)
	{
		ICommandValidatorResult validatorResult = Validator.Validate(parserResult);
		ICommandExecutorResult executorResult = Executor.Execute(validatorResult, callback);

		DiagnosticBag diagnostics =
		[
			..parserResult.Diagnostics,
			.. validatorResult.Diagnostics,
			.. executorResult.Diagnostics
		];

		watch.Stop();

		return new CommandRunResult(executorResult.Successful, parserResult, validatorResult, executorResult, diagnostics, watch.Elapsed);
	}
	#endregion
}
