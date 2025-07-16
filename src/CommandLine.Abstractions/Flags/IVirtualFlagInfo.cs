namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents information about a flag that isn't linked to anything.
/// </summary>
public interface IVirtualFlagInfo : IFlagInfo { }

/// <summary>
/// 	Represents information about a flag that isn't linked to anything.
/// </summary>
/// <typeparam name="T">The type of the flag's value.</typeparam>
public interface IVirtualFlagInfo<out T> : IVirtualFlagInfo, IFlagInfo<T> { }