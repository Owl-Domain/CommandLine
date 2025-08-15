namespace OwlDomain.CommandLine.Parsing.Values.Primitives;

/// <summary>
/// 	Represents a generic parser for <see cref="Nullable{T}"/> types.
/// </summary>
/// <typeparam name="T">The non-nullable struct type to parse.</typeparam>
/// <param name="valueParser">The parser for the non-nullable type.</param>
public sealed class NullableValueParser<T>(IValueParser<T> valueParser) : BaseValueParser<T?>
	where T : struct
{
	#region Fields
	private readonly IValueParser<T> _valueParser = valueParser;
	#endregion

	#region Properties
	/// <inheritdoc/>
	protected override bool AllowEmptyValues => true;
	#endregion

	#region Methods
	/// <inheritdoc/>
	protected override T? TryParse(IArgumentValueParseContext context, ITextParser parser, out string? error)
	{
		return TryParse(context, parser, context.Argument.IsNullable, out error);
	}

	/// <inheritdoc/>
	protected override T? TryParse(IFlagValueParseContext context, ITextParser parser, out string? error)
	{
		return TryParse(context, parser, context.Flag.IsNullable, out error);
	}

	private T? TryParse(IValueParseContext context, ITextParser parser, bool isNullable, out string? error)
	{
		error = default;

		if (IsEmptyValue(parser))
		{
			if (isNullable is false)
				error = string.Empty;

			return null;
		}

		IValueParseResult<T> result = _valueParser.Parse(context, parser);
		if (result.Successful)
			return result.Value;

		error = result.Error;
		return default;
	}
	#endregion
}
