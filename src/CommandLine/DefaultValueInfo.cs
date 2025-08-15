namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents information about the default value of a flag/argument.
/// </summary>
/// <param name="label">The label for the default value.</param>
public sealed class DefaultValueInfo(string label) : IDefaultValueInfo
{
	#region Properties
	/// <inheritdoc/>
	public string Label { get; } = label;
	#endregion
}
