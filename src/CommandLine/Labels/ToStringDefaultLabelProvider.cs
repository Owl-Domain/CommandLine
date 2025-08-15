namespace OwlDomain.CommandLine.Labels;

/// <summary>
/// 	Represents a default value label provider that tires to use the <see cref="object.ToString"/> method.
/// </summary>
/// <remarks>
/// 	This label provider will try to only use the <see cref="object.ToString"/> method
/// 	if it's different from the default implementation which returns the type name.
/// </remarks>
public sealed class ToStringDefaultLabelProvider : BaseDefaultValueLabelProvider
{
	#region Methods
	/// <inheritdoc/>
	protected override string? TryGet(IRootDefaultValueLabelProvider rootProvider, IEngineSettings settings, object? value)
	{
		// Note(Nightowl): This case should've been handled before anyway;
		if (value is null) return null;

		string? typeName = value.GetType().ToString();
		string? label = value.ToString();

		if (label is null || typeName is null)
			return null;

		if (label.Equals(typeName, StringComparison.OrdinalIgnoreCase))
			return null;

		return label;
	}
	#endregion
}
