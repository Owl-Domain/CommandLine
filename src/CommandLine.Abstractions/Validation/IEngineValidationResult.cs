namespace OwlDomain.CommandLine.Validation;

/// <summary>
/// 	Represents the validation result for a parsed command.
/// </summary>
public interface IEngineValidationResult
{
	#region Properties
	/// <summary>The parsing result that was validated.</summary>
	ICommandParserResult ParseResult { get; }

	/// <summary>The diagnostics that occurred during validation.</summary>
	IDiagnosticBag Diagnostics { get; }
	#endregion
}
