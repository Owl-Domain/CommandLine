namespace OwlDomain.CommandLine.Diagnostics;

/// <summary>
/// 	Represents the source of a diagnostic.
/// </summary>
public enum DiagnosticSource : byte
{
	/// <summary>The diagnostic came from the parsing stage.</summary>
	Parsing,

	/// <summary>The diagnostic came from the validation stage.</summary>
	Validation,

	/// <summary>The diagnostic came from the execution stage.</summary>
	Execution
}
