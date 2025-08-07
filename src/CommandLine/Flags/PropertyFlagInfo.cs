namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents information about a flag that is linked to a property.
/// </summary>
/// <typeparam name="T">The type of the flag's value.</typeparam>
/// <param name="property">The property that represents the flag.</param>
/// <param name="kind">The type of the flag.</param>
/// <param name="longName">The long name of the flag.</param>
/// <param name="shortName">The short name of the flag.</param>
/// <param name="isRequired">Whether the flag has to be set when executing the command.</param>
/// <param name="defaultValue">The default value for the flag.</param>
/// <param name="parser">The value parser selected for the flag.</param>
public class PropertyFlagInfo<T>(
	PropertyInfo property,
	FlagKind kind,
	string? longName,
	char? shortName,
	bool isRequired,
	T? defaultValue,
	IValueParser<T> parser)
	: BaseFlagInfo<T>(kind, longName, shortName, isRequired, defaultValue, parser), IPropertyFlagInfo<T>
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
