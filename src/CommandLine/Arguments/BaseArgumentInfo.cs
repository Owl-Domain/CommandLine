namespace OwlDomain.CommandLine.Arguments;

/// <summary>
/// 	Represents the base implementation for an engine command's argument.
/// </summary>
/// <typeparam name="T">The type of the argument's value.</typeparam>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public abstract class BaseArgumentInfo<T> : IArgumentInfo<T>
{
	#region Properties
	/// <inheritdoc/>
	public string Name { get; }

	/// <inheritdoc/>
	public int Position { get; }

	/// <inheritdoc/>
	public bool IsRequired { get; }

	/// <inheritdoc/>
	public T? DefaultValue { get; }

	/// <inheritdoc/>
	public IValueParser<T> Parser { get; }

	/// <inheritdoc/>
	public IDocumentationInfo? Documentation { get; }

	/// <inheritdoc/>
	public string? DefaultValueLabel { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="BaseArgumentInfo{T}"/>.</summary>
	/// <param name="name">The name of the argument.</param>
	/// <param name="position">The position of the argument.</param>
	/// <param name="isRequired">Whether the argument has to be set when executing the command.</param>
	/// <param name="defaultValue">The default value for the argument.</param>
	/// <param name="parser">The value parser selected for the argument.</param>
	/// <param name="documentation">The documentation for the argument.</param>
	/// <param name="defaultValueLabel">The label for the <paramref name="defaultValue"/>.</param>
	protected BaseArgumentInfo(
		string name,
		int position,
		bool isRequired,
		T? defaultValue,
		IValueParser<T> parser,
		IDocumentationInfo? documentation,
		string? defaultValueLabel)
	{
		name.ThrowIfEmptyOrWhitespace(nameof(name));
		position.ThrowIfLessThan(0, nameof(position));

		if (isRequired && defaultValue is not null)
			Throw.New.ArgumentException(nameof(defaultValue), "Default values are not allowed for required arguments.");

		Name = name;
		Position = position;
		IsRequired = isRequired;
		DefaultValue = defaultValue;
		Parser = parser;
		Documentation = documentation;
		DefaultValueLabel = defaultValueLabel ?? (isRequired ? null : defaultValue?.ToString()); ;
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay() => $"Argument {{ Name = ({Name}), ValueType = ({typeof(T)}) }}";
	#endregion
}
