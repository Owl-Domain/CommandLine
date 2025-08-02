namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of an argument parse operation.
/// </summary>
public interface IArgumentParseResult : IParseResult
{
	#region Properties
	/// <summary>The argument that was parsed.</summary>
	IArgumentInfo ArgumentInfo { get; }

	/// <summary>The value parsed for the argument.</summary>
	IValueParseResult Value { get; }
	#endregion
}
