namespace OwlDomain.CommandLine.Parsing.Values.Collections;

/// <summary>
/// 	Represents a parent type for <see cref="Memory{T}"/> and <see cref="ReadOnlyMemory{T}"/> parsers.
/// </summary>
public static class MemoryCollectionValueParser
{
	#region Nested types
	/// <summary>
	/// 	Represents the parser for <see cref="Memory{T}"/> values.
	/// </summary>
	/// <typeparam name="T">The type of the values in the collection.</typeparam>
	/// <param name="valueParser">The parser that was selected for the values in the array.</param>
	public sealed class Mutable<T>(IValueParser<T> valueParser) : BaseCollectionValueParser<Memory<T>, T>(valueParser)
	{
		#region Methods
		/// <inheritdoc/>
		protected override Memory<T> CreateCollection(IReadOnlyList<T> values)
		{
			// Note(Nightowl): No real way to optimise this easily;
			T[] array = [.. values];

			return new(array);
		}
		#endregion
	}

	/// <summary>
	/// 	Represents the parser for <see cref="ReadOnlyMemory{T}"/> values.
	/// </summary>
	/// <typeparam name="T">The type of the values in the collection.</typeparam>
	/// <param name="valueParser">The parser that was selected for the values in the array.</param>
	public sealed class ReadOnly<T>(IValueParser<T> valueParser) : BaseCollectionValueParser<ReadOnlyMemory<T>, T>(valueParser)
	{
		#region Methods
		/// <inheritdoc/>
		protected override ReadOnlyMemory<T> CreateCollection(IReadOnlyList<T> values)
		{
			// Note(Nightowl): No real way to optimise this easily;
			T[] array = [.. values];

			return new(array);
		}
		#endregion
	}
	#endregion
}
