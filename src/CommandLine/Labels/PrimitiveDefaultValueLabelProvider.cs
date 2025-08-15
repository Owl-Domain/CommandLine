namespace OwlDomain.CommandLine.Labels;

/// <summary>
/// 	Represents the default value label provider for primitive value types.
/// </summary>
public sealed class PrimitiveDefaultValueLabelProvider : BaseDefaultValueLabelProvider
{
	#region Methods
	/// <inheritdoc/>
	protected override string? TryGet(object? defaultValue)
	{
		if (defaultValue is not null && defaultValue.GetType().IsEnum)
			return defaultValue.ToString();

		return defaultValue switch
		{
			null => "<null>",
			true => "true",
			false => "false",

			byte value => $"{value}",
			ushort value => $"{value:n0}",
			uint value => $"{value:n0}",
			ulong value => $"{value:n0}",

			sbyte value => $"{value}",
			short value => $"{value:n0}",
			int value => $"{value:n0}",
			long value => $"{value:n0}",

			Half value => $"{value}",
			float value => $"{value}",
			double value => $"{value}",
			decimal value => $"{value}",

			// Todo(Nightowl): Improve text type labels by escaping some characters;
			char value => $"'{value}'",
			string value => $"\"{value}\"",

			_ => null,
		};
	}
	#endregion
}
