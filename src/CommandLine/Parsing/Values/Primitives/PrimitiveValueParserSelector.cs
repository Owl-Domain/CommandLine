using OwlDomain.CommandLine.Parsing.Values.Collections;

namespace OwlDomain.CommandLine.Parsing.Values.Primitives;

/// <summary>
/// 	Represents a selector for primitive value parsers.
/// </summary>
public sealed class PrimitiveValueParserSelector : BaseValueParserSelector
{
	#region Fields
	private readonly Dictionary<Type, WeakReference<IValueParser>> _cache = [];
	#endregion

	#region Methods
	/// <inheritdoc/>
	protected override IValueParser? TrySelect(IRootValueParserSelector rootSelector, Type type)
	{
		IValueParser? parser;

		if (_cache.TryGetValue(type, out WeakReference<IValueParser>? weakRef))
		{
			if (weakRef.TryGetTarget(out parser))
				return parser;

			parser = CreateParser(rootSelector, type);

			if (parser is not null)
				weakRef.SetTarget(parser);

			return parser;
		}

		parser = CreateParser(rootSelector, type);
		if (parser is not null)
			_cache.Add(type, new(parser));

		return parser;
	}
	private static IValueParser? CreateParser(IRootValueParserSelector rootSelector, Type type)
	{
		if (type == typeof(string))
			return new StringValueParser();

		if (type == typeof(bool))
			return new BooleanValueParser();

		if (TryCreateGenericParser(type, typeof(IFloatingPoint<>), typeof(DecimalValueParser<>), out IValueParser? parser))
			return parser;

		if (TryCreateGenericParser(type, typeof(IBinaryInteger<>), typeof(IntegerValueParser<>), out parser))
			return parser;

		if (TryCreateGenericParser(type, typeof(IParsable<>), typeof(ParsableValueParser<>), out parser))
			return parser;

		if (TryCreateCollectionParser(rootSelector, type, out parser))
			return parser;

		return null;
	}
	#endregion

	#region Collection methods
	private static bool TryCreateCollectionParser(IRootValueParserSelector rootSelector, Type type, [NotNullWhen(true)] out IValueParser? parser)
	{
		if (TryCreateArrayCollectionParser(rootSelector, type, out parser))
			return true;

		return false;
	}
	private static bool TryCreateArrayCollectionParser(IRootValueParserSelector rootSelector, Type type, [NotNullWhen(true)] out IValueParser? parser)
	{
		parser = default;

		if (type.IsSZArray is false)
			return false;

		Type? elementType = type.GetElementType();
		Debug.Assert(elementType is not null);

		if (rootSelector.TrySelect(elementType, out IValueParser? valueParser) is false)
			return false;

		Type parserType = typeof(ArrayCollectionValueParser<>).MakeGenericType(elementType);

		object? untypedParser = Activator.CreateInstance(parserType, [valueParser]);
		Debug.Assert(untypedParser is not null);

		parser = (IValueParser)untypedParser;
		return true;
	}
	#endregion

	#region Helpers
	private static bool TryCreateGenericParser(Type valueType, Type baseType, Type concreteType, [NotNullWhen(true)] out IValueParser? parser)
	{
		if (valueType.GetInterfaces().Any(type => type.IsGenericType && type.GetGenericTypeDefinition() == baseType) is false)
		{
			parser = default;
			return false;
		}

		Type filledBaseType = baseType.MakeGenericType(valueType);

		if (valueType.IsAssignableTo(filledBaseType))
		{
			Type parserType = concreteType.MakeGenericType(valueType);

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
