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

		Debug.Assert(parser.Current is not '"', "Quoted string are not currently implemented.");

		string value = parser.AdvanceUntilBreak();

		error = value.Length is 0 ? string.Empty : null;
		return value;
	}
	#endregion
}
