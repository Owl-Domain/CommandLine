namespace OwlDomain.CommandLine.Diagnostics;

/// <summary>
/// 	Represents a diagnostic caused by an exception.
/// </summary>
public interface IExceptionDiagnostic : IDiagnostic
{
	#region Properties
	/// <summary>The exception that caused the diagnostic.</summary>
	Exception Exception { get; }
	#endregion
}

/// <summary>
/// 	Represents a diagnostic caused by an exception.
/// </summary>
/// <param name="source">The source of the diagnostic.</param>
/// <param name="location">The location that the diagnostic is referring to.</param>
/// <param name="exception">The exception that caused the diagnostic.</param>
/// <exception cref="ArgumentOutOfRangeException">
/// 	Thrown if the given <paramref name="source"/> value is not defined in the <see cref="DiagnosticSource"/> <see langword="enum"/>.
/// </exception>
public sealed class ExceptionDiagnostic(
	DiagnosticSource source,
	TextLocation location,
	Exception exception)
	: Diagnostic(source, location, exception.Message), IExceptionDiagnostic
{
	#region Properties
	/// <inheritdoc/>
	public Exception Exception { get; } = exception;
	#endregion
}
