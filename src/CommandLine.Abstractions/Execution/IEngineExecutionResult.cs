namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents the execution result for a parsed and validated command.
/// </summary>
public interface IEngineExecutionResult
{
	#region Properties
	/// <summary>The validation result that was executed.</summary>
	ICommandValidatorResult ValidatorResult { get; }

	/// <summary>The diagnostics that occurred during execution.</summary>
	IDiagnosticBag Diagnostics { get; }
	#endregion
}
