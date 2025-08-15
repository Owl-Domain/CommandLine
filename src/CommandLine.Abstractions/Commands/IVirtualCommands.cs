namespace OwlDomain.CommandLine.Commands;

/// <summary>
/// 	Repreents a collection of virtual commands.
/// </summary>
public interface IVirtualCommands
{
	#region Properties
	/// <summary>The virtual help command.</summary>
	/// <remarks>This command should show the project documentation.</remarks>
	IVirtualCommandInfo? Help { get; }

	/// <summary>The virtual help command.</summary>
	/// <remarks>This command should show the project version.</remarks>
	IVirtualCommandInfo? Version { get; }
	#endregion
}
