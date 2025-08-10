namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents an executor for validated commands.
/// </summary>
public sealed class CommandExecutor : ICommandExecutor
{
	#region Events
	/// <inheritdoc/>
	public event CommandExecutionDelegate? OnExecute;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public ICommandExecutorResult Execute(ICommandValidatorResult validatorResult, CommandExecutionDelegate? callback = null)
	{
		if (validatorResult.Successful is false)
			return new CommandExecutorResult(false, validatorResult, new DiagnosticBag(), default, default);

		Stopwatch watch = Stopwatch.StartNew();
		IReadOnlyDictionary<IFlagInfo, object?> flags = GetFlags(validatorResult.ParserResult);
		IReadOnlyDictionary<IArgumentInfo, object?> arguments = GetArguments(validatorResult.ParserResult);

		ICommandGroupInfo? groupTarget = GetTargetGroup(validatorResult.ParserResult.CommandOrGroup) ?? validatorResult.Engine.RootGroup;
		ICommandInfo? commandTarget = GetTargetCommand(validatorResult.ParserResult.CommandOrGroup);

		DiagnosticBag diagnostics = [];

		if (callback is not null || OnExecute is null)
		{
			CommandExecutionContext context = new(diagnostics, validatorResult.Engine, groupTarget, commandTarget, arguments, flags);

			callback?.Invoke(context);
			if (OnExecute is not null)
			{
				foreach (Delegate del in OnExecute.GetInvocationList())
				{
					if (context.Handled)
						break;

					del.DynamicInvoke(context);
				}
			}

			if (context.Handled)
			{
				watch.Stop();
				return new CommandExecutorResult(diagnostics.Any() is false, validatorResult, diagnostics, watch.Elapsed, context.ResultValue);
			}
		}

		if (validatorResult.ParserResult.LeafCommand is not ICommandParseResult command)
		{
			Throw.New.ArgumentException(nameof(validatorResult), $"The given validation result did not have a parsed command to execute.");
			return default; // Note(Nightowl): Never happens, needed for analysis to know the 'command' variable is always assigned later on;
		}

		object? commandResult = default;

		Debug.Assert(command.Arguments.Count == command.CommandInfo.Arguments.Count);
		if (command.CommandInfo is IMethodCommandInfo methodCommand)
		{
			object? container = SetupContainer(methodCommand.Method, flags);
			object?[] parameters = SetupArguments(methodCommand.Method, arguments);

			commandResult = methodCommand.Method.Invoke(container, parameters);
		}
		else if (command.CommandInfo is IVirtualCommandInfo)
			Throw.New.NotImplementedException($"Executing virtual commands has not been implemented yet.");
		else
			Throw.New.InvalidOperationException($"Unknown command type ({command.CommandInfo?.GetType()}).");

		watch.Stop();
		return new CommandExecutorResult(diagnostics.Any() is false, validatorResult, diagnostics, watch.Elapsed, commandResult);
	}
	#endregion

	#region Helpers
	private static ICommandInfo? GetTargetCommand(IParseResult? result)
	{
		while (result is not null)
		{
			if (result is ICommandParseResult command)
				return command.CommandInfo;

			if (result is IGroupParseResult group)
				result = group.CommandOrGroup;
		}

		return null;
	}
	private static ICommandGroupInfo? GetTargetGroup(IParseResult? result)
	{
		while (result is not null)
		{
			if (result is ICommandParseResult command)
				return command.CommandInfo.Group;

			if (result is IGroupParseResult group)
			{
				if (group.CommandOrGroup is null or ICommandParseResult)
					return group.GroupInfo;

				result = group.CommandOrGroup;
			}
		}

		return null;
	}
	private static IReadOnlyDictionary<IFlagInfo, object?> GetFlags(ICommandParserResult result)
	{
		Dictionary<IFlagInfo, object?> flags = [];

		foreach (IFlagParseResult flag in result.Flags)
		{
			if (flag is IValueFlagParseResult valueFlag)
				flags.Add(valueFlag.FlagInfo, valueFlag.Value.Value);
			else if (flag is IToggleFlagParseResult toggleFlag)
				flags.Add(toggleFlag.FlagInfo, true);
			else if (flag is IChainFlagParseResult chainFlag)
			{
				foreach (IFlagInfo chainedFlag in chainFlag.FlagInfos)
					flags.Add(chainedFlag, true);
			}
			else if (flag is IRepeatFlagParseResult repeatFlag)
			{
				object? value = Convert.ChangeType(repeatFlag.Repetition, repeatFlag.FlagInfo.ValueType);
				flags.Add(repeatFlag.FlagInfo, value);
			}
			else
				Throw.New.NotSupportedException($"Unknown flag parse result type ({flag.GetType()}).");
		}

		return flags;
	}
	private static IReadOnlyDictionary<IArgumentInfo, object?> GetArguments(ICommandParserResult result)
	{
		Dictionary<IArgumentInfo, object?> arguments = [];

		foreach (IArgumentParseResult argument in result.Arguments)
			arguments.Add(argument.ArgumentInfo, argument.Value.Value);

		return arguments;
	}
	private static object? SetupContainer(MethodInfo method, IReadOnlyDictionary<IFlagInfo, object?> flags)
	{
		if (method.IsStatic is true)
			return null;

		Type? containerType = method.ReflectedType;
		Debug.Assert(containerType is not null);

		object? container = Activator.CreateInstance(containerType);
		Debug.Assert(container is not null);

		foreach (KeyValuePair<IFlagInfo, object?> pair in flags)
		{
			if (pair.Key is IPropertyFlagInfo propertyFlag)
			{
				Type? declaringType = propertyFlag.Property.DeclaringType;
				Debug.Assert(declaringType is not null);

				if (containerType != declaringType && (declaringType.IsAssignableFrom(containerType) is false))
					Throw.New.InvalidOperationException($"Couldn't set the property ({propertyFlag.Property}) on the given container type ({containerType}).");

				propertyFlag.Property.SetValue(container, pair.Value);
			}
		}

		return container;
	}
	private static object?[] SetupArguments(MethodInfo method, IReadOnlyDictionary<IArgumentInfo, object?> arguments)
	{
		object?[] args = new object?[method.GetParameters().Length];

		foreach (KeyValuePair<IArgumentInfo, object?> pair in arguments)
		{
			if (pair.Key is IParameterArgumentInfo parameterArgument)
				args[parameterArgument.Parameter.Position] = pair.Value;
		}

		return args;
	}
	#endregion
}
