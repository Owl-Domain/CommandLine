namespace OwlDomain.CommandLine.Parsing.Values;

/// <summary>
/// 	Represents the result of a value parsing operation.
/// </summary>
/// <typeparam name="T">The type of the parsed value.</typeparam>
public sealed class ValueParseResult<T> : IValueParseResult<T>
{
	#region Properties
	/// <inheritdoc/>
	public IValueParseContext Context { get; }

	/// <inheritdoc/>
	public string? Error { get; }

	/// <inheritdoc/>
	public T? Value { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="ValueParseResult{T}"/>.</summary>
	/// <param name="context">The context for the <paramref name="value"/> that was parsed.</param>
	/// <param name="value">The value that was parsed.</param>
	public ValueParseResult(IValueParseContext context, T? value)
	{
		Context = context;
		Value = value;
	}

	/// <summary>Creates a new instance of the <see cref="ValueParseResult{T}"/>.</summary>
	/// <param name="context">The context</param>
	/// <param name="error"></param>
	public ValueParseResult(IValueParseContext context, string error)
	{
		Context = context;
		Error = error;
	}
	#endregion
}
