namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents information about a flag that isn't linked to anything.
/// </summary>
/// <typeparam name="T">The type of the flag's value.</typeparam>
/// <param name="kind">The type of the flag.</param>
/// <param name="longName">The long name of the flag.</param>
/// <param name="shortName">The short name of the flag.</param>
/// <param name="valueInfo">The information about the flag's value.</param>
/// <param name="defaultValueInfo">The information aboue the flag's default value.</param>
/// <param name="documentation">The documentation for the flag.</param>
public class VirtualFlagInfo<T>(
	FlagKind kind,
	string? longName,
	char? shortName,
	IValueInfo<T> valueInfo,
	IDefaultValueInfo? defaultValueInfo,
	IDocumentationInfo? documentation)
	: BaseFlagInfo<T>(kind, longName, shortName, valueInfo, defaultValueInfo, documentation), IVirtualFlagInfo<T>
{
}
