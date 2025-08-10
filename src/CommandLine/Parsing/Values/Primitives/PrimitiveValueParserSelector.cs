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

		if (TryGetIntegerParser(type, out IValueParser? integerParser))
			return integerParser;

		if (TryGetGenericParser(type, out IValueParser? genericParser))
			return genericParser;

		return null;
	}
	#endregion

	#region Helpers
	private static bool TryGetIntegerParser(Type type, [NotNullWhen(true)] out IValueParser? parser)
	{
		Type integerType = typeof(IBinaryInteger<>).MakeGenericType(type);
		if (type.IsAssignableTo(integerType))
		{
			Type parserType = typeof(IntegerValueParser<>).MakeGenericType(type);

			object? instance = Activator.CreateInstance(parserType);
			Debug.Assert(instance is not null);

			parser = (IValueParser)instance;
			return true;
		}

		parser = default;
		return false;
	}
	private static bool TryGetGenericParser(Type type, [NotNullWhen(true)] out IValueParser? parser)
	{
		Type parsableType = typeof(IParsable<>).MakeGenericType(type);
		if (type.IsAssignableTo(parsableType))
		{
			Type parserType = typeof(ParsableValueParser<>).MakeGenericType(type);

			object? instance = Activator.CreateInstance(parserType);
			Debug.Assert(instance is not null);

			parser = (IValueParser)instance;
			return true;
		}

		parser = default;
		return false;
	}
	#endregion
}
