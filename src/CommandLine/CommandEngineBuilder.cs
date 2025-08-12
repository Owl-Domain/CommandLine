using OwlDomain.CommandLine.Parsing.Values.Primitives;
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
	private readonly BuilderSettings _settings = new();
	private readonly HashSet<Type> _classes = [];
	private readonly List<IValueParserSelector> _selectors = [];
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

		WithSelector<PrimitiveValueParserSelector>();

		_nameExtractor ??= NameExtractor.Instance;
		_commandParser ??= new CommandParser();
		_valueParserSelector = new RootValueParserSelector(_selectors);
		_commandValidator ??= new CommandValidator();
		_commandExecutor ??= new CommandExecutor();
		_documentationProvider ??= new DocumentationProvider();
		_documentationPrinter ??= new DocumentationPrinter();

		SetupVirtual(settings, out IVirtualFlags virtualFlags, out IVirtualCommands virtualCommands);

		Dictionary<string, ICommandGroupInfo> childGroups = [];
		Dictionary<string, ICommandInfo> childCommands = [];

		Type classType = _classes.Single();
		IReadOnlyCollection<IFlagInfo> classFlags = GetFlags(classType);
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

		foreach (IMethodCommandInfo command in GetCommands(classType, group, classFlags))
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
	private IReadOnlyCollection<IMethodCommandInfo> GetCommands(Type classType, ICommandGroupInfo group, IReadOnlyCollection<IFlagInfo> classFlags)
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
			IReadOnlyList<IArgumentInfo> arguments = GetArguments(method);
			IDocumentationInfo? documentation = _documentationProvider.GetInfo(method);

			MethodCommandInfo command = new(method, name, group, commandFlags, arguments, documentation);

			foreach (VirtualFlag virtualFlag in _virtualFlags)
			{
				if (virtualFlag.CommandPredicate.Invoke(command))
					commandFlags.Add(virtualFlag.Flag);
			}

			commands.Add(command);
		}

		return commands;
	}
	private List<IFlagInfo> GetFlags(Type type)
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

		return new VirtualFlagInfo<bool>(
			FlagKind.Toggle,
			settings.LongHelpFlagName,
			settings.ShortHelpFlagName,
			false,
			false,
			parser,
			documentation,
			null);
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
