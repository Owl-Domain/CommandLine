namespace OwlDomain.CommandLine.Diagnostics;

/// <summary>
/// 	Represents a bag that stores a collection of diagnostics.
/// </summary>
public interface IDiagnosticBag : IReadOnlyCollection<IDiagnostic>
{
	#region Properties
	/// <summary>Gets the diagnostic that were caused by exceptions.</summary>
	IEnumerable<IExceptionDiagnostic> Exceptions { get; }
	#endregion
}
