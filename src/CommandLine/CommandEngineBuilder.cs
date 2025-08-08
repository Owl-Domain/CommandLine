using OwlDomain.CommandLine.Parsing.Values.Primitives;

namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents a builder for the <see cref="ICommandEngine"/>.
/// </summary>
public sealed class CommandEngineBuilder : ICommandEngineBuilder
{
	#region Fields
	private readonly BuilderSettings _settings = new();
	private readonly HashSet<Type> _classes = [];
	private readonly List<IValueParserSelector> _selectors = [];
	private INameExtractor? _nameExtractor;
	private ICommandParser? _commandParser;
	private ICommandValidator? _commandValidator;
	private ICommandExecutor? _commandExecutor;
	private IDocumentationProvider? _documentationProvider;
	private IDocumentationPrinter? _documentationPrinter;
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
	public ICommandEngineBuilder Customise(Action<BuilderSettings> callback)
	{
		callback.Invoke(_settings);
		return this;
	}

	/// <inheritdoc/>
	public ICommandEngine Build()
	{
		if (_classes.Count is 0) Throw.New.InvalidOperationException("No classes were provided to extract the commands from.");
		if (_classes.Count > 1) Throw.New.NotSupportedException("Extracting commands from multiple classes is not supported yet.");

		IEngineSettings settings = CreateSettings(_settings);

		WithSelector<PrimitiveValueParserSelector>();

		_nameExtractor ??= NameExtractor.Instance;
		_commandParser ??= new CommandParser();
		_commandValidator ??= new CommandValidator();
		_commandExecutor ??= new CommandExecutor();
		_documentationProvider ??= new DocumentationProvider();
		_documentationPrinter ??= new DocumentationPrinter();

		Dictionary<string, ICommandGroupInfo> childGroups = [];
		Dictionary<string, ICommandInfo> childCommands = [];

		Type classType = _classes.Single();
		IReadOnlyCollection<IFlagInfo> classFlags = GetFlags(classType);
		IDocumentationInfo? documentation = _documentationProvider.GetInfo(classType);

		CommandGroupInfo group = new(null, null, classFlags, childGroups, childCommands, null, documentation);

		foreach (IMethodCommandInfo command in GetCommands(classType, group, classFlags))
		{
			Debug.Assert(command.Name is not null);
			childCommands.Add(command.Name, command);
		}

		return new CommandEngine(settings, group, _commandParser, _commandValidator, _commandExecutor, _documentationPrinter);
	}
	#endregion

	#region Build helpers
	private IReadOnlyCollection<IMethodCommandInfo> GetCommands(Type classType, ICommandGroupInfo group, IReadOnlyCollection<IFlagInfo> classFlags)
	{
		Debug.Assert(_nameExtractor is not null);
		Debug.Assert(_documentationProvider is not null);

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
			IDocumentationInfo? documentation = _documentationProvider.GetInfo(method);

			MethodCommandInfo command = new(method, name, group, flags, arguments, documentation);
			commands.Add(command);
		}

		return commands;
	}
	private IReadOnlyCollection<IFlagInfo> GetFlags(Type type)
	{
		Debug.Assert(_nameExtractor is not null);
		Debug.Assert(_documentationProvider is not null);

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
			FlagKind kind = GetFlagKind(property.PropertyType, property.Name, longName, shortName);
			IDocumentationInfo? documentation = _documentationProvider.GetInfo(property);

			IPropertyFlagInfo flag = CreatePropertyFlag(property, kind, longName, shortName, isRequired, defaultValue, parser, documentation, null);

			flags.Add(flag);
		}

		return flags;
	}
	private IReadOnlyList<IArgumentInfo> GetArguments(MethodInfo method)
	{
		Debug.Assert(_nameExtractor is not null);
		Debug.Assert(_documentationProvider is not null);

		List<IArgumentInfo> arguments = [];

		foreach (ParameterInfo parameter in method.GetParameters())
		{
			Debug.Assert(parameter.Name is not null);

			string name = _nameExtractor.GetArgumentName(parameter.Name);
			int position = parameter.Position;

			bool isRequired = parameter.HasDefaultValue is false;
			object? defaultValue = parameter.HasDefaultValue ? parameter.RawDefaultValue : null;
			IValueParser parser = SelectValueParser(parameter);
			IDocumentationInfo? documentation = _documentationProvider.GetInfo(parameter);

			IArgumentInfo argument = CreateParameterArgument(parameter, name, position, isRequired, defaultValue, parser, documentation, null);
			arguments.Add(argument);
		}

		return arguments;
	}
	#endregion

	#region Helpers
	private static FlagKind GetFlagKind(Type valueType, string originalName, string? longName, char? shortName)
	{
		if (valueType == typeof(bool))
			return FlagKind.Toggle;

		if (IsVerbosityFlag(originalName, longName, shortName) && IsNumericType(valueType))
			return FlagKind.Repeat;

		return FlagKind.Regular;
	}
	private static bool IsVerbosityFlag(string originalName, string? longName, char? shortName)
	{
		if (longName is not null)
		{
			if (longName.Equals("verbose", StringComparison.OrdinalIgnoreCase))
				return true;

			if (longName.Equals("verbosity", StringComparison.OrdinalIgnoreCase))
				return true;
		}

		if (shortName is 'v' or 'V')
		{
			if (originalName.Equals("verbose", StringComparison.OrdinalIgnoreCase))
				return true;

			if (originalName.Equals("verbosity", StringComparison.OrdinalIgnoreCase))
				return true;
		}

		return false;
	}
	private static bool IsNumericType(Type type)
	{
		if (type == typeof(byte)) return true;
		if (type == typeof(sbyte)) return true;

		if (type == typeof(ushort)) return true;
		if (type == typeof(short)) return true;

		if (type == typeof(uint)) return true;
		if (type == typeof(int)) return true;

		if (type == typeof(ulong)) return true;
		if (type == typeof(long)) return true;

		return false;
	}
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
	private static void ValidateSettings(IEngineSettings settings)
	{
		if (settings.LongFlagPrefix == settings.ShortFlagPrefix && (settings.MergeLongAndShortFlags is false))
			Throw.New.InvalidOperationException($"The {nameof(settings.LongFlagPrefix)} and {nameof(settings.ShortFlagPrefix)} settings had the same value, but the {settings.MergeLongAndShortFlags} setting was false.");

		if (settings.IncludeHelpCommand && string.IsNullOrWhiteSpace(settings.HelpCommandName))
			Throw.New.InvalidOperationException($"{nameof(settings.IncludeHelpCommand)} setting was set to true, but the {nameof(settings.HelpCommandName)} setting ({settings.HelpCommandName}) was invalid.");

		if (settings.IncludeHelpFlag && string.IsNullOrWhiteSpace(settings.LongHelpFlagName) && settings.ShortHelpFlagName is null)
			Throw.New.InvalidOperationException($"The {nameof(settings.IncludeHelpFlag)} settings was set set to true, but both the {nameof(settings.LongHelpFlagName)} ({settings.LongHelpFlagName}) and {nameof(settings.ShortHelpFlagName)} ({settings.ShortHelpFlagName}) settings had invalid values.");
	}
	private IEngineSettings CreateSettings(IEngineSettings settings)
	{
		ValidateSettings(settings);

		return new EngineSettings()
		{
			AllowFlagShadowing = settings.AllowFlagShadowing,
			LongFlagPrefix = settings.LongFlagPrefix,
			ShortFlagPrefix = settings.ShortFlagPrefix,
			MergeLongAndShortFlags = settings.MergeLongAndShortFlags,
			IncludeHelpFlag = settings.IncludeHelpFlag,
			LongHelpFlagName = settings.LongHelpFlagName,
			ShortHelpFlagName = settings.ShortHelpFlagName,
			IncludeHelpCommand = settings.IncludeHelpCommand,
			HelpCommandName = settings.HelpCommandName,
		};
	}
	#endregion

	#region Generic type helpers
	private static IPropertyFlagInfo CreatePropertyFlag(
		PropertyInfo property,
		FlagKind kind,
		string? longName,
		char? shortName,
		bool isRequired,
		object? defaultValue,
		IValueParser parser,
		IDocumentationInfo? documentation,
		string? defaultValueLabel)
	{
		Type type = typeof(PropertyFlagInfo<>).MakeGenericType(property.PropertyType);

		object? untyped = Activator.CreateInstance(type, [property, kind, longName, shortName, isRequired, defaultValue, parser, documentation, defaultValueLabel]);
		Debug.Assert(untyped is not null);

		return (IPropertyFlagInfo)untyped;
	}
	private static IParameterFlagInfo CreateParameterFlag(
		ParameterInfo parameter,
		FlagKind kind,
		string? longName,
		char? shortName,
		bool isRequired,
		object? defaultValue,
		IValueParser parser,
		IDocumentationInfo? documentation,
		string? defaultValueLabel)
	{
		Type type = typeof(ParameterFlagInfo<>).MakeGenericType(parameter.ParameterType);

		object? untyped = Activator.CreateInstance(type, [parameter, kind, longName, shortName, isRequired, defaultValue, parser, documentation, defaultValueLabel]);
		Debug.Assert(untyped is not null);

		return (IParameterFlagInfo)untyped;
	}
	private static IParameterArgumentInfo CreateParameterArgument(
		ParameterInfo parameter,
		string name,
		int position,
		bool isRequired,
		object? defaultValue,
		IValueParser parser,
		IDocumentationInfo? documentation,
		string? defaultValueLabel)
	{
		Type type = typeof(ParameterArgumentInfo<>).MakeGenericType(parameter.ParameterType);

		object? untyped = Activator.CreateInstance(type, [parameter, name, position, isRequired, defaultValue, parser, documentation, defaultValueLabel]);
		Debug.Assert(untyped is not null);

		return (IParameterArgumentInfo)untyped;
	}
	#endregion
}
