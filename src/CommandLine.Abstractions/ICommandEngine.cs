namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents the command line engine.
/// </summary>
public interface ICommandEngine
{
	#region Properties
	/// <summary>The root command group.</summary>
	ICommandGroupInfo RootGroup { get; }

	/// <summary>The parser to use for parsing the commands.</summary>
	ICommandParser Parser { get; }
	#endregion

	#region Methods
	/// <summary>Parses the given <paramref name="fragments"/>.</summary>
	/// <param name="fragments">The fragments to parse.</param>
	/// <returns>The result of the parse operation.</returns>
	ICommandParserResult Parse(string[] fragments);

	/// <summary>Parses the given <paramref name="command"/> text.</summary>
	/// <param name="command">The command text to parse.</param>
	/// <returns>The result of the parse operation.</returns>
	ICommandParserResult Parse(string command);

	/// <summary>Validates the given <paramref name="parseResult"/>.</summary>
	/// <param name="parseResult">The parsing result to validate.</param>
	/// <returns>The validation result.</returns>
	IEngineValidationResult Validate(ICommandParserResult parseResult);

	/// <summary>Executes the given <paramref name="validationResult"/>.</summary>
	/// <param name="validationResult">The validation result to execute.</param>
	/// <returns>The execution result.</returns>
	IEngineExecutionResult Execute(IEngineValidationResult validationResult);
	#endregion
}
