namespace OwlDomain.CommandLine.Diagnostics;

/// <summary>
/// 	Represents a diagnostic.
/// </summary>
public interface IDiagnostic
{
	#region Properties
	/// <summary>The source of the diagnostic.</summary>
	DiagnosticSource Source { get; }

	/// <summary>The location that the diagnostic is referring to.</summary>
	TextLocation Location { get; }

	/// <summary>The diagnostic message.</summary>
	string Message { get; }
	#endregion
}
