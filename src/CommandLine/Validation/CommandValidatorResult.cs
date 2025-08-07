namespace OwlDomain.CommandLine.Validation;

/// <summary>
/// 	Represents the validation result for a parsed command.
/// </summary>
/// <param name="successful">Whether the operation was successful.</param>
/// <param name="parserResult">The parsing result that was validated.</param>
/// <param name="diagnostics">The diagnostics that occurred during validation.</param>
/// <param name="duration">The amount of time that the parsing operation took.</param>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class CommandValidatorResult(
	bool successful,
	ICommandParserResult parserResult,
	IDiagnosticBag diagnostics,
	TimeSpan duration)
	: ICommandValidatorResult
{
	#region Properties
	/// <inheritdoc/>
	public bool Successful { get; } = successful;

	/// <inheritdoc/>
	public DiagnosticSource Stage => DiagnosticSource.Validation;

	/// <inheritdoc/>
	public ICommandEngine Engine => ParserResult.Engine;

	/// <inheritdoc/>
	public ICommandParserResult ParserResult { get; } = parserResult;

	/// <inheritdoc/>
	public IDiagnosticBag Diagnostics { get; } = diagnostics;

	/// <inheritdoc/>
	public TimeSpan Duration { get; } = duration;
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string typeName = nameof(CommandValidatorResult);
		const string successfulName = nameof(Successful);
		const string durationName = nameof(Duration);

		return $"{typeName} {{ {successfulName} = ({Successful}), {durationName} = ({Duration}) }}";
	}
	#endregion
}
