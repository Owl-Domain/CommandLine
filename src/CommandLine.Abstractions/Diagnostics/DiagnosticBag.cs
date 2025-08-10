namespace OwlDomain.CommandLine.Diagnostics;

/// <summary>
/// 	Represents a bag that stores a collection of diagnostics.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class DiagnosticBag : IDiagnosticBag, ICollection<IDiagnostic>
{
	#region Fields
	private readonly List<IDiagnostic> _diagnostics = [];
	#endregion

	#region Properties
	/// <inheritdoc/>
	public int Count => _diagnostics.Count;

	/// <inheritdoc/>
	bool ICollection<IDiagnostic>.IsReadOnly => false;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public void Add(IDiagnostic diagnostic) => _diagnostics.Add(diagnostic);

	/// <summary>Creates a new diagnostic and adds it to the bag.</summary>
	/// <param name="source">The source of the diagnostic.</param>
	/// <param name="location">The location that the diagnostic is referring to.</param>
	/// <param name="message">The diagnostic message.</param>
	public void Add(DiagnosticSource source, TextLocation location, string message)
	{
		Diagnostic diagnostic = new(source, location, message);
		Add(diagnostic);
	}

	/// <inheritdoc/>
	public bool Contains(IDiagnostic item) => _diagnostics.Contains(item);

	/// <inheritdoc/>
	public void CopyTo(IDiagnostic[] array, int arrayIndex) => _diagnostics.CopyTo(array, arrayIndex);

	/// <inheritdoc/>
	public bool Remove(IDiagnostic item) => _diagnostics.Remove(item);

	/// <inheritdoc/>
	public void Clear() => _diagnostics.Clear();

	/// <inheritdoc/>
	public IEnumerator<IDiagnostic> GetEnumerator() => _diagnostics.OrderBy(d => d.Source).GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_diagnostics.OrderBy(d => d.Source)).GetEnumerator();
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string typeName = nameof(DiagnosticBag);
		const string countName = nameof(Count);

		return $"{typeName} {{ {countName} = ({Count:n0}) }}";
	}
	#endregion
}
