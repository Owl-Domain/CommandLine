namespace OwlDomain.CommandLine.Parsing.Values.Networking;

/// <summary>
/// 	Represents a parser for the <see cref="Port"/> type.
/// </summary>
public sealed class PortValueParser : BaseValueParser<Port>
{
	#region Methods
	/// <inheritdoc/>
	protected override Port TryParse(IValueParseContext context, ITextParser parser, out string? error)
	{
		string text = parser.AdvanceUntilBreak();

		if (Port.TryParse(text, null, out Port port))
		{
			error = default;
			return port;
		}

		error = $"Failed to parse '{text}' as a port number.";
		return default;
	}
	#endregion
}
