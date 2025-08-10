namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents an executor for validated commands.
/// </summary>
public interface ICommandExecutor
{
	#region Events
	/// <summary>An event that is raised when the command is about to be executed.</summary>
	event CommandExecutionDelegate? OnExecute;
	#endregion

	#region Methods
	/// <summary>Executes the given <paramref name="validatorResult"/>.</summary>
	/// <param name="validatorResult">The validator result to execute.</param>
	/// <param name="callback">An optional callback which can be used to hook into the execution process.</param>
	/// <returns>The result of executing the given <paramref name="validatorResult"/>.</returns>
	ICommandExecutorResult Execute(ICommandValidatorResult validatorResult, CommandExecutionDelegate? callback = null);
	#endregion
}
