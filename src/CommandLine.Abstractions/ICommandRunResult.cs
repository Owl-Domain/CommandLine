namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents the end-to-end result of running a command.
/// </summary>
public interface ICommandRunResult
{
	#region Properties
	/// <summary>Whether the command was processed successfully.</summary>
	bool Successful { get; }

	/// <summary>Whether the operation was cancelled.</summary>
	bool WasCancelled { get; }

	/// <summary>The result from parsing the command.</summary>
	ICommandParserResult ParserResult { get; }

	/// <summary>The result from validating the command.</summary>
	ICommandValidatorResult ValidatorResult { get; }

	/// <summary>The result from executing the command.</summary>
	ICommandExecutorResult ExecutorResult { get; }

	/// <summary>All of the diagnostics that have occurred while processing the command.</summary>
	IDiagnosticBag Diagnostics { get; }

	/// <summary>The total amount of time it took to process the command.</summary>
	TimeSpan Duration { get; }
	#endregion
}
