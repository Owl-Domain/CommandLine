namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents an executor for validated commands.
/// </summary>
public interface ICommandExecutor
{
	#region Methods
	/// <summary>Executes the given <paramref name="validatorResult"/>.</summary>
	/// <param name="validatorResult">The validator result to execute.</param>
	/// <returns>The result of executing the given <paramref name="validatorResult"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if the given <paramref name="validatorResult"/> cannot be executed.</exception>

	ICommandExecutorResult Execute(ICommandValidatorResult validatorResult);
	#endregion
}
