namespace OwlDomain.CommandLine.Arguments;

/// <summary>
/// 	Represents information about an argument that is linked to a parameter.
/// </summary>
/// <typeparam name="T">The type of the argument's value.</typeparam>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class ParameterArgumentInfo<T> : IParameterArgumentInfo<T>
{
	#region Properties
	/// <inheritdoc/>
	public ParameterInfo Parameter { get; }

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
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="ParameterArgumentInfo{T}"/>.</summary>
	/// <param name="parameter">The parameter that represents the argument.</param>
	/// <param name="name">The name of the argument.</param>
	/// <param name="position">The position of the argument.</param>
	/// <param name="isRequired">Whether the argument has to be set when executing the command.</param>
	/// <param name="defaultValue">The default value for the argument.</param>
	/// <param name="parser">The value parser selected for the argument.</param>
	public ParameterArgumentInfo(ParameterInfo parameter, string name, int position, bool isRequired, T? defaultValue, IValueParser<T> parser)
	{
		name.ThrowIfEmptyOrWhitespace(nameof(name));
		position.ThrowIfLessThan(0, nameof(position));

		if (isRequired && defaultValue is not null)
			Throw.New.ArgumentException(nameof(defaultValue), "Default values are not allowed for required arguments.");

		Parameter = parameter;
		Name = name;
		Position = position;
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
	private string DebuggerDisplay() => $"Argument {{ Name = ({Name}), ValueType = ({typeof(T)}) }}";
	#endregion
}
