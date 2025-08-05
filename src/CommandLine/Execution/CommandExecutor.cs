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
		if (validatorResult.Diagnostics.Any())
			Throw.New.ArgumentException(nameof(validatorResult), $"Execution cannot be performed if there were validation errors.");

		if (validatorResult.ParserResult.LeafCommand is not ICommandParseResult command)
		{
			Throw.New.ArgumentException(nameof(validatorResult), $"The given validation result did not have a parsed command to execute.");
			return default; // Note(Nightowl): Never happens, needed for analysis to know the 'command' variable is always assigned later on;
		}

		DiagnosticBag diagnostics = [];

		Debug.Assert(command.Arguments.Count == command.CommandInfo.Arguments.Count);

		if (command.CommandInfo is IMethodCommandInfo methodCommand)
		{
			object? container = null;
			if (methodCommand.Method.IsStatic is false)
			{
				Type? containerType = methodCommand.Method.ReflectedType;
				Debug.Assert(containerType is not null);

				container = Activator.CreateInstance(containerType);
				Debug.Assert(container is not null);
			}

			object?[] parameters = new object?[methodCommand.Method.GetParameters().Length];

			foreach (IArgumentParseResult argument in command.Arguments)
				parameters[argument.ArgumentInfo.Position] = argument.Value.Value;

			_ = methodCommand.Method.Invoke(container, parameters);
		}
		else if (command.CommandInfo is IVirtualCommandInfo)
			Throw.New.NotImplementedException($"Executing virtual commands has not been implemented yet.");
		else
			Throw.New.InvalidOperationException($"Unknown command type ({command.CommandInfo?.GetType()}).");

		CommandExecutorResult result = new(validatorResult, diagnostics);

		return result;
	}
	#endregion
}
