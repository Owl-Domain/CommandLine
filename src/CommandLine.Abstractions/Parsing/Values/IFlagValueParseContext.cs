namespace OwlDomain.CommandLine.Parsing.Values;

/// <summary>
/// 	Represents a parsing context for a flag value.
/// </summary>
public interface IFlagValueParseContext : IValueParseContext
{
	#region Properties
	/// <summary>The flag that the value is being parsed for.</summary>
	IFlagInfo Flag { get; }
	object IValueParseContext.Target => Flag;
	#endregion
}
