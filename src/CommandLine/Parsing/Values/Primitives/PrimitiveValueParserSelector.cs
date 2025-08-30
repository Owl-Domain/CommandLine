namespace OwlDomain.CommandLine.Parsing.Values.Primitives;

/// <summary>
/// 	Represents a selector for primitive value parsers.
/// </summary>
public sealed class PrimitiveValueParserSelector : BaseValueParserSelector
{
	#region Methods
	/// <inheritdoc/>
	protected override IValueParser? TrySelect(IRootValueParserSelector rootSelector, Type type)
	{
		if (type == typeof(string))
			return new StringValueParser();

		if (type == typeof(bool))
			return new BooleanValueParser();

		if (TryCreateEnumParser(type, out IValueParser? parser))
			return parser;

		if (TryCreateGenericParser(type, typeof(IFloatingPoint<>), typeof(DecimalValueParser<>), out parser))
			return parser;

		if (TryCreateGenericParser(type, typeof(IBinaryInteger<>), typeof(IntegerValueParser<>), out parser))
			return parser;

		if (TryCreateGenericParser(type, typeof(IParsable<>), typeof(ParsableValueParser<>), out parser))
			return parser;

		if (TryCreateNullableParser(rootSelector, type, out parser))
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

		if (TryCreateMemoryCollectionParser(rootSelector, type, out parser))
			return true;

		if (TryCreateGeneralCollectionValueParser(rootSelector, type, out parser))
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
	private static bool TryCreateMemoryCollectionParser(IRootValueParserSelector rootValueParser, Type type, [NotNullWhen(true)] out IValueParser? parser)
	{
		parser = default;

		if (type.IsConstructedGenericType is false)
			return false;

		Type typeDef = type.GetGenericTypeDefinition();
		Type parserType, elementType;

		if (typeDef == typeof(Memory<>))
		{
			elementType = type.GetGenericArguments()[0];
			parserType = typeof(MemoryCollectionValueParser.Mutable<>).MakeGenericType(elementType);
		}
		else if (typeDef == typeof(ReadOnlyMemory<>))
		{
			elementType = type.GetGenericArguments()[0];
			parserType = typeof(MemoryCollectionValueParser.ReadOnly<>).MakeGenericType(elementType);
		}
		else
			return false;

		if (rootValueParser.TrySelect(elementType, out IValueParser? valueParser) is false)
			return false;

		object? untyped = Activator.CreateInstance(parserType, [valueParser]);
		Debug.Assert(untyped is not null);

		parser = (IValueParser)untyped;
		return true;
	}
	private static bool TryCreateGeneralCollectionValueParser(IRootValueParserSelector rootSelector, Type type, [NotNullWhen(true)] out IValueParser? parser)
	{
		ReadOnlySpan<Type> checkTypes =
		[
			type,
			..type.GetInterfaces()
		];

		foreach (Type interfaceType in checkTypes)
		{
			if (interfaceType.IsConstructedGenericType is false)
				continue;

			if (interfaceType.GetGenericTypeDefinition() != typeof(IEnumerable<>))
				continue;

			Type valueType = interfaceType.GetGenericArguments()[0];

			if (TryCreateGeneralCollectionValueParser(rootSelector, type, valueType, out parser))
				return true;
		}

		parser = default;
		return false;
	}
	private static bool TryCreateGeneralCollectionValueParser(IRootValueParserSelector rootSelector, Type collectionType, Type valueType, [NotNullWhen(true)] out IValueParser? parser)
	{
		parser = default;

		if (rootSelector.TrySelect(valueType, out IValueParser? valueParser) is false)
			return false;

		Type concreteType = collectionType;
		if (collectionType.IsInterface)
		{
			ReadOnlySpan<Type> knownTypes =
			[
				typeof(List<>).MakeGenericType(valueType),
				typeof(HashSet<>).MakeGenericType(valueType)
			];

			Type? chosenType = null;
			foreach (Type knownType in knownTypes)
			{
				if (collectionType.IsAssignableFrom(knownType))
				{
					chosenType = knownType;
					break;
				}
			}

			if (chosenType is null)
				return false;

			concreteType = chosenType;
		}
		else if (collectionType.IsAbstract)
			return false;

		Type enumerableType = typeof(IEnumerable<>).MakeGenericType(valueType);

		ConstructorInfo? fastConstructor = concreteType.GetConstructor([enumerableType]);
		if (fastConstructor is not null)
		{
			Type parserType = typeof(GeneralCollectionValueParser<,>).MakeGenericType(concreteType, valueType);
			object? untyped = Activator.CreateInstance(parserType, [valueParser, fastConstructor]);
			Debug.Assert(untyped is not null);

			parser = (IValueParser)untyped;
			return true;
		}

		ConstructorInfo? emptyConstructor = concreteType.GetConstructor(BindingFlags.Public, Type.EmptyTypes);
		if (emptyConstructor is null)
			return false;

		MethodInfo? addRangeMethod = concreteType.GetMethod("AddRange", BindingFlags.Public | BindingFlags.Instance, [enumerableType]);
		MethodInfo? addMethod = null;

		if (addRangeMethod is null)
			addMethod = concreteType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, [valueType]);

		if (addRangeMethod is not null || addMethod is not null)
		{
			Type parserType = typeof(GeneralCollectionValueParser<,>).MakeGenericType(concreteType, valueType);
			object? untyped = Activator.CreateInstance(parserType, [valueParser, emptyConstructor, addRangeMethod, addMethod]);
			Debug.Assert(untyped is not null);

			parser = (IValueParser)untyped;
			return true;
		}

		return false;
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
	private static bool TryCreateNullableParser(IRootValueParserSelector rootSelector, Type type, [NotNullWhen(true)] out IValueParser? parser)
	{
		parser = default;

		Type? valueType = Nullable.GetUnderlyingType(type);
		if (valueType is null)
			return false;

		Debug.Assert(valueType.IsValueType);

		if (rootSelector.TrySelect(valueType, out IValueParser? valueParser) is false)
			return false;

		Type parserType = typeof(NullableValueParser<>).MakeGenericType(valueType);

		object? untyped = Activator.CreateInstance(parserType, [valueParser]);
		Debug.Assert(untyped is not null);

		parser = (IValueParser)untyped;
		return true;
	}
	private static bool TryCreateEnumParser(Type valueType, [NotNullWhen(true)] out IValueParser? parser)
	{
		parser = default;

		if (valueType.IsEnum is false)
			return false;

		Type parserType =
			valueType.GetCustomAttribute<FlagsAttribute>() is null ?
			typeof(EnumValueParser<>) :
			typeof(FlagEnumValueParser<>);

		parserType = parserType.MakeGenericType(valueType);

		object? untyped = Activator.CreateInstance(parserType);
		Debug.Assert(untyped is not null);

		parser = (IValueParser)untyped;
		return true;
	}
	#endregion
}
