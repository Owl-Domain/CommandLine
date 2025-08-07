namespace OwlDomain.CommandLine.Parsing.Values.Primitives;

/// <summary>
/// 	Represents a parser for <see langword="string"/> values.
/// </summary>
public sealed class StringValueParser : BaseValueParser<string>
{
	#region Methods
	/// <inheritdoc/>
	protected override string? TryParse(IValueParseContext context, ITextParser parser, out string? error)
	{
		if (parser.IsLazy is false)
		{
			error = null;
			return parser.AdvanceText();
		}

		if (parser.Current is '"')
		{
			error = "Quoted strings are not currently implemented";
			return default;
		}

		string value = parser.AdvanceUntilBreak();

		error = value.Length is 0 ? string.Empty : null;
		return value;
	}
	#endregion
}
