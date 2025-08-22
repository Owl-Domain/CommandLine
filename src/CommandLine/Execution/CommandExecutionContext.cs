namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents the context for the execution of a command.
/// </summary>
/// <param name="diagnostics">A collection of the diagnostics reported during the execution.</param>
/// <param name="engine">The engine that is handling the execution.</param>
/// <param name="groupTarget">The targetted group.</param>
/// <param name="commandTarget">The targetted command.</param>
/// <param name="arguments">The arguments that will be passed to the target upon execution.</param>
/// <param name="flags">The flags that will be passed to the target upon execution.</param>
/// <param name="validatorResult">The result of validating the command that will be executed.</param>
/// <param name="cancellationTokenSource">>The source of the cancellation token for the current operation.</param>
public sealed class CommandExecutionContext(
	DiagnosticBag diagnostics,
	ICommandEngine engine,
	ICommandGroupInfo groupTarget,
	ICommandInfo? commandTarget,
	IReadOnlyDictionary<IArgumentInfo, object?> arguments,
	IFlagExecutionContext flags,
	ICommandValidatorResult validatorResult,
	CancellationTokenSource cancellationTokenSource)
	: ICommandExecutionContext
{
	#region Properties
	/// <inheritdoc/>
	public bool Handled { get; private set; }

	/// <inheritdoc/>
	public object? ResultValue { get; private set; }

	/// <inheritdoc/>
	public DiagnosticBag Diagnostics { get; } = diagnostics;

	/// <inheritdoc/>
	public ICommandEngine Engine { get; } = engine;

	/// <inheritdoc/>
	public ICommandGroupInfo GroupTarget { get; } = groupTarget;

	/// <inheritdoc/>
	public ICommandInfo? CommandTarget { get; } = commandTarget;

	/// <inheritdoc/>
	public IReadOnlyDictionary<IArgumentInfo, object?> Arguments { get; } = arguments;

	/// <inheritdoc/>
	public IFlagExecutionContext Flags { get; } = flags;

	/// <inheritdoc/>
	public ICommandValidatorResult ValidatorResult { get; } = validatorResult;

	/// <inheritdoc/>
	public CancellationTokenSource CancellationTokenSource { get; } = cancellationTokenSource;
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
