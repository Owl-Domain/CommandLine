namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents information about a flag.
/// </summary>
public interface IFlagInfo
{
	#region Properties
	/// <summary>The type of the flag.</summary>
	FlagKind Kind { get; }

	/// <summary>The long name of the flag.</summary>
	string? LongName { get; }

	/// <summary>The short name of the flag.</summary>
	char? ShortName { get; }

	/// <summary>The information about the flag's value.</summary>
	IValueInfo ValueInfo { get; }

	/// <summary>The information about the flag's default value.</summary>
	/// <remarks>If this value is <see langword="null"/> then the flag must be required.</remarks>
	IDefaultValueInfo? DefaultValueInfo { get; }

	/// <summary>The documentation for the flag.</summary>
	IDocumentationInfo? Documentation { get; }
	#endregion
}

/// <summary>
/// 	Represents information about a flag.
/// </summary>
/// <typeparam name="T">The type of the flag's value.</typeparam>
public interface IFlagInfo<out T> : IFlagInfo
{
	#region Properties
	/// <summary>The information about the flag's value.</summary>
	new IValueInfo<T> ValueInfo { get; }
	IValueInfo IFlagInfo.ValueInfo => ValueInfo;
	#endregion
}
