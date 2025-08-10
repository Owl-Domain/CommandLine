namespace OwlDomain.CommandLine.Commands;

/// <summary>
/// 	Repreents a collection of virtual commands.
/// </summary>
public interface IVirtualCommands
{
	#region Properties
	/// <summary>The virtual help command.</summary>
	IVirtualCommandInfo? Help { get; }
	#endregion
}
