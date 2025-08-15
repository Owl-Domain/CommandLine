namespace OwlDomain.CommandLine.Parsing.Values;

/// <summary>
/// 	Represents the parsing context for parsing values.
/// </summary>
public interface IValueParseContext : IParseContext
{
	#region Properties
	/// <summary>The information about the value.</summary>
	IValueInfo ValueInfo { get; }

	/// <summary>The target that the value is being parsed for.</summary>
	object Target { get; }
	#endregion
}
