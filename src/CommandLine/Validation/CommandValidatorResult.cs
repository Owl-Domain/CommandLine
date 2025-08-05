namespace OwlDomain.CommandLine.Validation;

/// <summary>
/// 	Represents the validation result for a parsed command.
/// </summary>
/// <param name="parserResult">The parsing result that was validated.</param>
/// <param name="diagnostics">The diagnostics that occurred during validation.</param>
public sealed class CommandValidatorResult(
	ICommandParserResult parserResult,
	IDiagnosticBag diagnostics)
	: ICommandValidatorResult
{
	#region Properties
	/// <inheritdoc/>
	public DiagnosticSource Stage => DiagnosticSource.Validation;

	/// <inheritdoc/>
	public ICommandEngine Engine => ParserResult.Engine;

	/// <inheritdoc/>
	public ICommandParserResult ParserResult { get; } = parserResult;

	/// <inheritdoc/>
	public IDiagnosticBag Diagnostics { get; } = diagnostics;
	#endregion
}
