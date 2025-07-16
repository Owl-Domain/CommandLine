namespace OwlDomain.CommandLine.Commands;

/// <summary>
/// 	Represents information about a command that is linked to a method.
/// </summary>
public interface IMethodCommandInfo : ICommandInfo
{
	#region Properties
	/// <summary>The method that the command is linked to.</summary>
	MethodInfo Method { get; }
	#endregion
}
