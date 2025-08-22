using System.Globalization;

namespace OwlDomain.CommandLine.Parsing.Values.Primitives;

/// <summary>
/// 	Represents a parser for <see cref="IBinaryInteger{TSelf}"/> values.
/// </summary>
public sealed class IntegerValueParser<T> : BaseValueParser<T>
	where T : IBinaryInteger<T>
{
	#region Methods
	/// <inheritdoc/>
	protected override T? TryParse(IValueParseContext context, ITextParser parser, out string? error)
	{
		NumberFormatInfo format = context.Engine.Settings.NumberFormat;
		string text = parser.AdvanceUntilBreak().Replace("_", "");

		if (text.StartsWith("0x"))
		{
			if (T.TryParse(text.AsSpan(2), NumberStyles.AllowHexSpecifier, format, out T? value))
			{
				error = default;
				return value;
			}

			error = $"Failed to parse '{text}' as a hexadecimal integer.";
			return default;
		}

#if NET8_0_OR_GREATER
		if (text.StartsWith("0b"))
		{
			if (T.TryParse(text.AsSpan(2), NumberStyles.AllowBinarySpecifier, format, out T? value))
			{
				error = default;
				return value;
			}

			error = $"Failed to parse '{text}' as a binary integer.";
			return default;
		}
#endif

		if (T.TryParse(text, NumberStyles.AllowExponent | NumberStyles.AllowTrailingSign, format, out T? result))
		{
			error = default;
			return result;
		}

		error = $"Failed to parse '{text}' as an integer.";
		return default;
	}
	#endregion
}
