namespace OwlDomain.CommandLine.Parsing.Values.Primitives;

/// <summary>
/// 	Represents a selector for primitive value parsers.
/// </summary>
public sealed class PrimitiveValueParserSelector : BaseValueParserSelector
{
	#region Methods
	/// <inheritdoc/>
	protected override IValueParser? TrySelect(Type type)
	{
		if (type == typeof(string))
			return new StringValueParser();

		if (type == typeof(bool))
			return new BooleanValueParser();

#if NET7_0_OR_GREATER
		Type parsableType = typeof(IParsable<>).MakeGenericType(type);
		if (type.IsAssignableTo(parsableType))
		{
			Type parserType = typeof(ParsableValueParser<>).MakeGenericType(type);

			object? instance = Activator.CreateInstance(parserType);
			Debug.Assert(instance is not null);

			return (IValueParser)instance;
		}
#endif

		return null;
	}
	#endregion
}
