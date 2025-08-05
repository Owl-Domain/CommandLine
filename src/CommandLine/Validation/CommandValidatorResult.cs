namespace OwlDomain.CommandLine.Validation;

/// <summary>
/// 	Represents the validation result for a parsed command.
/// </summary>
/// <param name="successful">Whether the operation was successful.</param>
/// <param name="parserResult">The parsing result that was validated.</param>
/// <param name="diagnostics">The diagnostics that occurred during validation.</param>
/// <param name="duration">The amount of time that the parsing operation took.</param>
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
}
