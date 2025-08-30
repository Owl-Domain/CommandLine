using System.Linq.Expressions;

namespace OwlDomain.CommandLine.Parsing.Values.Primitives;

/// <summary>
/// 	Represents a value parser for a flag <see langword="enum"/> of the given type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the flag <see langword="enum"/> to parse.</typeparam>
/// <remarks>This parser will only handle flag enums, for regular enums use the <see cref="EnumValueParser{T}"/> instead.</remarks>
public sealed class FlagEnumValueParser<T> : IValueParser<T>
	where T : struct, Enum
{
	#region Nested types
	private delegate T MergeFlagsDelegate(T left, T right);
	#endregion

	#region Fields
	private static readonly ArrayCollectionValueParser<T> CollectionParser = GetCollectionParser();
	private static readonly MergeFlagsDelegate MergeFlags = CreateMergeDelegate();
	#endregion

	#region Methods
	/// <inheritdoc/>
	public IValueParseResult<T> Parse(IValueParseContext context, ITextParser parser)
	{
		ICollectionValueParseResult<T[], T> collectionResult = CollectionParser.Parse(context, parser);

		if (collectionResult.Successful)
		{
			Debug.Assert(collectionResult.Value is not null);

			T value = default;
			foreach (T flag in collectionResult.Value)
				value = MergeFlags.Invoke(value, flag);

			CollectionValueParseResult<T, T> result = new(
				context,
				collectionResult.Location,
				value,
				collectionResult.Prefix,
				collectionResult.Separators,
				collectionResult.Values,
				collectionResult.Suffix);

			return result;
		}

		Debug.Assert(collectionResult.Error is not null);

		return new CollectionValueParseResult<T, T>(
			context,
			collectionResult.Location,
			collectionResult.Error);
	}
	#endregion

	#region Helpers
	private static MergeFlagsDelegate CreateMergeDelegate()
	{
		Type type = typeof(T);
		Debug.Assert(type.IsEnum);

		Type baseType = type.GetEnumUnderlyingType();

		ParameterExpression left = Expression.Parameter(type, "left");
		ParameterExpression right = Expression.Parameter(type, "right");

		Expression convertLeft = Expression.Convert(left, baseType);
		Expression convertRight = Expression.Convert(right, baseType);

		Expression or = Expression.Or(convertLeft, convertRight);
		Expression convertBack = Expression.Convert(or, type);

		Expression<MergeFlagsDelegate> expression = Expression.Lambda<MergeFlagsDelegate>(convertBack, left, right);
		return expression.Compile();
	}
	private static ArrayCollectionValueParser<T> GetCollectionParser()
	{
		EnumValueParser<T> baseParser = new();
		ArrayCollectionValueParser<T> collectionParser = new(baseParser);

		return collectionParser;
	}
	#endregion
}
