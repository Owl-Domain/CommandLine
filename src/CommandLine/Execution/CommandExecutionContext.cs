namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents the context for the execution of a command.
/// </summary>
public sealed class CommandExecutionContext : ICommandExecutionContext
{
	#region Properties
	/// <inheritdoc/>
	public bool Handled { get; private set; }

	/// <inheritdoc/>
	public object? ResultValue { get; private set; }

	/// <inheritdoc/>
	public DiagnosticBag Diagnostics { get; }

	/// <inheritdoc/>
	public ICommandEngine Engine { get; }

	/// <inheritdoc/>
	public ICommandGroupInfo? GroupTarget { get; }

	/// <inheritdoc/>
	public ICommandInfo? CommandTarget { get; }

	/// <inheritdoc/>
	public IReadOnlyDictionary<IArgumentInfo, object?> Arguments { get; }

	/// <inheritdoc/>
	public IReadOnlyDictionary<IFlagInfo, object?> Flags { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="CommandExecutionContext"/>.</summary>
	/// <param name="diagnostics">A collection of the diagnostics reported during the execution.</param>
	/// <param name="engine">The engine that is handling the execution.</param>
	/// <param name="groupTarget">The targetted group, if this is <see langword="null"/> then the <paramref name="commandTarget"/> must not be <see langword="null"/>.</param>
	/// <param name="commandTarget">The targetted command, if this is <see langword="null"/> then the <paramref name="groupTarget"/> must not be <see langword="null"/>.</param>
	/// <param name="arguments">The arguments that will be passed to the target upon execution.</param>
	/// <param name="flags">The flags that will be passed to the target upon execution.</param>
	public CommandExecutionContext(
		DiagnosticBag diagnostics,
		ICommandEngine engine,
		ICommandGroupInfo? groupTarget,
		ICommandInfo? commandTarget,
		IReadOnlyDictionary<IArgumentInfo, object?> arguments,
		IReadOnlyDictionary<IFlagInfo, object?> flags)
	{
		if (groupTarget is null && commandTarget is null)
			Throw.New.ArgumentException(nameof(groupTarget), "The execution context must have either the group or command target.");

		Diagnostics = diagnostics;
		Engine = engine;
		GroupTarget = groupTarget;
		CommandTarget = commandTarget;
		Arguments = arguments;
		Flags = flags;
	}

	/// <summary>Creates a new instance of the <see cref="CommandExecutionContext"/>.</summary>
	/// <param name="diagnostics">A collection of the diagnostics reported during the execution.</param>
	/// <param name="engine">The engine that is handling the execution.</param>
	/// <param name="target">The targetted group.</param>
	/// <param name="arguments">The arguments that will be passed to the target upon execution.</param>
	/// <param name="flags">The flags that will be passed to the target upon execution.</param>
	public CommandExecutionContext(
		DiagnosticBag diagnostics,
		ICommandEngine engine,
		ICommandGroupInfo target,
		IReadOnlyDictionary<IArgumentInfo, object?> arguments,
		IReadOnlyDictionary<IFlagInfo, object?> flags)
		: this(diagnostics, engine, target, null, arguments, flags) { }

	/// <summary>Creates a new instance of the <see cref="CommandExecutionContext"/>.</summary>
	/// <param name="diagnostics">A collection of the diagnostics reported during the execution.</param>
	/// <param name="engine">The engine that is handling the execution.</param>
	/// <param name="target">The targetted command.</param>
	/// <param name="arguments">The arguments that will be passed to the target upon execution.</param>
	/// <param name="flags">The flags that will be passed to the target upon execution.</param>
	public CommandExecutionContext(
		DiagnosticBag diagnostics,
		ICommandEngine engine,
		ICommandInfo target,
		IReadOnlyDictionary<IArgumentInfo, object?> arguments,
		IReadOnlyDictionary<IFlagInfo, object?> flags)
		: this(diagnostics, engine, null, target, arguments, flags) { }
	#endregion

	#region Methods
	/// <inheritdoc/>
	public void Handle(object? resultValue)
	{
		if (Handled)
			Throw.New.InvalidOperationException("This command execution operation has already been handled.");

		Handled = true;
		ResultValue = resultValue;
	}
	#endregion
}
