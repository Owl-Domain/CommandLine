namespace OwlDomain.CommandLine.Parsing.Values;

/// <summary>
/// 	Represents a parsing context for an argument value.
/// </summary>
public interface IArgumentValueParseContext : IValueParseContext
{
	#region Properties
	/// <summary>The argument that the value is being parsed for.</summary>
	IArgumentInfo Argument { get; }
	object IValueParseContext.Target => Argument;
	#endregion
}
