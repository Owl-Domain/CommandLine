namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents the execution result for a validated command.
/// </summary>
/// <param name="validationResult">The validation result that was executed.</param>
/// <param name="diagnostics">The diagnostics that occurred during execution.</param>
public sealed class EngineExecutionResult(
	IEngineValidationResult validationResult,
	IDiagnosticBag diagnostics)
	: IEngineExecutionResult
{
	#region Properties
	/// <inheritdoc/>
	public IEngineValidationResult ValidationResult { get; } = validationResult;

	/// <inheritdoc/>
	public IDiagnosticBag Diagnostics { get; } = diagnostics;
	#endregion
}
