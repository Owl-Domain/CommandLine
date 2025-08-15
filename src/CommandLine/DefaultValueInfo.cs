namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents information about the default value of a flag/argument.
/// </summary>
/// <param name="label">The label for the default value.</param>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class DefaultValueInfo(string label) : IDefaultValueInfo
{
	#region Properties
	/// <inheritdoc/>
	public string Label { get; } = label;
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string typeName = nameof(DefaultValueInfo);
		const string labelName = nameof(label);

		return $"{typeName} {{ {labelName} = ({Label}) }}";
	}
	#endregion
}
