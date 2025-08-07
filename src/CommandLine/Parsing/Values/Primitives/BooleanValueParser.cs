namespace OwlDomain.CommandLine.Parsing.Values.Primitives;

/// <summary>
/// 	Represents a parser for <see langword="bool"/> values.
/// </summary>
public sealed class BooleanValueParser : BaseValueParser<bool>
{
	#region Methods
	/// <inheritdoc/>
	protected override bool TryParse(IValueParseContext context, ITextParser parser, out string? error)
	{
		string text = parser.AdvanceUntilBreak();

		if (MatchAny(text, "true", "yes", "t", "y"))
		{
			error = default;
			return true;
		}

		if (MatchAny(text, "false", "no", "f", "n"))
		{
			error = default;
			return false;
		}

		error = $"Couldn't parse '{text}' as a boolean value.";
		return default;
	}
	#endregion

	#region Helpers
	private static bool MatchAny(string text, params ReadOnlySpan<string> values)
	{
		foreach (string current in values)
		{
			if (text.Equals(current, StringComparison.OrdinalIgnoreCase))
				return true;
		}

		return false;
	}
	#endregion
}
