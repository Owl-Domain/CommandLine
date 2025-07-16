namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents a builder for the <see cref="ICommandEngine"/>.
/// </summary>
public sealed class CommandEngineBuilder : ICommandEngineBuilder
{
	#region Fields
	private readonly HashSet<Type> _classes = [];
	#endregion

	#region Methods
	/// <inheritdoc/>
	public ICommandEngineBuilder From(Type @class)
	{
		if (@class.IsClass is false)
			Throw.New.ArgumentException(nameof(@class), "The given type was not a class.");

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
	public ICommandEngine Build()
	{
		if (_classes.Count is 0) Throw.New.InvalidOperationException("No classes were provided to extract the commands from.");
		if (_classes.Count > 1) Throw.New.NotSupportedException("Extracting commands from multiple classes is not supported yet.");

		Type classType = _classes.Single();

		Dictionary<string, ICommandGroupInfo> childGroups = [];
		Dictionary<string, ICommandInfo> childCommands = [];
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

	#region Helpers
	private static IReadOnlyCollection<IMethodCommandInfo> GetCommands(Type classType, ICommandGroupInfo group, IReadOnlyCollection<IFlagInfo> classFlags)
	{
		List<IMethodCommandInfo> commands = [];

		foreach (MethodInfo method in classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy))
		{
			if (method.IsSpecialName || method.DeclaringType == typeof(object))
				continue;

			if (method.ReturnType != typeof(void))
				Throw.New.NotSupportedException("Command return values are not supported yet.");

			string name = method.Name.ToLower();

			IReadOnlyCollection<IFlagInfo> flags = method.IsStatic ? [] : classFlags;
			IReadOnlyList<IArgumentInfo> arguments = GetArguments(method);

			MethodCommandInfo command = new(method, name, group, flags, arguments);
			commands.Add(command);
		}

		return commands;
	}
	private static IReadOnlyCollection<IFlagInfo> GetFlags(Type type)
	{
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

			string longName = property.Name.ToLower();
			char shortName = longName[0];

			IPropertyFlagInfo flag = CreatePropertyFlag(property, longName, shortName, isRequired, defaultValue);

			flags.Add(flag);
		}

		return flags;
	}
	private static IReadOnlyList<IArgumentInfo> GetArguments(MethodInfo method)
	{
		List<IArgumentInfo> arguments = [];

		foreach (ParameterInfo parameter in method.GetParameters())
		{
			Debug.Assert(parameter.Name is not null);

			string name = parameter.Name;
			int position = parameter.Position;

			bool isRequired = parameter.HasDefaultValue is false;
			object? defaultValue = parameter.HasDefaultValue ? parameter.RawDefaultValue : null;

			IArgumentInfo argument = CreateParameterArgument(parameter, name, position, isRequired, defaultValue);
			arguments.Add(argument);
		}

		return arguments;
	}
	#endregion

	#region Generic type helpers
	private static IPropertyFlagInfo CreatePropertyFlag(PropertyInfo property, string? longName, char? shortName, bool isRequired, object? defaultValue)
	{
		Type type = typeof(PropertyFlagInfo<>).MakeGenericType(property.PropertyType);

		object? untyped = Activator.CreateInstance(type, [property, longName, shortName, isRequired, defaultValue]);
		Debug.Assert(untyped is not null);

		return (IPropertyFlagInfo)untyped;
	}
	private static IParameterFlagInfo CreateParameterFlag(ParameterInfo parameter, string? longName, char? shortName, bool isRequired, object? defaultValue)
	{
		Type type = typeof(ParameterFlagInfo<>).MakeGenericType(parameter.ParameterType);

		object? untyped = Activator.CreateInstance(type, [parameter, longName, shortName, isRequired, defaultValue]);
		Debug.Assert(untyped is not null);

		return (IParameterFlagInfo)untyped;
	}
	private static IParameterArgumentInfo CreateParameterArgument(ParameterInfo parameter, string name, int position, bool isRequired, object? defaultValue)
	{
		Type type = typeof(ParameterArgumentInfo<>).MakeGenericType(parameter.ParameterType);

		object? untyped = Activator.CreateInstance(type, [parameter, name, position, isRequired, defaultValue]);
		Debug.Assert(untyped is not null);

		return (IParameterArgumentInfo)untyped;
	}
	#endregion
}
