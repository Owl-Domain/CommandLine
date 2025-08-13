namespace OwlDomain.CommandLine.Parsing.Values;

/// <summary>
/// 	Represents the root selector for value parsers.
/// </summary>
public interface IRootValueParserSelector
{
	#region Methods
	/// <summary>Tries to select a value parser for the given <paramref name="type"/>.</summary>
	/// <param name="type">The type to select the value parser for.</param>
	/// <param name="parser">The value parser selected for the given <paramref name="type"/>.</param>
	/// <returns><see langword="true"/> if the value <paramref name="parser"/> could be selected, <see langword="false"/> otherwise.</returns>
	bool TrySelect(Type type, [NotNullWhen(true)] out IValueParser? parser);

	/// <summary>Tries to select a value parser for the given <paramref name="parameter"/>.</summary>
	/// <param name="parameter">The parameter to select the value parser for.</param>
	/// <param name="parser">The value parser selected for the given <paramref name="parameter"/>.</param>
	/// <returns><see langword="true"/> if the value <paramref name="parser"/> could be selected, <see langword="false"/> otherwise.</returns>
	bool TrySelect(ParameterInfo parameter, [NotNullWhen(true)] out IValueParser? parser);

	/// <summary>Tries to select a value parser for the given <paramref name="property"/>.</summary>
	/// <param name="property">The property to select the value parser for.</param>
	/// <param name="parser">The value parser selected for the given <paramref name="property"/>.</param>
	/// <returns><see langword="true"/> if the value <paramref name="parser"/> could be selected, <see langword="false"/> otherwise.</returns>
	bool TrySelect(PropertyInfo property, [NotNullWhen(true)] out IValueParser? parser);
	#endregion
}
