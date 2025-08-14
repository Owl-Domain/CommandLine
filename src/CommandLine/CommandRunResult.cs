namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents the end-to-end result of running a command.
/// </summary>
/// <param name="successful">Whether the command was processed successfully.</param>
/// <param name="wasCancelled">Whether the operation was cancelled.</param>
/// <param name="parserResult">The result from parsing the command.</param>
/// <param name="validatorResult">The result from validating the command.</param>
/// <param name="executorResult">The result from executing the command</param>
/// <param name="diagnostics">All of the diagnostics that have occurred while processing the command.</param>
/// <param name="duration">The total amount of time it took to process the command.</param>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class CommandRunResult(
	bool successful,
	bool wasCancelled,
	ICommandParserResult parserResult,
	ICommandValidatorResult validatorResult,
	ICommandExecutorResult executorResult,
	IDiagnosticBag diagnostics,
	TimeSpan duration)
	: ICommandRunResult
{
	#region Properties
	/// <inheritdoc/>
	public bool Successful { get; } = successful;

	/// <inheritdoc/>
	public ICommandParserResult ParserResult { get; } = parserResult;

	/// <inheritdoc/>
	public ICommandValidatorResult ValidatorResult { get; } = validatorResult;

	/// <inheritdoc/>
	public ICommandExecutorResult ExecutorResult { get; } = executorResult;

	/// <inheritdoc/>
	public IDiagnosticBag Diagnostics { get; } = diagnostics;

	/// <inheritdoc/>
	public TimeSpan Duration { get; } = duration;

	/// <inheritdoc/>
	public bool WasCancelled { get; } = wasCancelled;
	#endregion

	#region Helpers
	private string DebuggerDisplay()
	{
		const string typeName = nameof(CommandRunResult);
		const string successfulName = nameof(Successful);
		const string durationName = nameof(Duration);

		return $"{typeName} {{ {successfulName} = ({Successful}), {durationName} = ({Duration}) }}";
	}
	#endregion
}
