namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents an executor for validated commands.
/// </summary>
public sealed class CommandExecutor : ICommandExecutor
{
	#region Methods
	/// <inheritdoc/>
	public ICommandExecutorResult Execute(ICommandValidatorResult validatorResult)
	{
		if (validatorResult.Successful is false)
			return new CommandExecutorResult(false, validatorResult, new DiagnosticBag(), default);

		if (validatorResult.ParserResult.LeafCommand is not ICommandParseResult command)
		{
			Throw.New.ArgumentException(nameof(validatorResult), $"The given validation result did not have a parsed command to execute.");
			return default; // Note(Nightowl): Never happens, needed for analysis to know the 'command' variable is always assigned later on;
		}

		Stopwatch watch = Stopwatch.StartNew();
		DiagnosticBag diagnostics = [];

		Debug.Assert(command.Arguments.Count == command.CommandInfo.Arguments.Count);

		if (command.CommandInfo is IMethodCommandInfo methodCommand)
		{
			object? container = SetupContainer(validatorResult.ParserResult, methodCommand);
			object?[] parameters = new object?[methodCommand.Method.GetParameters().Length];

			foreach (IArgumentParseResult argument in command.Arguments)
				parameters[argument.ArgumentInfo.Position] = argument.Value.Value;

			_ = methodCommand.Method.Invoke(container, parameters);
		}
		else if (command.CommandInfo is IVirtualCommandInfo)
			Throw.New.NotImplementedException($"Executing virtual commands has not been implemented yet.");
		else
			Throw.New.InvalidOperationException($"Unknown command type ({command.CommandInfo?.GetType()}).");

		watch.Stop();
		CommandExecutorResult result = new(diagnostics.Any() is false, validatorResult, diagnostics, watch.Elapsed);

		return result;
	}
	#endregion

	#region Helpers
	private static object? SetupContainer(ICommandParserResult parserResult, IMethodCommandInfo command)
	{
		if (command.Method.IsStatic is true)
			return null;

		Type? containerType = command.Method.ReflectedType;
		Debug.Assert(containerType is not null);

		object? container = Activator.CreateInstance(containerType);
		Debug.Assert(container is not null);

		foreach (IFlagParseResult flag in parserResult.Flags)
		{
			if (flag is IValueFlagParseResult valueFlag)
			{
				if (valueFlag.FlagInfo is IPropertyFlagInfo propertyFlag)
				{
					Type? declaringType = propertyFlag.Property.DeclaringType;
					Debug.Assert(declaringType is not null);

					if (containerType == declaringType || declaringType.IsAssignableFrom(containerType))
						propertyFlag.Property.SetValue(container, valueFlag.Value.Value);
				}
			}
		}

		return container;
	}
	#endregion
}
