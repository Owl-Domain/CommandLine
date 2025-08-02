namespace OwlDomain.CommandLine.Parsing.Values;

/// <summary>
/// 	Represents the parsing context for parsing values.
/// </summary>
public interface IValueParseContext : IParseContext
{
	#region Properties
	/// <summary>The type of the value being parsed.</summary>
	Type ValueType { get; }

	/// <summary>The target that the value is being parsed for.</summary>
	object Target { get; }
	#endregion
}
