namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents the execution result for a validated command.
/// </summary>
/// <param name="successful">Whether the operation was successful.</param>
/// <param name="wasCancelled">whether the ooperation was cancelled.</param>
/// <param name="validatorResult">The validation result that was executed.</param>
/// <param name="diagnostics">The diagnostics that occurred during execution.</param>
/// <param name="duration">The amount of time that the execution operation took.</param>
/// <param name="result">The (optional) result returned by the command.</param>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class CommandExecutorResult(
	bool successful,
	bool wasCancelled,
	ICommandValidatorResult validatorResult,
	IDiagnosticBag diagnostics,
	TimeSpan duration,
	object? result)
	: ICommandExecutorResult
{
	#region Properties
	/// <inheritdoc/>
	public bool Successful { get; } = successful;

	/// <inheritdoc/>
	public bool WasCancelled { get; } = wasCancelled;

	/// <inheritdoc/>
	public DiagnosticSource Stage => DiagnosticSource.Execution;

	/// <inheritdoc/>
	public ICommandEngine Engine => ValidatorResult.Engine;

	/// <inheritdoc/>
	public ICommandValidatorResult ValidatorResult { get; } = validatorResult;

	/// <inheritdoc/>
	public IDiagnosticBag Diagnostics { get; } = diagnostics;

	/// <inheritdoc/>
	public TimeSpan Duration { get; } = duration;

	/// <inheritdoc/>
	public object? Result { get; } = result;
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string typeName = nameof(CommandExecutorResult);
		const string successfulName = nameof(Successful);
		const string durationName = nameof(Duration);
		const string resultName = nameof(Result);

		return $"{typeName} {{ {successfulName} = ({Successful}), {resultName} = ({Result}), {durationName} = ({Duration}) }}";
	}
	#endregion
}
