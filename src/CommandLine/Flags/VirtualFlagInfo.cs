namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents information about a flag that isn't linked to anything.
/// </summary>
/// <typeparam name="T">The type of the flag's value.</typeparam>
/// <param name="kind">The type of the flag.</param>
/// <param name="longName">The long name of the flag.</param>
/// <param name="shortName">The short name of the flag.</param>
/// <param name="isRequired">Whether the flag has to be set when executing the command.</param>
/// <param name="defaultValue">The default value for the flag.</param>
/// <param name="parser">The value parser selected for the flag.</param>
public class VirtualFlagInfo<T>(
	FlagKind kind,
	string? longName,
	char? shortName,
	bool isRequired,
	T? defaultValue,
	IValueParser<T> parser)
	: BaseFlagInfo<T>(kind, longName, shortName, isRequired, defaultValue, parser), IVirtualFlagInfo<T>
{
}
