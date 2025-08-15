namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents information about a flag that is linked to a parameter.
/// </summary>
/// <typeparam name="T">The type of the flag's value.</typeparam>
/// <param name="parameter">The parameter that represents the flag.</param>
/// <param name="kind">The type of the flag.</param>
/// <param name="longName">The long name of the flag.</param>
/// <param name="shortName">The short name of the flag.</param>
/// <param name="isRequired">Whether the flag has to be set when executing the command.</param>
/// <param name="isNullable">Whether the argument allows <see langword="null"/> values.</param>
/// <param name="defaultValue">The default value for the flag.</param>
/// <param name="parser">The value parser selected for the flag.</param>
/// <param name="documentation">The documentation for the flag.</param>
/// <param name="defaultValueLabel">The label for the <paramref name="defaultValue"/>.</param>
public class ParameterFlagInfo<T>(
	ParameterInfo parameter,
	FlagKind kind,
	string? longName,
	char? shortName,
	bool isRequired,
	bool isNullable,
	T? defaultValue,
	IValueParser<T> parser,
	IDocumentationInfo? documentation,
	string? defaultValueLabel)
	: BaseFlagInfo<T>(kind, longName, shortName, isRequired, isNullable, defaultValue, parser, documentation, defaultValueLabel), IParameterFlagInfo<T>
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
