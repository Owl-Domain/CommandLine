namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents information about the value of a flag/argument.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <param name="isRequired">Whether the value is required.</param>
/// <param name="isNullable">Whether <see langword="null"/> values are allowed.</param>
/// <param name="parser">The parser selected for the value.</param>
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
}
