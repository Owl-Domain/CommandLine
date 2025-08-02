namespace OwlDomain.CommandLine.Diagnostics;

/// <summary>
/// 	Represents a bag that stores a collection of diagnostics.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class DiagnosticBag : IDiagnosticBag, ICollection<IDiagnostic>
{
	#region Nested types
	private sealed class DiagnosticComparer : IComparer<IDiagnostic>
	{
		#region Properties
		public static DiagnosticComparer Instance = new();
		#endregion

		public int Compare(IDiagnostic? x, IDiagnostic? y)
		{
			if (x is null && y is null) return 0;
			if (x is null) return -1;
			if (y is null) return 1;

			return x.Source.CompareTo(y.Source);
		}
	}
	#endregion

	#region Fields
	private readonly SortedList<IDiagnostic, IDiagnostic> _diagnostics = new(DiagnosticComparer.Instance);
	#endregion

	#region Properties
	/// <inheritdoc/>
	public int Count => _diagnostics.Count;

	/// <inheritdoc/>
	bool ICollection<IDiagnostic>.IsReadOnly => false;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public void Add(IDiagnostic diagnostic) => _diagnostics.Add(diagnostic, diagnostic);

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
	public bool Contains(IDiagnostic item) => _diagnostics.ContainsValue(item);

	/// <inheritdoc/>
	public void CopyTo(IDiagnostic[] array, int arrayIndex) => _diagnostics.Values.CopyTo(array, arrayIndex);

	/// <inheritdoc/>
	public bool Remove(IDiagnostic item) => _diagnostics.Remove(item);

	/// <inheritdoc/>
	public void Clear() => _diagnostics.Clear();

	/// <inheritdoc/>
	public IEnumerator<IDiagnostic> GetEnumerator() => _diagnostics.Values.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_diagnostics.Values).GetEnumerator();
	#endregion

	#region Helpers
	private string DebuggerDisplay()
	{
		const string typeName = nameof(DiagnosticBag);
		const string countName = nameof(Count);

		return $"{typeName} {{ {countName} = ({Count:n0}) }}";
	}
	#endregion
}
