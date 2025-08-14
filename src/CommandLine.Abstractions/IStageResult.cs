namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents information about the result of an operation for a specific stage.
/// </summary>
public interface IStageResult
{
	#region Properties
	/// <summary>Whether the operation was successful.</summary>
	bool Successful { get; }

	/// <summary>Whether the operation was cancelled.</summary>
	bool WasCancelled { get; }

	/// <summary>The stage the result is for.</summary>
	DiagnosticSource Stage { get; }

	/// <summary>The engine that was used for the operation.</summary>
	ICommandEngine Engine { get; }

	/// <summary>The diagnostics that occurred during the operation.</summary>
	IDiagnosticBag Diagnostics { get; }

	/// <summary>The amount of time that the operation took.</summary>
	TimeSpan Duration { get; }
	#endregion
}
