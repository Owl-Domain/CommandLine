namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Repreents a collection of known virtual flags.
/// </summary>
public sealed class VirtualFlags : IVirtualFlags
{
	#region Properties
	/// <inheritdoc/>
	public IVirtualFlagInfo? Help { get; init; }
	#endregion
}
