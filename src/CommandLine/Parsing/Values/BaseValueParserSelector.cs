namespace OwlDomain.CommandLine.Parsing.Values;

/// <summary>
/// 	Represents a selector for value parsers.
/// </summary>
public abstract class BaseValueParserSelector : IValueParserSelector
{
	#region Fields
	private readonly Dictionary<Type, WeakReference<IValueParser>> _cache = [];
	#endregion

	#region Properties
	/// <summary>Whether this selector should cache the created value parsers.</summary>
	protected virtual bool AllowCaching => true;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public bool TrySelect(IRootValueParserSelector rootSelector, Type type, [NotNullWhen(true)] out IValueParser? parser)
	{
		if (TryGetCached(type, out parser))
			return true;

		parser = TrySelect(rootSelector, type);

		if (parser is not null)
		{
			if (type.IsAssignableFrom(parser.ValueType) is false)
				Throw.New.InvalidOperationException($"The selected parser ({parser.GetType()}) does not handle values of the required type ({type}).");

			AddToCache(type, parser);
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
	public bool TrySelect(IRootValueParserSelector rootSelector, ParameterInfo parameter, [NotNullWhen(true)] out IValueParser? parser)
	{
		if (TryGetCached(parameter.ParameterType, out parser))
			return true;

		parser = TrySelect(rootSelector, parameter);

		if (parser is not null)
		{
			if (parameter.ParameterType.IsAssignableFrom(parser.ValueType) is false)
				Throw.New.InvalidOperationException($"The selected parser ({parser.GetType()}) does not handle values of the required type ({parameter.ParameterType}).");

			AddToCache(parameter.ParameterType, parser);
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
	public bool TrySelect(IRootValueParserSelector rootSelector, PropertyInfo property, [NotNullWhen(true)] out IValueParser? parser)
	{
		if (TryGetCached(property.PropertyType, out parser))
			return true;

		parser = TrySelect(rootSelector, property);

		if (parser is not null)
		{
			if (property.PropertyType.IsAssignableFrom(parser.ValueType) is false)
				Throw.New.InvalidOperationException($"The selected parser ({parser.GetType()}) does not handle values of the required type ({property.PropertyType}).");

			AddToCache(property.PropertyType, parser);
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

	#region Helpers
	/// <summary>Tries to get the value <paramref name="parser"/> that was cached for the given value <paramref name="type"/>.</summary>
	/// <param name="type">The type of the value that the parser is cached for.</param>
	/// <param name="parser">The cached parser.</param>
	/// <returns>
	/// 	<see langword="true"/> if the cached value <paramref name="parser"/>
	/// 	could be obtained, <see langword="false"/> otherwise.
	/// </returns>
	protected bool TryGetCached(Type type, [NotNullWhen(true)] out IValueParser? parser)
	{
		if (AllowCaching && _cache.TryGetValue(type, out WeakReference<IValueParser>? weakRef) && weakRef.TryGetTarget(out parser))
			return true;

		parser = default;
		return false;
	}

	/// <summary>Adds the given value <paramref name="parser"/> to the cache for the given value <paramref name="type"/>.</summary>
	/// <param name="type">The type of the value to cache the given <paramref name="parser"/> for.</param>
	/// <param name="parser">The value parser to cache.</param>
	/// <remarks>If a value parser is already cached for the given value <paramref name="type"/>, this method will replace it.</remarks>
	protected void AddToCache(Type type, IValueParser parser)
	{
		if (AllowCaching is false)
			return;

		if (_cache.TryGetValue(type, out WeakReference<IValueParser>? weakRef))
		{
			weakRef.SetTarget(parser);
			return;
		}

		weakRef = new(parser);
		_cache.Add(type, weakRef);
	}
	#endregion
}
