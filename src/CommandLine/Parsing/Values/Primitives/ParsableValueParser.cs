namespace OwlDomain.CommandLine.Parsing.Values.Primitives;

#if NET7_0_OR_GREATER

/// <summary>
/// 	Represents a general parser for <see cref="IParsable{TSelf}"/> values.
/// </summary>
public sealed class ParsableValueParser<T> : BaseValueParser<T>
	where T : IParsable<T>
{
	#region Methods
	/// <inheritdoc/>
	protected override T? TryParse(IValueParseContext context, ITextParser parser, out string? error)
	{
		string text = parser.AdvanceUntilBreak();

		if (T.TryParse(text, null, out T? result))
		{
			error = default;
			return result;
		}

		try
		{
			error = default;
			return T.Parse(text, null);
		}
		catch (Exception exception)
		{
			error = exception.Message;
			return default;
		}
	}
	#endregion
}

#endif