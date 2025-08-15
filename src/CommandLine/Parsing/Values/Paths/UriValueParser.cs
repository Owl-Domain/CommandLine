namespace OwlDomain.CommandLine.Parsing.Values.Paths;

/// <summary>
/// 	Represents a parser for the <see cref="Uri"/> type.
/// </summary>
public sealed class UriValueParser : BaseValueParser<Uri>
{
	#region Methods
	/// <inheritdoc/>
	protected override Uri? TryParse(IValueParseContext context, ITextParser parser, out string? error)
	{
		string text = parser.AdvanceUntilBreak();
		try
		{
			error = default;
			return new(text, UriKind.RelativeOrAbsolute);
		}
		catch (Exception exception)
		{
			error = exception.Message;
			return default;
		}
	}
	#endregion
}
