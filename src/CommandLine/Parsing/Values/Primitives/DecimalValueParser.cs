using System.Globalization;

namespace OwlDomain.CommandLine.Parsing.Values.Primitives;

/// <summary>
/// 	Represents a parser for <see cref="IFloatingPoint{TSelf}"/> values.
/// </summary>
public sealed class DecimalValueParser<T> : BaseValueParser<T>
	where T : IFloatingPoint<T>
{
	#region Methods
	/// <inheritdoc/>
	protected override T? TryParse(IValueParseContext context, ITextParser parser, out string? error)
	{
		string text = parser.AdvanceUntilBreak().Replace("_", "");

		if (T.TryParse(text, NumberStyles.AllowExponent | NumberStyles.AllowTrailingSign, context.Engine.Settings.NumberFormat, out T? result))
		{
			error = default;
			return result;
		}

		error = $"Failed to parse '{text}' as a decimal number.";
		return default;
	}
	#endregion
}
