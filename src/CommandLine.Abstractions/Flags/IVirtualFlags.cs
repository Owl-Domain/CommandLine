namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Repreents a collection of known virtual flags.
/// </summary>
public interface IVirtualFlags
{
	#region Properties
	/// <summary>The virtual help flag.</summary>
	IVirtualFlagInfo? Help { get; }
	#endregion
}
