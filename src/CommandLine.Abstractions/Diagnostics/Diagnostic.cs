namespace OwlDomain.CommandLine.Diagnostics;

/// <summary>
/// 	Represents a diagnostic.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public class Diagnostic : IDiagnostic
{
	#region Properties
	/// <inheritdoc/>
	public DiagnosticSource Source { get; }

	/// <inheritdoc/>
	public TextLocation Location { get; }

	/// <inheritdoc/>
	public string Message { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="Diagnostic"/>.</summary>
	/// <param name="source">The source of the diagnostic.</param>
	/// <param name="location">The location that the diagnostic is referring to.</param>
	/// <param name="message">The diagnostic message.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// 	Thrown if the given <paramref name="source"/> value is not defined in the <see cref="DiagnosticSource"/> <see langword="enum"/>.
	/// </exception>
	public Diagnostic(DiagnosticSource source, TextLocation location, string message)
	{
		source.ThrowIfNotDefined(nameof(source));

		Source = source;
		Location = location;
		Message = message;
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		string typeName = GetType().Name;
		const string sourceName = nameof(Source);
		const string messageName = nameof(Message);

		return $"{typeName} {{ {sourceName} = ({Source}), {messageName} = ({Message}) }}";
	}
	#endregion
}
