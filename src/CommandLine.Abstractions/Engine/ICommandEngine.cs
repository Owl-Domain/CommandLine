namespace OwlDomain.CommandLine.Engine;

/// <summary>Represents a delegate for command execution.</summary>
/// <param name="context">The context for the executing command.</param>
/// <returns>The result value returned by the command.</returns>
public delegate void CommandExecutionDelegate(ICommandExecutionContext context);

/// <summary>
/// 	Represents the command line engine.
/// </summary>
public interface ICommandEngine
{
	#region Properties
	/// <summary>The settings for the engine.</summary>
	IEngineSettings Settings { get; }

	/// <summary>The root command group.</summary>
	ICommandGroupInfo RootGroup { get; }

	/// <summary>The parser to use for parsing the commands.</summary>
	ICommandParser Parser { get; }

	/// <summary>The root selector for value parsers.</summary>
	IRootValueParserSelector ValueParserSelector { get; }

	/// <summary>The validator to use for validating the parsed commands.</summary>
	ICommandValidator Validator { get; }

	/// <summary>The executor to use for executing the validated commands.</summary>
	ICommandExecutor Executor { get; }

	/// <summary>The documentation printer for the engine.</summary>
	IDocumentationPrinter DocumentationPrinter { get; }

	/// <summary>The command output printer for the engine.</summary>
	IOutputPrinter OutputPrinter { get; }

	/// <summary>The known virtual commands that have been added to the engine.</summary>
	IVirtualCommands VirtualCommands { get; }

	/// <summary>The known virtual flags that have been added to the engine.</summary>
	IVirtualFlags VirtualFlags { get; }
	#endregion

	#region Methods
	/// <summary>Parses the given command <paramref name="fragments"/>.</summary>
	/// <param name="fragments">The command fragments to parse.</param>
	/// <returns>The result of the parse operation.</returns>
	/// <remarks>
	/// 	This overload is intended to be used when you have the tokenised command fragments,
	/// 	for example when you have access to the <see cref="Environment.GetCommandLineArgs"/>.
	/// </remarks>
	ICommandParserResult Parse(string[] fragments);

	/// <summary>Parses the given <paramref name="command"/> text.</summary>
	/// <param name="command">The command text to parse.</param>
	/// <returns>The result of the parse operation.</returns>
	/// <remarks>
	/// 	This overload is intended to be used in REPL-like circumstances
	/// 	where you only have access to the full command.
	/// </remarks>
	ICommandParserResult Parse(string command);

	/// <summary>Validates the given <paramref name="parserResult"/>.</summary>
	/// <param name="parserResult">The parsing result to validate.</param>
	/// <returns>The validation result.</returns>
	/// /// <exception cref="ArgumentException">Thrown if the given <paramref name="parserResult"/> cannot be validated.</exception>
	ICommandValidatorResult Validate(ICommandParserResult parserResult);

	/// <summary>Executes the given <paramref name="validatorResult"/>.</summary>
	/// <param name="validatorResult">The validation result to execute.</param>
	/// <param name="callback">An optional callback which can be used to hook into the execution process.</param>
	/// <returns>The execution result.</returns>
	/// <exception cref="ArgumentException">Thrown if the given <paramref name="validatorResult"/> cannot be executed.</exception>
	ICommandExecutorResult Execute(ICommandValidatorResult validatorResult, CommandExecutionDelegate? callback = null);

	/// <summary>Parses, validates and executes the given command <paramref name="fragments"/>.</summary>
	/// <param name="fragments">The command fragments to process.</param>
	/// <param name="callback">An optional callback which can be used to hook into the execution process.</param>
	/// <returns>The result of the run operation.</returns>
	/// <remarks>
	/// 	This overload is intended to be used when you have the tokenised command fragments,
	/// 	for example when you have access to the <see cref="Environment.GetCommandLineArgs"/>.
	/// </remarks>
	ICommandRunResult Run(string[] fragments, CommandExecutionDelegate? callback = null);

	/// <summary>Parses, validates and executes the given <paramref name="command"/> text.</summary>
	/// <param name="command">The command to process.</param>
	/// <param name="callback">An optional callback which can be used to hook into the execution process.</param>
	/// <returns>The result of the run operation.</returns>
	/// <remarks>
	/// 	This overload is intended to be used in REPL-like circumstances
	/// 	where you only have access to the full command.
	/// </remarks>
	ICommandRunResult Run(string command, CommandExecutionDelegate? callback = null);

	/// <summary>Enters REPL mode where several commands can be executed.</summary>
	/// <param name="promptCallback">The callback to use when getting the prompt for the next command.</param>
	void Repl(Func<string> promptCallback);
	#endregion
}

/// <summary>
/// 	Contains various extension methods related to the <see cref="ICommandEngine"/>.
/// </summary>
public static class ICommandEngineExtensions
{
	#region Methods
	/// <summary>Enters REPL mode where several commands can be executed.</summary>
	/// <param name="engine">The engine to enter the REPL mode in.</param>
	/// <param name="prompt">The text to use for the command prompt.</param>
	public static void Repl(this ICommandEngine engine, string prompt = "> ")
	{
		string Callback() => prompt;

		engine.Repl(Callback);
	}
	#endregion
}