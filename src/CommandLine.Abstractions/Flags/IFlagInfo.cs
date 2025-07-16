namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents information about a flag.
/// </summary>
public interface IFlagInfo
{
	#region Properties
	/// <summary>The long name of the flag.</summary>
	string? LongName { get; }

	/// <summary>The short name of the flag.</summary>
	char? ShortName { get; }

	/// <summary>The type of the flag's value.</summary>
	Type ValueType { get; }

	/// <summary>Whether the flag has to be set when executing a command.</summary>
	bool IsRequired { get; }

	/// <summary>The default value for the flag.</summary>
	object? DefaultValue { get; }
	#endregion
}

/// <summary>
/// 	Represents information about a flag.
/// </summary>
/// <typeparam name="T">The type of the flag's value.</typeparam>
public interface IFlagInfo<out T> : IFlagInfo
{
	#region Properties
	Type IFlagInfo.ValueType => typeof(T);

	/// <summary>The default value for the flag.</summary>
	new T? DefaultValue { get; }
	object? IFlagInfo.DefaultValue => DefaultValue;
	#endregion
}