namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents the execution result for a parsed and validated command.
/// </summary>
public interface ICommandExecutorResult : IStageResult
{
	#region Properties
	/// <summary>The validation result that was executed.</summary>
	ICommandValidatorResult ValidatorResult { get; }

	/// <summary>The (optional) result returned by the command.</summary>
	object? Result { get; }
	#endregion
}
