namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents information about a flag that is linked to a parameter.
/// </summary>
/// <typeparam name="T">The type of the flag's value.</typeparam>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public class PropertyFlagInfo<T> : IPropertyFlagInfo<T>
{
	#region Properties
	/// <inheritdoc/>
	public PropertyInfo Property { get; }

	/// <inheritdoc/>
	public string? LongName { get; }

	/// <inheritdoc/>
	public char? ShortName { get; }

	/// <inheritdoc/>
	public bool IsRequired { get; }

	/// <inheritdoc/>
	public T? DefaultValue { get; }

	/// <inheritdoc/>
	public IValueParser<T> Parser { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="PropertyFlagInfo{T}"/>.</summary>
	/// <param name="property">The property that represents the flag.</param>
	/// <param name="longName">The long name of the flag.</param>
	/// <param name="shortName">The short name of the flag.</param>
	/// <param name="isRequired">Whether the flag has to be set when executing the command.</param>
	/// <param name="defaultValue">The default value for the flag.</param>
	/// <param name="parser">The value parser selected for the flag.</param>
	public PropertyFlagInfo(PropertyInfo property, string? longName, char? shortName, bool isRequired, T? defaultValue, IValueParser<T> parser)
	{
		longName?.ThrowIfEmptyOrWhitespace(nameof(longName));

		if (longName is null && shortName is null)
			Throw.New.ArgumentException(nameof(longName), "Either the long name or the short name of the flag must be specified at a minimum.");

		if (isRequired && defaultValue is not null)
			Throw.New.ArgumentException(nameof(defaultValue), "Default values are not allowed for required flags.");

		Property = property;
		LongName = longName;
		ShortName = shortName;
		IsRequired = isRequired;
		DefaultValue = defaultValue;
		Parser = parser;
	}
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

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay() => $"Flag {{ LongName = ({LongName}), ShortName = ({ShortName}), ValueType = ({typeof(T)}) }}";
	#endregion
}
