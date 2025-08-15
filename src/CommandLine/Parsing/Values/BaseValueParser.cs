namespace OwlDomain.CommandLine.Parsing.Values;

/// <summary>
/// 	Represents the base implementation for a value parser.
/// </summary>
/// <typeparam name="T">The type of the value that will be parsed.</typeparam>
public abstract class BaseValueParser<T> : IValueParser<T>
{
	#region Properties
	/// <summary>Whether the parser should allow for empty values to be handled.</summary>
	/// <remarks>
	/// 	Empty values <b>not</b> the same as missing values. Empty values are a
	/// 	special case where the user specifically passed in an empty value.
	/// </remarks>
	protected virtual bool AllowEmptyValues => false;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public IValueParseResult<T> Parse(IValueParseContext context, ITextParser parser)
	{
		context.CancellationToken.ThrowIfCancellationRequested();

		T? value;
		string? error;
		TextPoint start = parser.Point;

		if (IsValueMissing(parser))
		{
			value = default;
			error = string.Empty;
		}
		else if (AllowEmptyValues is false && IsEmptyValue(parser))
		{
			error = IsNullable(context) ? default : string.Empty;
			value = default;
		}
		else if (context is IFlagValueParseContext flag)
			value = TryParse(flag, parser, out error);
		else if (context is IArgumentValueParseContext argument)
			value = TryParse(argument, parser, out error);
		else
		{
			Throw.New.ArgumentException(nameof(context), $"Unknown value parse context type ({context?.GetType()}).");
			error = default;
			value = default;
		}

		TextPoint end = parser.Point;

		if (error is not null)
			return new ValueParseResult<T>(context, new(start, end), error);

		return new ValueParseResult<T>(context, new(start, end), value);
	}

	/// <summary>Tries to parse the value using the given text <paramref name="parser"/>.</summary>
	/// <param name="context">The context that describes what the parsed value is for.</param>
	/// <param name="parser">The general text parser that should be used for parsing the value.</param>
	/// <param name="error">The error that occurred during parsing.</param>
	/// <returns>The parsed value.</returns>
	/// <remarks>You should override this if you want to parse the value the same way regardless of if it's a flag or an argument.</remarks>
	protected virtual T? TryParse(IValueParseContext context, ITextParser parser, out string? error)
	{
		Throw.New.NotImplementedException($"The ({GetType()}) value parser didn't implement any parsing logic.");

		error = default;
		return default;
	}

	/// <summary>Tries to parse the value using the given text <paramref name="parser"/>.</summary>
	/// <param name="context">The context that describes what flag the parsed value is for.</param>
	/// <param name="parser">The general text parser that should be used for parsing the value.</param>
	/// <param name="error">The error that occurred during parsing.</param>
	/// <returns>The parsed value.</returns>
	/// <remarks>If you override this, you should also override <see cref="TryParse(IArgumentValueParseContext, ITextParser, out string?)"/>.</remarks>
	protected virtual T? TryParse(IFlagValueParseContext context, ITextParser parser, out string? error)
	{
		return TryParse((IValueParseContext)context, parser, out error);
	}

	/// <summary>Tries to parse the value using the given text <paramref name="parser"/>.</summary>
	/// <param name="context">The context that describes what argument the parsed value is for.</param>
	/// <param name="parser">The general text parser that should be used for parsing the value.</param>
	/// <param name="error">The error that occurred during parsing.</param>
	/// <returns>The parsed value.</returns>
	/// <remarks>If you override this, you should also override <see cref="TryParse(IFlagValueParseContext, ITextParser, out string?)"/>.</remarks>
	protected virtual T? TryParse(IArgumentValueParseContext context, ITextParser parser, out string? error)
	{
		return TryParse((IValueParseContext)context, parser, out error);
	}
	#endregion

	#region Helpers
	/// <summary>Checks whether the next thing to parse is a missing value.</summary>
	/// <param name="parser">The parser to use for the check.</param>
	/// <returns><see langword="true"/> if the next thing to parse is a missing value, <see langword="false"/> otherwise.</returns>
	public bool IsValueMissing(ITextParser parser)
	{
		if (parser.IsLazy)
			return parser.IsAtEnd;

		Debug.Assert(parser.IsGreedy);

		return parser.IsAtEnd && parser.CurrentFragment.Length > 0;
	}

	/// <summary>Checks whether the next thing to parse is an empty value.</summary>
	/// <param name="parser">The parser to use for the check.</param>
	/// <returns><see langword="true"/> if the next thing to parse is an empty value, <see langword="false"/> otherwise.</returns>
	public bool IsEmptyValue(ITextParser parser)
	{
		if (parser.IsLazy)
			return false;

		Debug.Assert(parser.IsGreedy);

		return parser.CurrentFragment.Length is 0;
	}

	/// <summary>Checks whether the given parse <paramref name="context"/> allows <see langword="null"/> values.</summary>
	/// <param name="context">The parse context to check.</param>
	/// <returns>
	/// 	<see langword="true"/> if the given parse <paramref name="context"/>
	/// 	allows <see langword="null"/> values, <see langword="false"/> otherwise.
	/// </returns>
	public bool IsNullable(IValueParseContext context)
	{
		return context switch
		{
			IFlagValueParseContext flag => flag.Flag.IsNullable,
			IArgumentValueParseContext argument => argument.Argument.IsNullable,

			_ => Throw.New.ArgumentException<bool>(nameof(context), $"Unknown value parse context type ({context?.GetType()}).")
		};
	}
	#endregion
}
