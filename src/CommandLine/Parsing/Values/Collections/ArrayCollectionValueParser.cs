namespace OwlDomain.CommandLine.Parsing.Values.Collections;

/// <summary>
/// 	Represents the parser for an array value.
/// </summary>
/// <typeparam name="TValue">The type of the values in the array.</typeparam>
/// <param name="valueParser">The parser that was selected for the values in the array.</param>
public sealed class ArrayCollectionValueParser<TValue>(IValueParser<TValue> valueParser) : BaseCollectionValueParser<TValue[], TValue>(valueParser)
{
	#region Methods
	/// <inheritdoc/>
	protected override TValue[] CreateCollection(IReadOnlyList<TValue> values)
	{
		TValue[] array = new TValue[values.Count];

		for (int i = 0; i < values.Count; i++)
			array[i] = values[i];

		return array;
	}
	#endregion
}
