namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents the command line engine.
/// </summary>
/// <param name="rootGroup">The root command group.</param>
/// <param name="parser">The parser to use for parsing the commands.</param>
/// <param name="validator">The validator to use for validating the parsed commands.</param>
/// <param name="executor">The executor to use for executing the validated commands.</param>
/// <param name="documentationPrinter">The documentation printer for the engine.</param>
public sealed class CommandEngine(
	ICommandGroupInfo rootGroup,
	ICommandParser parser,
	ICommandValidator validator,
	ICommandExecutor executor,
	IDocumentationPrinter documentationPrinter)
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

	/// <inheritdoc/>
	public IDocumentationPrinter DocumentationPrinter { get; } = documentationPrinter;
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

	/// <inheritdoc/>
	public ICommandRunResult Run(string[] fragments)
	{
		Stopwatch watch = Stopwatch.StartNew();
		ICommandParserResult parserResult = Parser.Parse(this, fragments);

		return Run(watch, parserResult);
	}

	/// <inheritdoc/>
	public ICommandRunResult Run(string command)
	{
		Stopwatch watch = Stopwatch.StartNew();
		ICommandParserResult parserResult = Parser.Parse(this, command);

		return Run(watch, parserResult);
	}

	private ICommandRunResult Run(Stopwatch watch, ICommandParserResult parserResult)
	{
		ICommandValidatorResult validatorResult = Validator.Validate(parserResult);
		ICommandExecutorResult executorResult = Executor.Execute(validatorResult);

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
