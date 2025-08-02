namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	The kind of the text token.
/// </summary>
public enum TextTokenKind : byte
{
	/// <summary>Represents an error token.</summary>
	Error,

	/// <summary>Represents a token that hasn't been processed.</summary>
	Unprocessed,

	/// <summary>Represents a token for the name of a command group.</summary>
	GroupName,

	/// <summary>Represents a token for the name of a command.</summary>
	CommandName,

	/// <summary>Represents a token for the name of a flag.</summary>
	FlagName,

	/// <summary>Represents a token for a value.</summary>
	Value,

	/// <summary>Represents a token for a symbol.</summary>
	Symbol,
}
