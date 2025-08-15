namespace OwlDomain.CommandLine.Labels;

/// <summary>
/// 	Represents the root provider for default flag/argument values.
/// </summary>
/// <param name="providers">The default value label providers that the root provider should use.</param>
public sealed class RootDefaultValueLabelProvider(IReadOnlyList<IDefaultValueLabelProvider> providers) : IRootDefaultValueLabelProvider
{
	#region Fields
	private readonly IReadOnlyList<IDefaultValueLabelProvider> _providers = providers;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public bool TryGet(IEngineSettings settings, ParameterInfo parameter, [NotNullWhen(true)] out string? label)
	{
		foreach (IDefaultValueLabelProvider provider in _providers)
		{
			if (provider.TryGet(this, settings, parameter, out label))
				return true;
		}

		label = default;
		return false;
	}

	/// <inheritdoc/>
	public bool TryGet(IEngineSettings settings, ParameterInfo parameter, object? value, [NotNullWhen(true)] out string? label)
	{
		foreach (IDefaultValueLabelProvider provider in _providers)
		{
			if (provider.TryGet(this, settings, parameter, value, out label))
				return true;
		}

		label = default;
		return false;
	}

	/// <inheritdoc/>
	public bool TryGet(IEngineSettings settings, PropertyInfo property, [NotNullWhen(true)] out string? label)
	{
		foreach (IDefaultValueLabelProvider provider in _providers)
		{
			if (provider.TryGet(this, settings, property, out label))
				return true;
		}

		label = default;
		return false;
	}

	/// <inheritdoc/>
	public bool TryGet(IEngineSettings settings, PropertyInfo property, object? value, [NotNullWhen(true)] out string? label)
	{
		foreach (IDefaultValueLabelProvider provider in _providers)
		{
			if (provider.TryGet(this, settings, property, value, out label))
				return true;
		}

		label = default;
		return false;
	}

	/// <inheritdoc/>
	public bool TryGet(IEngineSettings settings, object? value, [NotNullWhen(true)] out string? label)
	{
		foreach (IDefaultValueLabelProvider provider in _providers)
		{
			if (provider.TryGet(this, settings, value, out label))
				return true;
		}

		label = default;
		return false;
	}
	#endregion
}
