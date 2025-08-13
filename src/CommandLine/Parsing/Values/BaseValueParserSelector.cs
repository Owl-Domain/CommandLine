namespace OwlDomain.CommandLine.Parsing.Values;

/// <summary>
/// 	Represents a selector for value parsers.
/// </summary>
public abstract class BaseValueParserSelector : IValueParserSelector
{
	#region Methods
	/// <inheritdoc/>
	public bool TrySelect(IRootValueParserSelector rootSelector, Type type, [NotNullWhen(true)] out IValueParser? parser)
	{
		parser = TrySelect(rootSelector, type);

		if (parser is not null)
		{
			if (type.IsAssignableFrom(parser.ValueType) is false)
				Throw.New.InvalidOperationException($"The selected parser ({parser.GetType()}) does not handle values of the required type ({type}).");

			return true;
		}

		return false;
	}

	/// <inheritdoc/>
	public bool TrySelect(IRootValueParserSelector rootSelector, ParameterInfo parameter, [NotNullWhen(true)] out IValueParser? parser)
	{
		parser = TrySelect(rootSelector, parameter);

		if (parser is not null)
		{
			if (parameter.ParameterType.IsAssignableFrom(parser.ValueType) is false)
				Throw.New.InvalidOperationException($"The selected parser ({parser.GetType()}) does not handle values of the required type ({parameter.ParameterType}).");

			return true;
		}

		return false;
	}

	/// <inheritdoc/>
	public bool TrySelect(IRootValueParserSelector rootSelector, PropertyInfo property, [NotNullWhen(true)] out IValueParser? parser)
	{
		parser = TrySelect(rootSelector, property);

		if (parser is not null)
		{
			if (property.PropertyType.IsAssignableFrom(parser.ValueType) is false)
				Throw.New.InvalidOperationException($"The selected parser ({parser.GetType()}) does not handle values of the required type ({property.PropertyType}).");

			return true;
		}

		return false;
	}

	/// <summary>Tries to select a value parser for the given <see langword="property"/>.</summary>
	/// <param name="rootSelector">The selector which can be used to select the parser for nested values.</param>
	/// <param name="parameter">The parameter to try and select a value parser for.</param>
	/// <returns>The selected value parser, or <see langword="null"/> if a parser couldn't be selected.</returns>
	/// <remarks>If you override this you should also override <see cref="TrySelect(IRootValueParserSelector,PropertyInfo)"/>.</remarks>
	protected virtual IValueParser? TrySelect(IRootValueParserSelector rootSelector, ParameterInfo parameter)
	{
		return TrySelect(rootSelector, parameter.ParameterType);
	}

	/// <summary>Tries to select a value parser for the given <see langword="property"/>.</summary>
	/// <param name="rootSelector">The selector which can be used to select the parser for nested values.</param>
	/// <param name="property">The property to try and select a value parser for.</param>
	/// <returns>The selected value parser, or <see langword="null"/> if a parser couldn't be selected.</returns>
	/// <remarks>If you override this you should also override <see cref="TrySelect(IRootValueParserSelector,ParameterInfo)"/>.</remarks>
	protected virtual IValueParser? TrySelect(IRootValueParserSelector rootSelector, PropertyInfo property)
	{
		return TrySelect(rootSelector, property.PropertyType);
	}

	/// <summary>Tries to select a value parser for the given <paramref name="type"/>.</summary>
	/// <param name="rootSelector">The selector which can be used to select the parser for nested values.</param>
	/// <param name="type">The type to try and select a value parser for.</param>
	/// <returns>The selected value parser, or <see langword="null"/> if a parser couldn't be selected.</returns>
	/// <remarks>You should override this if your selector doesn't care about any attributes.</remarks>
	protected virtual IValueParser? TrySelect(IRootValueParserSelector rootSelector, Type type)
	{
		Throw.New.NotImplementedException($"The ({GetType()}) value parser selector didn't implement any selection logic.");
		return default;
	}
	#endregion
}
