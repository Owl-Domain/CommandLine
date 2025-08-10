namespace OwlDomain.CommandLine.Arguments;

/// <summary>
/// 	Represents information about an argument that is linked to a parameter.
/// </summary>
/// <typeparam name="T">The type of the argument's value.</typeparam>
/// <param name="parameter">The parameter that represents the argument.</param>
/// <param name="name">The name of the argument.</param>
/// <param name="position">The position of the argument.</param>
/// <param name="isRequired">Whether the argument has to be set when executing the command.</param>
/// <param name="defaultValue">The default value for the argument.</param>
/// <param name="parser">The value parser selected for the argument.</param>
/// <param name="documentation">The documentation for the argument.</param>
/// <param name="defaultValueLabel">The label for the <paramref name="defaultValue"/>.</param>
public sealed class ParameterArgumentInfo<T>(
	ParameterInfo parameter,
	string name,
	int position,
	bool isRequired,
	T? defaultValue,
	IValueParser<T> parser,
	IDocumentationInfo? documentation,
	string? defaultValueLabel)
	: BaseArgumentInfo<T>(name, position, isRequired, defaultValue, parser, documentation, defaultValueLabel), IParameterArgumentInfo<T>
{
	#region Properties
	/// <inheritdoc/>
	public ParameterInfo Parameter { get; } = parameter;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public TAttribute? GetAttribute<TAttribute>()
			where TAttribute : Attribute
	{
		return Parameter.GetCustomAttribute<TAttribute>();
	}

	/// <inheritdoc/>
	public bool TryGetAttribute<TAttribute>([NotNullWhen(true)] out TAttribute? attribute)
		where TAttribute : Attribute
	{
		return Parameter.TryGetCustomAttribute(out attribute);
	}

	/// <inheritdoc/>
	public IEnumerable<TAttribute> GetAttributes<TAttribute>()
		where TAttribute : Attribute
	{
		return Parameter.GetCustomAttributes<TAttribute>();
	}

	/// <inheritdoc/>
	public bool TryGetAttributes<TAttribute>([NotNullWhen(true)] out IEnumerable<TAttribute>? attributes)
		where TAttribute : Attribute
	{
		return Parameter.TryGetCustomAttributes(out attributes);
	}
	#endregion
}
