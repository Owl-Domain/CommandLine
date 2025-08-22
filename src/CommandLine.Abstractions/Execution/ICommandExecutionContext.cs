namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents the context for the execution of a command.
/// </summary>
public interface ICommandExecutionContext
{
	#region Properties
	/// <summary>Whether the execution has been handled.</summary>
	bool Handled { get; }

	/// <summary>The result of executing the command.</summary>
	object? ResultValue { get; }

	/// <summary>A collection of the diagnostics reported during the execution.</summary>
	DiagnosticBag Diagnostics { get; }

	/// <summary>The engine that is handling the execution.</summary>
	ICommandEngine Engine { get; }

	/// <summary>The targetted group.</summary>
	ICommandGroupInfo GroupTarget { get; }

	/// <summary>The targetted command.</summary>
	ICommandInfo? CommandTarget { get; }

	/// <summary>The arguments that will be passed to the target upon execution.</summary>
	IReadOnlyDictionary<IArgumentInfo, object?> Arguments { get; }

	/// <summary>The flags that will be passed to the target upon execution.</summary>
	IFlagExecutionContext Flags { get; }

	/// <summary>The result of validating the command that will be executed.</summary>
	ICommandValidatorResult ValidatorResult { get; }

	/// <summary>The source of the cancellation token for the current operation.</summary>
	CancellationTokenSource CancellationTokenSource { get; }
	#endregion

	#region Methods
	/// <summary>Marks the execution operation as being handled.</summary>
	/// <param name="resultValue">The result of executing the command.</param>
	/// <exception cref="InvalidOperationException">Thrown if the execution has already been handled.</exception>
	void Handle(object? resultValue);
	#endregion
}

/// <summary>
/// 	Contains various extension methods related to the <see cref="ICommandExecutionContext"/>.
/// </summary>
public static class ICommandExecutionContextExtensions
{
	#region Methods
	/// <summary>
	/// 	Checks whether the given <paramref name="command"/> is the command
	/// 	targetted by the given execution <paramref name="context"/>.
	/// </summary>
	/// <param name="context">The execution context to check.</param>
	/// <param name="command">
	/// 	The command to check for, if this is <see langword="null"/>
	/// 	then the result will always be <see langword="false"/>.
	/// </param>
	/// <returns>
	/// 	<see langword="true"/> if the given <paramref name="command"/> is the command targetted
	/// 	by the given execution <paramref name="context"/>, <see langword="false"/> otherwise.
	/// </returns>
	public static bool IsCommand(this ICommandExecutionContext context, [NotNullWhen(true)] ICommandInfo? command)
	{
		return command is not null && context.CommandTarget == command;
	}

	/// <summary>checks whether the targetted command is the virtual help command.</summary>
	/// <param name="context">The execution context to use for the check.</param>
	/// <returns>
	/// 	<see langword="true"/> if the targetted command is the
	/// 	virtual help command, <see langword="false"/> otherwise.
	/// </returns>
	public static bool IsHelpCommand(this ICommandExecutionContext context) => context.IsCommand(context.Engine.VirtualCommands.Help);

	/// <summary>checks whether the targetted command is the virtual version command.</summary>
	/// <param name="context">The execution context to use for the check.</param>
	/// <returns>
	/// 	<see langword="true"/> if the targetted command is the
	/// 	virtual version command, <see langword="false"/> otherwise.
	/// </returns>
	public static bool IsVersionCommand(this ICommandExecutionContext context) => context.IsCommand(context.Engine.VirtualCommands.Version);
	#endregion
}