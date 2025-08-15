namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents information about a flag that is linked to a property.
/// </summary>
/// <typeparam name="T">The type of the flag's value.</typeparam>
/// <param name="property">The property that represents the flag.</param>
/// <param name="kind">The type of the flag.</param>
/// <param name="longName">The long name of the flag.</param>
/// <param name="shortName">The short name of the flag.</param>
/// <param name="valueInfo">The information about the flag's value.</param>
/// <param name="defaultValueInfo">The information aboue the flag's default value.</param>
/// <param name="documentation">The documentation for the flag.</param>
public class PropertyFlagInfo<T>(
	PropertyInfo property,
	FlagKind kind,
	string? longName,
	char? shortName,
	IValueInfo<T> valueInfo,
	IDefaultValueInfo? defaultValueInfo,
	IDocumentationInfo? documentation)
	: BaseFlagInfo<T>(kind, longName, shortName, valueInfo, defaultValueInfo, documentation), IPropertyFlagInfo<T>
{
	#region Properties
	/// <inheritdoc/>
	public PropertyInfo Property { get; } = property;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public TAttribute? GetAttribute<TAttribute>()
		where TAttribute : Attribute
	{
		return Property.GetCustomAttribute<TAttribute>();
	}

	/// <inheritdoc/>
	public bool TryGetAttribute<TAttribute>([NotNullWhen(true)] out TAttribute? attribute)
		where TAttribute : Attribute
	{
		return Property.TryGetCustomAttribute(out attribute);
	}

	/// <inheritdoc/>
	public IEnumerable<TAttribute> GetAttributes<TAttribute>()
		where TAttribute : Attribute
	{
		return Property.GetCustomAttributes<TAttribute>();
	}

	/// <inheritdoc/>
	public bool TryGetAttributes<TAttribute>([NotNullWhen(true)] out IEnumerable<TAttribute>? attributes)
		where TAttribute : Attribute
	{
		return Property.TryGetCustomAttributes(out attributes);
	}
	#endregion
}
