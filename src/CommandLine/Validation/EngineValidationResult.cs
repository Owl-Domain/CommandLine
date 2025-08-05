namespace OwlDomain.CommandLine.Validation;

/// <summary>
/// 	Represents the validation result for a parsed command.
/// </summary>
/// <param name="parseResult">The parsing result that was validated.</param>
/// <param name="diagnostics">The diagnostics that occurred during validation.</param>
public sealed class EngineValidationResult(
	ICommandParserResult parseResult,
	IDiagnosticBag diagnostics)
	: IEngineValidationResult
{
	#region Properties
	/// <inheritdoc/>
	public ICommandParserResult ParseResult { get; } = parseResult;

	/// <inheritdoc/>
	public IDiagnosticBag Diagnostics { get; } = diagnostics;
	#endregion
}
