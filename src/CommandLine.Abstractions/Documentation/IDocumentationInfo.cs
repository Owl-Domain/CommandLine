using OwlDomain.Documentation.Document.Nodes;

namespace OwlDomain.CommandLine.Documentation;

/// <summary>
///	Represents a container for documentation information.
/// </summary>
public interface IDocumentationInfo
{
	#region Properties
	/// <summary>The summary information.</summary>
	IDocumentationNode Summary { get; }

	/// <summary>The remarks information.</summary>
	IDocumentationNode? Remarks { get; }
	#endregion
}
