namespace OwlDomain.CommandLine.Labels;

/// <summary>
/// 	Represents the base implementation for a default value label provider.
/// </summary>
public abstract class BaseDefaultValueLabelProvider : IDefaultValueLabelProvider
{
	#region Methods
	/// <inheritdoc/>
	public bool TryGet(ParameterInfo parameter, [NotNullWhen(true)] out string? label)
	{
		label = TryGet(parameter);
		return label is not null;
	}

	/// <inheritdoc/>
	public bool TryGet(ParameterInfo parameter, object? value, [NotNullWhen(true)] out string? label)
	{
		label = TryGet(parameter, value);
		return label is not null;
	}

	/// <inheritdoc/>
	public bool TryGet(PropertyInfo property, [NotNullWhen(true)] out string? label)
	{
		label = TryGet(property);
		return label is not null;
	}

	/// <inheritdoc/>
	public bool TryGet(PropertyInfo property, object? value, [NotNullWhen(true)] out string? label)
	{
		label = TryGet(property, value);
		return label is not null;
	}

	/// <inheritdoc/>
	public bool TryGet(object? value, [NotNullWhen(true)] out string? label)
	{
		label = TryGet(value);
		return label is not null;
	}


	/// <summary>Tries to get the label for the default value of the given <paramref name="parameter"/>.</summary>
	/// <param name="parameter">The parameter to try and get the default value label for.</param>
	/// <returns>
	/// 	The label for the default value of the given <paramref name="parameter"/>,
	/// 	or <see langword="null"/> if a label couldn't be obtained.
	/// </returns>
	/// <remarks>This method should not try and get the actual default value for the given <paramref name="parameter"/>.</remarks>
	protected virtual string? TryGet(ParameterInfo parameter) => null;

	/// <summary>Tries to get the label for the default value of the given <paramref name="parameter"/>.</summary>
	/// <param name="parameter">The parameter to try and get the default value label for.</param>
	/// <param name="value">The default value for the given <paramref name="parameter"/>.</param>
	/// <returns>
	/// 	The label for the default value of the given <paramref name="parameter"/>,
	/// 	or <see langword="null"/> if a label couldn't be obtained.
	/// </returns>
	protected virtual string? TryGet(ParameterInfo parameter, object? value) => null;

	/// <summary>Tries to get the label for the default value of the given <paramref name="property"/>.</summary>
	/// <param name="property">The property to try and get the default value label for.</param>
	/// <returns>
	/// 	The label for the default value of the given <paramref name="property"/>,
	/// 	or <see langword="null"/> if a label couldn't be obtained.
	/// </returns>
	/// <remarks>This method should not try and get the actual default value for the given <paramref name="property"/>.</remarks>
	protected virtual string? TryGet(PropertyInfo property) => null;

	/// <summary>Tries to get the label for the default value of the given <paramref name="property"/>.</summary>
	/// <param name="property">The property to try and get the default value label for.</param>
	/// <param name="value">The default value for the given <paramref name="property"/>.</param>
	/// <returns>
	/// 	The label for the default value of the given <paramref name="property"/>,
	/// 	or <see langword="null"/> if a label couldn't be obtained.
	/// </returns>
	protected virtual string? TryGet(PropertyInfo property, object? value) => null;

	/// <summary>Tries to get the label for the given default <paramref name="value"/>.</summary>
	/// <param name="value">The default value to try and get the label for.</param>
	/// <returns>
	/// 	The label for the given default <paramref name="value"/>,
	/// 	or <see langword="null"/> if a label couldn't be obtained.
	/// </returns>
	protected virtual string? TryGet(object? value) => null;
	#endregion
}
