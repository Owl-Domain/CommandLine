namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents the execution result for a validated command.
/// </summary>
/// <param name="successful">Whether the operation was successful.</param>
/// <param name="validatorResult">The validation result that was executed.</param>
/// <param name="diagnostics">The diagnostics that occurred during execution.</param>
/// <param name="duration">The amount of time that the execution operation took.</param>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class CommandExecutorResult(
	bool successful,
	ICommandValidatorResult validatorResult,
	IDiagnosticBag diagnostics,
	TimeSpan duration)
	: ICommandExecutorResult
{
	#region Properties
	/// <inheritdoc/>
	public bool Successful { get; } = successful;

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
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string typeName = nameof(CommandExecutorResult);
		const string successfulName = nameof(Successful);
		const string durationName = nameof(Duration);

		return $"{typeName} {{ {successfulName} = ({Successful}), {durationName} = ({Duration}) }}";
	}
	#endregion
}
