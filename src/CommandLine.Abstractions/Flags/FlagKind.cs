namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents the different flag types.
/// </summary>
public enum FlagKind : byte
{
	/// <summary>Represents a regular flag that can have a value.</summary>
	Regular,

	/// <summary>Represents a flag that acts as a boolean toggle.</summary>
	Toggle,

	/// <summary>Represents a repeat style flag that can represent an integer scale.</summary>
	Repeat,
}
