namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents information about the result of an operation for a specific stage.
/// </summary>
public interface IStageResult
{
	#region Properties
	/// <summary>The stage the result is for.</summary>
	DiagnosticSource Stage { get; }

	/// <summary>The engine that was used for the operation.</summary>
	ICommandEngine Engine { get; }

	/// <summary>The diagnostics that occurred during the operation.</summary>
	IDiagnosticBag Diagnostics { get; }
	#endregion
}
