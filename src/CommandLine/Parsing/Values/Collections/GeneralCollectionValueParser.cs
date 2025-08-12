namespace OwlDomain.CommandLine.Parsing.Values.Collections;

/// <summary>
/// 	Represents the general parser for collection types.
/// </summary>
/// <typeparam name="TCollection">The type of the collection that was parsed.</typeparam>
/// <typeparam name="TValue">The type of the values in the collection.</typeparam>
public sealed class GeneralCollectionValueParser<TCollection, TValue> : BaseCollectionValueParser<TCollection, TValue>
	where TCollection : IEnumerable<TValue>
{
	#region Fields
	private readonly ConstructorInfo? _fastConstructor;
	private readonly ConstructorInfo? _emptyConstructor;
	private readonly MethodInfo? _addRangeMethod;
	private readonly MethodInfo? _addMethod;
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="GeneralCollectionValueParser{TCollection, TValue}"/>.</summary>
	/// <param name="valueParser">The parser that was selected for the values in the collection.</param>
	/// <param name="fastConstructor">The constructor that can be used to initialise the collection with values.</param>
	/// <exception cref="ArgumentException">Thrown if the given <paramref name="fastConstructor"/> isn't a valid fast constructor.</exception>
	public GeneralCollectionValueParser(IValueParser<TValue> valueParser, ConstructorInfo fastConstructor)
		: base(valueParser)
	{
		_fastConstructor = fastConstructor;

		ParameterInfo[] parameters = fastConstructor.GetParameters();
		if (parameters.Length is not 1)
			Throw.New.ArgumentException(nameof(fastConstructor), "The given fast constructor was expected to only have one parameter.");

		ParameterInfo parameter = parameters[0];
		if (parameter.ParameterType != typeof(IEnumerable<TValue>))
			Throw.New.ArgumentException(nameof(fastConstructor), $"The given fast constructor was expected to take in a value of the ({typeof(IEnumerable<TValue>)}) type.");
	}

	/// <summary>Creates a new instance of the <see cref="GeneralCollectionValueParser{TCollection, TValue}"/>.</summary>
	/// <param name="valueParser">The parser that was selected for the values in the collection.</param>
	/// <param name="emptyConstructor">The empty constructor which can be used to create an instance of the <typeparamref name="TCollection"/>.</param>
	/// <param name="addRangeMethod">The method which can be used to add several values to the collection at once.</param>
	/// <param name="addMethod"></param>
	public GeneralCollectionValueParser(IValueParser<TValue> valueParser, ConstructorInfo emptyConstructor, MethodInfo? addRangeMethod, MethodInfo? addMethod)
		: base(valueParser)
	{
		if (emptyConstructor.GetParameters().Length is not 0)
			Throw.New.ArgumentException(nameof(emptyConstructor), $"The given constructor ({emptyConstructor}) was not a parameterless constructor.");

		if (addRangeMethod is null && addMethod is null)
			Throw.New.ArgumentException(nameof(addMethod), $"The expected collection type ({typeof(TCollection)}) must have an 'Add' method.");

		if (addRangeMethod is not null)
		{
			ParameterInfo[] parameters = addRangeMethod.GetParameters();
			if (parameters.Length is not 1)
				Throw.New.ArgumentException(nameof(addRangeMethod), "The given 'AddRange' method was expected to only have one parameter.");

			ParameterInfo parameter = parameters[0];
			if (parameter.ParameterType != typeof(IEnumerable<TValue>))
				Throw.New.ArgumentException(nameof(addRangeMethod), $"The given 'AddRange' method was expected to take in a value of the ({typeof(IEnumerable<TValue>)}) type.");
		}

		if (addMethod is not null)
		{
			ParameterInfo[] parameters = addMethod.GetParameters();
			if (parameters.Length is not 1)
				Throw.New.ArgumentException(nameof(addMethod), "The given 'Add' method was expected to only have one parameter.");

			ParameterInfo parameter = parameters[0];
			if (parameter.ParameterType != typeof(TValue))
				Throw.New.ArgumentException(nameof(addMethod), $"The given 'Add' method was expected to take in a value of the ({typeof(TValue)}) type.");
		}

		_emptyConstructor = emptyConstructor;
		_addRangeMethod = addRangeMethod;
		_addMethod = addMethod;
	}
	#endregion

	#region Methods
	/// <inheritdoc/>
	protected override TCollection CreateCollection(IReadOnlyList<TValue> values)
	{
		object untyped;
		if (_fastConstructor is not null)
		{
			untyped = _fastConstructor.Invoke([values]);
			return (TCollection)untyped;
		}

		Debug.Assert(_emptyConstructor is not null);
		untyped = _emptyConstructor.Invoke([]);

		if (_addRangeMethod is not null)
			_addRangeMethod.Invoke(untyped, [values]);
		else
		{
			Debug.Assert(_addMethod is not null);

			foreach (TValue value in values)
				_addMethod.Invoke(untyped, [value]);
		}

		return (TCollection)untyped;
	}
	#endregion
}
