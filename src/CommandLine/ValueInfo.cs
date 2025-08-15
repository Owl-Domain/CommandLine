namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents information about the value of a flag/argument.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <param name="isRequired">Whether the value is required.</param>
/// <param name="isNullable">Whether <see langword="null"/> values are allowed.</param>
/// <param name="parser">The parser selected for the value.</param>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class ValueInfo<T>(bool isRequired, bool isNullable, IValueParser<T> parser) : IValueInfo<T>
{
	#region Properties
	/// <inheritdoc/>
	public bool IsRequired { get; } = isRequired;

	/// <inheritdoc/>
	public bool IsNullable { get; } = isNullable;

	/// <inheritdoc/>
	public IValueParser<T> Parser { get; } = parser;
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string typeName = nameof(ValueInfo<T>);
		const string isRequiredName = nameof(IsRequired);
		const string isNullableName = nameof(IsNullable);
		const string parserName = nameof(Parser);

		return $"{typeName} {{ {isRequiredName} = ({IsRequired}), {isNullableName} = ({IsNullable}), {parserName} = ({Parser}) }}";
	}
	#endregion
}
