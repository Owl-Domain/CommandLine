namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents information about the default value of a flag/argument.
/// </summary>
public interface IDefaultValueInfo
{
	#region Properties
	/// <summary>The label for the default value.</summary>
	string Label { get; }
	#endregion
}
