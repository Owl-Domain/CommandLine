namespace OwlDomain.CommandLine.Parsing.Values;

/// <summary>
/// 	Represents the root selector for value parsers.
/// </summary>
/// <param name="selectors">The registered value parser selectors.</param>
public sealed class RootValueParserSelector(IReadOnlyList<IValueParserSelector> selectors) : IRootValueParserSelector
{
	#region Fields
	private readonly IReadOnlyList<IValueParserSelector> _selectors = selectors;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public bool TrySelect(Type type, [NotNullWhen(true)] out IValueParser? parser)
	{
		foreach (IValueParserSelector selector in _selectors)
		{
			if (selector.TrySelect(this, type, out parser))
				return true;
		}

		parser = default;
		return false;
	}

	/// <inheritdoc/>
	public bool TrySelect(ParameterInfo parameter, [NotNullWhen(true)] out IValueParser? parser)
	{
		foreach (IValueParserSelector selector in _selectors)
		{
			if (selector.TrySelect(this, parameter, out parser))
				return true;
		}

		parser = default;
		return false;
	}

	/// <inheritdoc/>
	public bool TrySelect(PropertyInfo property, [NotNullWhen(true)] out IValueParser? parser)
	{
		foreach (IValueParserSelector selector in _selectors)
		{
			if (selector.TrySelect(this, property, out parser))
				return true;
		}

		parser = default;
		return false;
	}
	#endregion
}
