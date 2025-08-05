namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents the execution result for a validated command.
/// </summary>
/// <param name="validatorResult">The validation result that was executed.</param>
/// <param name="diagnostics">The diagnostics that occurred during execution.</param>
public sealed class CommandExecutorResult(
	ICommandValidatorResult validatorResult,
	IDiagnosticBag diagnostics)
	: ICommandExecutorResult
{
	#region Properties
	/// <inheritdoc/>
	public DiagnosticSource Stage => DiagnosticSource.Execution;

	/// <inheritdoc/>
	public ICommandEngine Engine => ValidatorResult.Engine;

	/// <inheritdoc/>
	public ICommandValidatorResult ValidatorResult { get; } = validatorResult;

	/// <inheritdoc/>
	public IDiagnosticBag Diagnostics { get; } = diagnostics;
	#endregion
}
