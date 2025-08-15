using OwlDomain.Documentation.Document.Nodes;

namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents a builder for the <see cref="ICommandEngine"/>.
/// </summary>
public sealed class CommandEngineBuilder : ICommandEngineBuilder
{
	#region Nested types
	private readonly record struct VirtualCommand(IVirtualCommandInfo Command, Predicate<ICommandGroupInfo> Predicate);
	private readonly record struct VirtualFlag(IVirtualFlagInfo Flag, Predicate<ICommandGroupInfo> GroupPredicate, Predicate<ICommandInfo> CommandPredicate);
	#endregion

	#region Fields
	private readonly NullabilityInfoContext _nullabilityContext = new();
	private readonly BuilderSettings _settings = new();
	private readonly HashSet<Type> _classes = [];
	private readonly List<IValueParserSelector> _parserSelectors = [];
	private readonly List<ICommandInjector> _injectors = [];
	private readonly List<IDefaultValueLabelProvider> _labelProviders = [];
	private IRootDefaultValueLabelProvider? _rootLabelProvider;
	private INameExtractor? _nameExtractor;
	private ICommandParser? _commandParser;
	private IRootValueParserSelector? _valueParserSelector;
	private ICommandValidator? _commandValidator;
	private ICommandExecutor? _commandExecutor;
	private IDocumentationProvider? _documentationProvider;
	private IDocumentationPrinter? _documentationPrinter;
	private readonly List<VirtualCommand> _virtualCommands = [];
	private readonly List<VirtualFlag> _virtualFlags = [];
	#endregion

	#region Methods
	/// <inheritdoc/>
	public ICommandEngineBuilder From(Type @class)
	{
		if (@class.IsClass is false)
			Throw.New.ArgumentException(nameof(@class), "The given type was not a class.");

		if (@class.GetConstructor(Type.EmptyTypes) is not null)
			Throw.New.ArgumentException(nameof(@class), "The given class did not have a parameterless constructor.");

		_classes.Add(@class);

		return this;
	}

	/// <inheritdoc/>
	public ICommandEngineBuilder From<T>() where T : class
	{
		_classes.Add(typeof(T));

		return this;
	}

	/// <inheritdoc/>
	public ICommandEngineBuilder With(IValueParserSelector selector)
	{
		if (_parserSelectors.Contains(selector) is false)
			_parserSelectors.Add(selector);

		return this;
	}

	/// <inheritdoc/>
	public ICommandEngineBuilder With(ICommandInjector injector)
	{
		if (_injectors.Contains(injector) is false)
			_injectors.Add(injector);

		return this;
	}

	/// <inheritdoc/>
	public ICommandEngineBuilder With(IDefaultValueLabelProvider provider)
	{
		if (_labelProviders.Contains(provider) is false)
			_labelProviders.Add(provider);

		return this;
	}

	/// <inheritdoc/>
	public ICommandEngineBuilder Customise(Action<BuilderSettings> callback)
	{
		callback.Invoke(_settings);
		return this;
	}

	/// <inheritdoc/>
	public ICommandEngineBuilder WithVirtualCommand(IVirtualCommandInfo command, Predicate<ICommandGroupInfo> predicate)
	{
		_virtualCommands.Add(new(command, predicate));
		return this;
	}

	/// <inheritdoc/>
	public ICommandEngineBuilder WirthVirtualFlag(IVirtualFlagInfo flag, Predicate<ICommandGroupInfo> groupPredicate, Predicate<ICommandInfo> commandPredicate)
	{
		_virtualFlags.Add(new(flag, groupPredicate, commandPredicate));
		return this;
	}

	/// <inheritdoc/>
	public ICommandEngine Build()
	{
		if (_classes.Count is 0) Throw.New.InvalidOperationException("No classes were provided to extract the commands from.");
		if (_classes.Count > 1) Throw.New.NotSupportedException("Extracting commands from multiple classes is not supported yet.");

		IEngineSettings settings = EngineSettings.From(_settings);

		EnsureDefaults();
		SetupVirtual(settings, out IVirtualFlags virtualFlags, out IVirtualCommands virtualCommands);

		Dictionary<string, ICommandGroupInfo> childGroups = [];
		Dictionary<string, ICommandInfo> childCommands = [];

		Type classType = _classes.Single();
		IReadOnlyCollection<IFlagInfo> classFlags = GetFlags(settings, classType, out IReadOnlyCollection<InjectedPropertyInfo> injectedClassProperties);
		IDocumentationInfo? documentation = _documentationProvider.GetInfo(classType);

		List<IFlagInfo> groupFlags = [.. classFlags];
		CommandGroupInfo group = new(null, null, groupFlags, childGroups, childCommands, null, documentation);

		foreach (VirtualCommand virtualCommand in _virtualCommands)
		{
			Debug.Assert(virtualCommand.Command.Name is not null);

			if (virtualCommand.Predicate.Invoke(group))
				childCommands.Add(virtualCommand.Command.Name, virtualCommand.Command);
		}

		foreach (VirtualFlag virtualFlag in _virtualFlags)
		{
			if (virtualFlag.GroupPredicate.Invoke(group))
				groupFlags.Add(virtualFlag.Flag);
		}

		foreach (IMethodCommandInfo command in GetCommands(settings, classType, group, classFlags, injectedClassProperties))
		{
			Debug.Assert(command.Name is not null);
			childCommands.Add(command.Name, command);
		}

		return new CommandEngine(
			settings,
			group,
			_commandParser,
			_valueParserSelector,
			_commandValidator,
			_commandExecutor,
			_documentationPrinter,
			virtualCommands,
			virtualFlags);
	}
	#endregion

	#region Build helpers
	[MemberNotNull(
		nameof(_commandParser), nameof(_valueParserSelector), nameof(_commandValidator),
		nameof(_commandExecutor), nameof(_documentationProvider), nameof(_documentationPrinter),
		nameof(_rootLabelProvider))]
	private void EnsureDefaults()
	{
		this.WithValueParserSelector<PathValueParserSelector>();
		this.WithValueParserSelector<NetworkingValueParserSelector>();
		this.WithValueParserSelector<PrimitiveValueParserSelector>();

		this.WithCommandInjector<EngineCommandInjector>();

		this.WithDefaultValueLabelProvider<PrimitiveDefaultValueLabelProvider>();
		this.WithDefaultValueLabelProvider<CollectionDefaultValueLabelProvider>();
		this.WithDefaultValueLabelProvider<ToStringDefaultLabelProvider>();

		_nameExtractor ??= NameExtractor.Instance;
		_commandParser ??= new CommandParser();
		_valueParserSelector = new RootValueParserSelector(_parserSelectors);
		_commandValidator ??= new CommandValidator();
		_commandExecutor ??= new CommandExecutor();
		_documentationProvider ??= new DocumentationProvider();
		_documentationPrinter ??= new DocumentationPrinter();
		_rootLabelProvider ??= new RootDefaultValueLabelProvider(_labelProviders);
	}
	private IReadOnlyCollection<IMethodCommandInfo> GetCommands(
		IEngineSettings settings,
		Type classType,
		ICommandGroupInfo group,
		IReadOnlyCollection<IFlagInfo> classFlags,
		IReadOnlyCollection<InjectedPropertyInfo> injectedClassProperties)
	{
		Debug.Assert(_nameExtractor is not null);
		Debug.Assert(_documentationProvider is not null);

		List<IMethodCommandInfo> commands = [];

		foreach (MethodInfo method in classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy))
		{
			if (method.IsSpecialName || method.DeclaringType == typeof(object))
				continue;

			string? name = _nameExtractor.GetCommandName(method);
			if (name is null)
				Throw.New.InvalidOperationException($"Couldn't extract a command name from the method ({method}).");

			List<IFlagInfo> commandFlags = method.IsStatic ? [] : [.. classFlags];
			IReadOnlyList<IArgumentInfo> arguments = GetArguments(settings, method, out IReadOnlyCollection<InjectedParameterInfo> injectedParameters);
			IDocumentationInfo? documentation = _documentationProvider.GetInfo(method);

			MethodCommandInfo command = new(method, name, group, commandFlags, arguments, documentation, injectedParameters, injectedClassProperties);

			foreach (VirtualFlag virtualFlag in _virtualFlags)
			{
				if (virtualFlag.CommandPredicate.Invoke(command))
					commandFlags.Add(virtualFlag.Flag);
			}

			commands.Add(command);
		}

		return commands;
	}
	private List<IFlagInfo> GetFlags(IEngineSettings settings, Type type, out IReadOnlyCollection<InjectedPropertyInfo> injectedProperties)
	{
		Debug.Assert(_nameExtractor is not null);
		Debug.Assert(_documentationProvider is not null);

		List<IFlagInfo> flags = [];
		List<InjectedPropertyInfo> injected = [];
		injectedProperties = injected;

		object? container = null;

		PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
		foreach (PropertyInfo property in properties)
		{
			if (TryGetInjected(property, out InjectedPropertyInfo injectedProperty))
			{
				injected.Add(injectedProperty);
				continue;
			}

			string? longName = _nameExtractor.GetLongFlagName(property);
			char? shortName = _nameExtractor.GetShortFlagName(property);
			FlagKind kind = GetFlagKind(property.PropertyType, property.Name, longName, shortName);

			bool isNullable =
				_nullabilityContext.Create(property).WriteState is NullabilityState.Nullable &&
				property.GetCustomAttribute<DisallowNullAttribute>() is null;

			bool isRequired = property.GetCustomAttribute<RequiredMemberAttribute>() is not null;
			IValueParser parser = SelectValueParser(property);
			IValueInfo valueInfo = CreateValueInfo(property.PropertyType, isRequired, isNullable, parser);

			IDefaultValueInfo? defaultValueInfo = null;
			if (isRequired is false)
			{
				string label = GetDefaultValueLabel(settings, property, type, ref container);
				defaultValueInfo = new DefaultValueInfo(label);
			}

			IDocumentationInfo? documentation = _documentationProvider.GetInfo(property);

			IPropertyFlagInfo flag = CreatePropertyFlag(property, kind, longName, shortName, valueInfo, defaultValueInfo, documentation);

			flags.Add(flag);
		}

		return flags;
	}
	private IReadOnlyList<IArgumentInfo> GetArguments(IEngineSettings settings, MethodInfo method, out IReadOnlyCollection<InjectedParameterInfo> injectedParameters)
	{
		Debug.Assert(_nameExtractor is not null);
		Debug.Assert(_documentationProvider is not null);

		List<IArgumentInfo> arguments = [];
		List<InjectedParameterInfo> injected = [];
		injectedParameters = injected;

		foreach (ParameterInfo parameter in method.GetParameters())
		{
			if (TryGetInjected(parameter, out InjectedParameterInfo injectedParameter))
			{
				injected.Add(injectedParameter);
				continue;
			}

			Debug.Assert(parameter.Name is not null);

			string name = _nameExtractor.GetArgumentName(parameter.Name);
			int position = parameter.Position;

			bool isNullable =
				_nullabilityContext.Create(parameter).WriteState is NullabilityState.Nullable &&
				parameter.GetCustomAttribute<DisallowNullAttribute>() is null;

			bool isRequired = parameter.HasDefaultValue is false;
			IValueParser parser = SelectValueParser(parameter);
			IValueInfo valueInfo = CreateValueInfo(parameter.ParameterType, isRequired, isNullable, parser);

			IDefaultValueInfo? defaultValueInfo = null;
			if (isRequired is false)
			{
				string label = GetDefaultValueLabel(settings, parameter);
				defaultValueInfo = new DefaultValueInfo(label);
			}

			IDocumentationInfo? documentation = _documentationProvider.GetInfo(parameter);

			IArgumentInfo argument = CreateParameterArgument(parameter, name, position, valueInfo, defaultValueInfo, documentation);
			arguments.Add(argument);
		}

		return arguments;
	}
	#endregion

	#region Virtual command helpers
	private void SetupVirtual(IEngineSettings settings, out IVirtualFlags flags, out IVirtualCommands commands)
	{
		Debug.Assert(_commandExecutor is not null);

		commands = SetupVirtualCommands(settings);
		flags = SetupVirtualFlags(settings);

		if (commands.Help is not null)
			WithVirtualCommand(commands.Help, group => true);

		if (flags.Help is not null)
			WirthVirtualFlag(flags.Help, group => true, cmd => true);

		if (commands.Help is not null || flags.Help is not null)
			_commandExecutor.OnExecute += HelpExecutionHandler;
	}
	private IVirtualFlags SetupVirtualFlags(IEngineSettings settings)
	{
		return new VirtualFlags()
		{
			Help = TryCreateHelpFlag(settings)
		};
	}
	private static IVirtualCommands SetupVirtualCommands(IEngineSettings settings)
	{
		return new VirtualCommands()
		{
			Help = TryCreateHelpCommand(settings),
		};
	}
	private IVirtualFlagInfo? TryCreateHelpFlag(IEngineSettings settings)
	{
		if (settings.IncludeHelpFlag is false)
			return null;

		TextDocumentationNode summary = new("Shows the help information about the available commands.");
		DocumentationInfo documentation = new(summary, null);

		IValueParser<bool> parser = SelectValueParser<bool>();
		string label = GetDefaultValueLabel(settings, false);

		return new VirtualFlagInfo<bool>(
			FlagKind.Toggle,
			settings.LongHelpFlagName,
			settings.ShortHelpFlagName,
			new ValueInfo<bool>(false, false, parser),
			new DefaultValueInfo(label),
			documentation);
	}
	private static IVirtualCommandInfo? TryCreateHelpCommand(IEngineSettings settings)
	{
		if (settings.IncludeHelpCommand is false)
			return null;

		TextDocumentationNode summary = new("Shows the help information about the available commands.");
		DocumentationInfo documentation = new(summary, null);

		return new VirtualCommandInfo(settings.HelpCommandName, null, [], [], documentation, false);
	}
	private static void HelpExecutionHandler(ICommandExecutionContext context)
	{
		if (context.CommandTarget == context.Engine.VirtualCommands.Help)
		{
			context.Engine.DocumentationPrinter.Print(context.Engine, context.GroupTarget);
			context.Handle(null);

			return;
		}

		IFlagInfo? helpFlag = context.Engine.VirtualFlags.Help;
		if (helpFlag is not null && context.Flags.ContainsKey(helpFlag))
		{
			if (context.CommandTarget is not null)
				context.Engine.DocumentationPrinter.Print(context.Engine, context.CommandTarget);
			else
				context.Engine.DocumentationPrinter.Print(context.Engine, context.GroupTarget);

			context.Handle(null);

			return;
		}
	}
	#endregion

	#region Value parser helpers
	private IValueParser<T> SelectValueParser<T>()
	{
		IValueParser parser = SelectValueParser(typeof(T));

		return (IValueParser<T>)parser;
	}
	private IValueParser SelectValueParser(Type type)
	{
		Debug.Assert(_valueParserSelector is not null);

		if (_valueParserSelector.TrySelect(type, out IValueParser? parser))
			return parser;

		Throw.New.NotSupportedException($"Couldn't select a value parser for the type ({type}).");
		return default;
	}
	private IValueParser SelectValueParser(PropertyInfo property)
	{
		Debug.Assert(_valueParserSelector is not null);

		if (_valueParserSelector.TrySelect(property, out IValueParser? parser))
			return parser;

		Throw.New.NotSupportedException($"Couldn't select a value parser for the property ({property}).");
		return default;
	}
	private IValueParser SelectValueParser(ParameterInfo parameter)
	{
		Debug.Assert(_valueParserSelector is not null);

		if (_valueParserSelector.TrySelect(parameter, out IValueParser? parser))
			return parser;

		Throw.New.NotSupportedException($"Couldn't select a value parser for the parameter ({parameter}).");
		return default;
	}
	#endregion

	#region Flag helpers
	private static FlagKind GetFlagKind(Type valueType, string originalName, string? longName, char? shortName)
	{
		if (valueType == typeof(bool) || valueType == typeof(bool?))
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
	#endregion

	#region Injection helpers
	private bool TryGetInjected(ParameterInfo parameter, out InjectedParameterInfo injected)
	{
		foreach (ICommandInjector injector in _injectors)
		{
			if (injector.CanInject(parameter))
			{
				injected = new(parameter, injector);
				return true;
			}
		}

		injected = default;
		return false;
	}
	private bool TryGetInjected(PropertyInfo property, out InjectedPropertyInfo injected)
	{
		foreach (ICommandInjector injector in _injectors)
		{
			if (injector.CanInject(property))
			{
				injected = new(property, injector);
				return true;
			}
		}

		injected = default;
		return false;
	}
	#endregion

	#region Label helpers
	private string GetDefaultValueLabel(IEngineSettings settings, PropertyInfo property, Type containerType, ref object? container)
	{
		Debug.Assert(_rootLabelProvider is not null);

		if (_rootLabelProvider.TryGet(settings, property, out string? label))
			return label;

		container ??= Activator.CreateInstance(containerType);
		Debug.Assert(container is not null);

		object? value = property.GetValue(container);

		if (_rootLabelProvider.TryGet(settings, property, value, out label))
			return label;

		if (_rootLabelProvider.TryGet(settings, value, out label))
			return label;

		Throw.New.NotSupportedException($"Couldn't select a label for the default value of the given property ({property}).");
		return default;
	}
	private string GetDefaultValueLabel(IEngineSettings settings, ParameterInfo parameter)
	{
		Debug.Assert(_rootLabelProvider is not null);

		if (_rootLabelProvider.TryGet(settings, parameter, out string? label))
			return label;

		if (parameter.HasDefaultValue)
		{
			object? value = parameter.DefaultValue;

			if (_rootLabelProvider.TryGet(settings, parameter, value, out label))
				return label;

			if (_rootLabelProvider.TryGet(settings, value, out label))
				return label;
		}

		Throw.New.NotSupportedException($"Couldn't select a label for the default value of the given parameter ({parameter}).");
		return default;
	}
	private string GetDefaultValueLabel(IEngineSettings settings, object? value)
	{
		Debug.Assert(_rootLabelProvider is not null);

		if (_rootLabelProvider.TryGet(settings, value, out string? label))
			return label;

		Throw.New.NotSupportedException($"Couldn't select a label for the given default value ({value}).");
		return default;
	}
	#endregion

	#region Generic type helpers
	private static IValueInfo CreateValueInfo(Type valueType, bool isRequired, bool isNullable, IValueParser parser)
	{
		Type type = typeof(ValueInfo<>).MakeGenericType(valueType);

		object? untyped = Activator.CreateInstance(type, [isRequired, isNullable, parser]);
		Debug.Assert(untyped is not null);

		return (IValueInfo)untyped;
	}
	private static IPropertyFlagInfo CreatePropertyFlag(
		PropertyInfo property,
		FlagKind kind,
		string? longName,
		char? shortName,
		IValueInfo valueInfo,
		IDefaultValueInfo? defaultValueInfo,
		IDocumentationInfo? documentation)
	{
		Type type = typeof(PropertyFlagInfo<>).MakeGenericType(property.PropertyType);

		object? untyped = Activator.CreateInstance(type, [property, kind, longName, shortName, valueInfo, defaultValueInfo, documentation]);
		Debug.Assert(untyped is not null);

		return (IPropertyFlagInfo)untyped;
	}
	private static IParameterFlagInfo CreateParameterFlag(
		ParameterInfo parameter,
		FlagKind kind,
		string? longName,
		char? shortName,
		IValueInfo valueInfo,
		IDefaultValueInfo? defaultValueInfo,
		IDocumentationInfo? documentation)
	{
		Type type = typeof(ParameterFlagInfo<>).MakeGenericType(parameter.ParameterType);

		object? untyped = Activator.CreateInstance(type, [parameter, kind, longName, shortName, valueInfo, defaultValueInfo, documentation]);
		Debug.Assert(untyped is not null);

		return (IParameterFlagInfo)untyped;
	}
	private static IParameterArgumentInfo CreateParameterArgument(
		ParameterInfo parameter,
		string name,
		int position,
		IValueInfo valueInfo,
		IDefaultValueInfo? defaultValueInfo,
		IDocumentationInfo? documentation)
	{
		Type type = typeof(ParameterArgumentInfo<>).MakeGenericType(parameter.ParameterType);

		object? untyped = Activator.CreateInstance(type, [parameter, name, position, valueInfo, defaultValueInfo, documentation]);
		Debug.Assert(untyped is not null);

		return (IParameterArgumentInfo)untyped;
	}
	#endregion
}
