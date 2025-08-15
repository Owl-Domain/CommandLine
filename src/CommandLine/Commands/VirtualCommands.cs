namespace OwlDomain.CommandLine.Commands;

/// <summary>
/// 	Repreents a collection of known virtual commands.
/// </summary>
public sealed class VirtualCommands : IVirtualCommands
{
	#region Properties
	/// <inheritdoc/>
	public IVirtualCommandInfo? Help { get; init; }

	/// <inheritdoc/>
	public IVirtualCommandInfo? Version { get; init; }
	#endregion
}
