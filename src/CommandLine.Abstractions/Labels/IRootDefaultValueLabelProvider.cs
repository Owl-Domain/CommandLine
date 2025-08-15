namespace OwlDomain.CommandLine.Labels;

/// <summary>
/// 	Represents the root provider for default flag/argument values.
/// </summary>
public interface IRootDefaultValueLabelProvider
{
	#region Methods
	/// <summary>Tries to get the label for the default value of the given <paramref name="parameter"/>.</summary>
	/// <param name="settings">The command engine settings.</param>
	/// <param name="parameter">The parameter to get the default value label for.</param>
	/// <param name="label">The obtained label for the default value of the given <paramref name="parameter"/>.</param>
	/// <returns>
	/// 	<see langword="true"/> if the default value <paramref name="label"/>
	/// 	could be obtained, <see langword="false"/> otherwise.
	/// </returns>
	bool TryGet(IEngineSettings settings, ParameterInfo parameter, [NotNullWhen(true)] out string? label);

	/// <summary>Tries to get the label for the default value of the given <paramref name="parameter"/>.</summary>
	/// <param name="settings">The command engine settings.</param>
	/// <param name="parameter">The parameter to get the default value label for.</param>
	/// <param name="value">The default value for the given <paramref name="parameter"/>.</param>
	/// <param name="label">The obtained label for the default value of the given <paramref name="parameter"/>.</param>
	/// <returns>
	/// 	<see langword="true"/> if the default value <paramref name="label"/>
	/// 	could be obtained, <see langword="false"/> otherwise.
	/// </returns>
	bool TryGet(IEngineSettings settings, ParameterInfo parameter, object? value, [NotNullWhen(true)] out string? label);

	/// <summary>Tries to get the label for the default value of the given <paramref name="property"/>.</summary>
	/// <param name="settings">The command engine settings.</param>
	/// <param name="property">The property to get the default value label for.</param>
	/// <param name="label">The obtained label for the default value of the given <paramref name="property"/>.</param>
	/// <returns>
	/// 	<see langword="true"/> if the default value <paramref name="label"/>
	/// 	could be obtained, <see langword="false"/> otherwise.
	/// </returns>
	bool TryGet(IEngineSettings settings, PropertyInfo property, [NotNullWhen(true)] out string? label);

	/// <summary>Tries to get the label for the default value of the given <paramref name="property"/>.</summary>
	/// <param name="settings">The command engine settings.</param>
	/// <param name="property">The property to get the default value label for.</param>
	/// <param name="value">The default value for the given <paramref name="property"/>.</param>
	/// <param name="label">The obtained label for the default value of the given <paramref name="property"/>.</param>
	/// <returns>
	/// 	<see langword="true"/> if the default value <paramref name="label"/>
	/// 	could be obtained, <see langword="false"/> otherwise.
	/// </returns>
	bool TryGet(IEngineSettings settings, PropertyInfo property, object? value, [NotNullWhen(true)] out string? label);

	/// <summary>Tries to get the label for the given default <paramref name="value"/>.</summary>
	/// <param name="settings">The command engine settings.</param>
	/// <param name="value">The default value to get the label for.</param>
	/// <param name="label">The obtained label for the given default <paramref name="value"/>.</param>
	/// <returns>
	/// 	<see langword="true"/> if the default <paramref name="value"/> <paramref name="label"/>
	/// 	could be obtained, <see langword="false"/> otherwise.
	/// </returns>
	bool TryGet(IEngineSettings settings, object? value, [NotNullWhen(true)] out string? label);
	#endregion
}
