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
	protected override T? TryParse(IValueParseContext context, ITextParser parser, out string? error)
	{
		error = default;

		if (IsEmptyValue(parser))
		{
			if (IsNullable(context) is false)
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
