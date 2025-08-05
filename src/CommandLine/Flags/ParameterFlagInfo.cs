namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents information about a flag that is linked to a parameter.
/// </summary>
/// <typeparam name="T">The type of the flag's value.</typeparam>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public class ParameterFlagInfo<T> : IParameterFlagInfo<T>
{
	#region Properties
	/// <inheritdoc/>
	public ParameterInfo Parameter { get; }

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
	/// <summary>Creates a new instance of the <see cref="ParameterFlagInfo{T}"/>.</summary>
	/// <param name="parameter">The parameter that represents the flag.</param>
	/// <param name="longName">The long name of the flag.</param>
	/// <param name="shortName">The short name of the flag.</param>
	/// <param name="isRequired">Whether the flag has to be set when executing the command.</param>
	/// <param name="defaultValue">The default value for the flag.</param>
	/// <param name="parser">The value parser selected for the flag.</param>
	public ParameterFlagInfo(ParameterInfo parameter, string? longName, char? shortName, bool isRequired, T? defaultValue, IValueParser<T> parser)
	{
		longName?.ThrowIfEmptyOrWhitespace(nameof(longName));

		if (longName is null && shortName is null)
			Throw.New.ArgumentException(nameof(longName), "Either the long name or the short name of the flag must be specified at a minimum.");

		if (isRequired && defaultValue is not null)
			Throw.New.ArgumentException(nameof(defaultValue), "Default values are not allowed for required flags.");

		Parameter = parameter;
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

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay() => $"Flag {{ LongName = ({LongName}), ShortName = ({ShortName}), ValueType = ({typeof(T)}) }}";
	#endregion
}
