using OwlDomain.CommandLine.Parsing.Values.Primitives;

namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents a builder for the <see cref="ICommandEngine"/>.
/// </summary>
public sealed class CommandEngineBuilder : ICommandEngineBuilder
{
	#region Fields
	private readonly HashSet<Type> _classes = [];
	private readonly List<IValueParserSelector> _selectors = [];
	private INameExtractor? _nameExtractor;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public ICommandEngineBuilder From(Type @class)
	{
		if (@class.IsClass is false)
			Throw.New.ArgumentException(nameof(@class), "The given type was not a class.");

		if (@class.GetConstructor([]) is not null)
			Throw.New.ArgumentException(nameof(@class), "The given class did not have a parameterless constructor.");

		_classes.Add(@class);

		return this;
	}

	/// <inheritdoc/>
	public ICommandEngineBuilder From<T>() where T : class, new()
	{
		_classes.Add(typeof(T));

		return this;
	}

	/// <inheritdoc/>
	public ICommandEngineBuilder WithSelector(IValueParserSelector selector)
	{
		if (_selectors.Contains(selector) is false)
			_selectors.Add(selector);

		return this;
	}

	/// <inheritdoc/>
	public ICommandEngineBuilder WithSelector<T>() where T : IValueParserSelector, new()
	{
		T selector = new();
		return WithSelector(selector);
	}

	/// <inheritdoc/>
	public ICommandEngine Build()
	{
		if (_classes.Count is 0) Throw.New.InvalidOperationException("No classes were provided to extract the commands from.");
		if (_classes.Count > 1) Throw.New.NotSupportedException("Extracting commands from multiple classes is not supported yet.");

		WithSelector<PrimitiveValueParserSelector>();
		_nameExtractor ??= NameExtractor.Instance;

		Dictionary<string, ICommandGroupInfo> childGroups = [];
		Dictionary<string, ICommandInfo> childCommands = [];

		Type classType = _classes.Single();
		IReadOnlyCollection<IFlagInfo> classFlags = GetFlags(classType);

		CommandGroupInfo group = new(null, null, classFlags, childGroups, childCommands, null);

		foreach (IMethodCommandInfo command in GetCommands(classType, group, classFlags))
		{
			Debug.Assert(command.Name is not null);
			childCommands.Add(command.Name, command);
		}

		return new CommandEngine(group);
	}
	#endregion

	#region Build helpers
	private IReadOnlyCollection<IMethodCommandInfo> GetCommands(Type classType, ICommandGroupInfo group, IReadOnlyCollection<IFlagInfo> classFlags)
	{
		Debug.Assert(_nameExtractor is not null);

		List<IMethodCommandInfo> commands = [];

		foreach (MethodInfo method in classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy))
		{
			if (method.IsSpecialName || method.DeclaringType == typeof(object))
				continue;

			if (method.ReturnType != typeof(void))
				Throw.New.NotSupportedException("Command return values are not supported yet.");

			string? name = _nameExtractor.GetCommandName(method);
			if (name is null)
				Throw.New.InvalidOperationException($"Couldn't extract a command name from the method ({method}).");

			IReadOnlyCollection<IFlagInfo> flags = method.IsStatic ? [] : classFlags;
			IReadOnlyList<IArgumentInfo> arguments = GetArguments(method);

			MethodCommandInfo command = new(method, name, group, flags, arguments);
			commands.Add(command);
		}

		return commands;
	}
	private IReadOnlyCollection<IFlagInfo> GetFlags(Type type)
	{
		Debug.Assert(_nameExtractor is not null);

		List<IFlagInfo> flags = [];

		object? instance = Activator.CreateInstance(type);
		Debug.Assert(instance is not null);

		PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
		foreach (PropertyInfo property in properties)
		{
#if NET7_0_OR_GREATER
			bool isRequired = property.GetCustomAttribute<RequiredMemberAttribute>() is not null;
#else
			bool isRequired = false;
#endif

			object? defaultValue = isRequired ? null : property.GetValue(instance);

			string? longName = _nameExtractor.GetLongFlagName(property);
			char? shortName = _nameExtractor.GetShortFlagName(property);
			IValueParser parser = SelectValueParser(property);

			IPropertyFlagInfo flag = CreatePropertyFlag(property, longName, shortName, isRequired, defaultValue, parser);

			flags.Add(flag);
		}

		return flags;
	}
	private IReadOnlyList<IArgumentInfo> GetArguments(MethodInfo method)
	{
		Debug.Assert(_nameExtractor is not null);

		List<IArgumentInfo> arguments = [];

		foreach (ParameterInfo parameter in method.GetParameters())
		{
			Debug.Assert(parameter.Name is not null);

			string name = _nameExtractor.GetArgumentName(parameter.Name);
			int position = parameter.Position;

			bool isRequired = parameter.HasDefaultValue is false;
			object? defaultValue = parameter.HasDefaultValue ? parameter.RawDefaultValue : null;
			IValueParser parser = SelectValueParser(parameter);

			IArgumentInfo argument = CreateParameterArgument(parameter, name, position, isRequired, defaultValue, parser);
			arguments.Add(argument);
		}

		return arguments;
	}
	#endregion

	#region Helpers
	private IValueParser SelectValueParser(PropertyInfo property)
	{
		foreach (IValueParserSelector selector in _selectors)
		{
			if (selector.TrySelect(property, out IValueParser? parser))
				return parser;
		}

		Throw.New.NotSupportedException($"Couldn't select a value parser for the property ({property}).");
		return default;
	}
	private IValueParser SelectValueParser(ParameterInfo parameter)
	{
		foreach (IValueParserSelector selector in _selectors)
		{
			if (selector.TrySelect(parameter, out IValueParser? parser))
				return parser;
		}

		Throw.New.NotSupportedException($"Couldn't select a value parser for the parameter ({parameter}).");
		return default;
	}
	#endregion

	#region Generic type helpers
	private static IPropertyFlagInfo CreatePropertyFlag(PropertyInfo property, string? longName, char? shortName, bool isRequired, object? defaultValue, IValueParser parser)
	{
		Type type = typeof(PropertyFlagInfo<>).MakeGenericType(property.PropertyType);

		object? untyped = Activator.CreateInstance(type, [property, longName, shortName, isRequired, defaultValue, parser]);
		Debug.Assert(untyped is not null);

		return (IPropertyFlagInfo)untyped;
	}
	private static IParameterFlagInfo CreateParameterFlag(ParameterInfo parameter, string? longName, char? shortName, bool isRequired, object? defaultValue, IValueParser parser)
	{
		Type type = typeof(ParameterFlagInfo<>).MakeGenericType(parameter.ParameterType);

		object? untyped = Activator.CreateInstance(type, [parameter, longName, shortName, isRequired, defaultValue, parser]);
		Debug.Assert(untyped is not null);

		return (IParameterFlagInfo)untyped;
	}
	private static IParameterArgumentInfo CreateParameterArgument(ParameterInfo parameter, string name, int position, bool isRequired, object? defaultValue, IValueParser parser)
	{
		Type type = typeof(ParameterArgumentInfo<>).MakeGenericType(parameter.ParameterType);

		object? untyped = Activator.CreateInstance(type, [parameter, name, position, isRequired, defaultValue, parser]);
		Debug.Assert(untyped is not null);

		return (IParameterArgumentInfo)untyped;
	}
	#endregion
}
