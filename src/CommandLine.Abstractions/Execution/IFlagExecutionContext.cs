namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents the flag context for the execution of a command.
/// </summary>
public interface IFlagExecutionContext : IReadOnlyDictionary<IFlagInfo, object?>
{
	#region Properties
	/// <summary>The value of the virtual help flag.</summary>
	/// <remarks>This will default to <see langword="false"/> if the flag wasn't provided.</remarks>
	bool Help { get; }
	#endregion
}
