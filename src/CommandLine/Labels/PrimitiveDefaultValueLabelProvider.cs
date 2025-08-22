namespace OwlDomain.CommandLine.Labels;

/// <summary>
/// 	Represents the default value label provider for primitive value types.
/// </summary>
public sealed class PrimitiveDefaultValueLabelProvider : BaseDefaultValueLabelProvider
{
	#region Methods
	/// <inheritdoc/>
	protected override string? TryGet(IRootDefaultValueLabelProvider rootProvider, IEngineSettings settings, object? defaultValue)
	{
		if (defaultValue is not null && defaultValue.GetType().IsEnum)
			return defaultValue.ToString();

		return defaultValue switch
		{
			null => "<null>",
			true => "true",
			false => "false",

			byte value => value.ToString(settings.NumberFormat),
			ushort value => value.ToString(settings.NumberFormat),
			uint value => value.ToString(settings.NumberFormat),
			ulong value => value.ToString(settings.NumberFormat),

			sbyte value => value.ToString(settings.NumberFormat),
			short value => value.ToString(settings.NumberFormat),
			int value => value.ToString(settings.NumberFormat),
			long value => value.ToString(settings.NumberFormat),

			Half value => value.ToString(settings.NumberFormat),
			float value => value.ToString(settings.NumberFormat),
			double value => value.ToString(settings.NumberFormat),
			decimal value => value.ToString(settings.NumberFormat),

			// Todo(Nightowl): Improve text type labels by escaping some characters;
			char value => $"'{value}'",
			string value => $"\"{value}\"",

			_ => null,
		};
	}
	#endregion
}
