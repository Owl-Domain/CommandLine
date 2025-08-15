using System.Text;

namespace OwlDomain.CommandLine.Labels;

/// <summary>
/// 	Represents the default value label provider for collection types.
/// </summary>
public sealed class CollectionDefaultValueLabelProvider : BaseDefaultValueLabelProvider
{
	#region Nested types
	private delegate bool TryGetLabelDelegate(IEngineSettings settings, object? value, [NotNullWhen(true)] out string? label);
	#endregion

	#region Methods
	/// <inheritdoc/>
	protected override string? TryGet(IRootDefaultValueLabelProvider rootProvider, IEngineSettings settings, ParameterInfo parameter, object? value)
	{
		bool Callback(IEngineSettings settings, object? value, [NotNullWhen(true)] out string? label)
		{
			return rootProvider.TryGet(settings, parameter, value, out label);
		}

		return GetLabel(settings, Callback, value);
	}

	/// <inheritdoc/>
	protected override string? TryGet(IRootDefaultValueLabelProvider rootProvider, IEngineSettings settings, PropertyInfo property, object? value)
	{
		bool Callback(IEngineSettings settings, object? value, [NotNullWhen(true)] out string? label)
		{
			return rootProvider.TryGet(settings, property, value, out label);
		}

		return GetLabel(settings, Callback, value);
	}

	/// <inheritdoc/>
	protected override string? TryGet(IRootDefaultValueLabelProvider rootProvider, IEngineSettings settings, object? value)
	{
		return GetLabel(settings, rootProvider.TryGet, value);
	}

	private static string? GetLabel(
		IEngineSettings settings,
		TryGetLabelDelegate labelDelegate,
		object? defaultValue)
	{
		if (defaultValue is null)
			return null;

		if (defaultValue is not IEnumerable enumerable)
			return null;

		StringBuilder builder = new(settings.ListPrefix);

		string separator = settings.ListValueSeparator;
		Debug.Assert(separator.Length > 0);
		bool addSpace = separator.Any(char.IsWhiteSpace) is false;

		bool hadFirst = false;
		foreach (object? value in enumerable)
		{
			if (hadFirst)
			{
				builder.Append(separator);
				if (addSpace)
					builder.Append(' ');
			}
			else
				hadFirst = true;

			if (labelDelegate.Invoke(settings, value, out string? valueLabel) is false)
				return null;

			builder.Append(valueLabel);
		}

		builder.Append(settings.ListSuffix);

		string label = builder.ToString();
		return label;
	}
	#endregion
}
