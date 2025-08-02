namespace OwlDomain.CommandLine.Diagnostics;

/// <summary>
/// 	Represents a bag that stores a collection of diagnostics.
/// </summary>
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
	public void Add(IDiagnostic item) => _diagnostics.Add(item, item);

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
}
