namespace OwlDomain.CommandLine.Arguments;

/// <summary>
/// 	Represents information about an argument that is linked to a parameter.
/// </summary>
/// <typeparam name="T">The type of the argument's value.</typeparam>
/// <param name="parameter">The parameter that represents the argument.</param>
/// <param name="name">The name of the argument.</param>
/// <param name="position">The position of the argument.</param>
/// <param name="valueInfo">The information about the argument's value.</param>
/// <param name="defaultValueInfo">The information aboue the argument's default value.</param>>
/// <param name="documentation">The documentation for the argument.</param>
public sealed class ParameterArgumentInfo<T>(
	ParameterInfo parameter,
	string name,
	int position,
	IValueInfo<T> valueInfo,
	IDefaultValueInfo? defaultValueInfo,
	IDocumentationInfo? documentation)
	: BaseArgumentInfo<T>(name, position, valueInfo, defaultValueInfo, documentation), IParameterArgumentInfo<T>
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
