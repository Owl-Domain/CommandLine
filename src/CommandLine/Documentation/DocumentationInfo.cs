using OwlDomain.Documentation.Document.Nodes;

namespace OwlDomain.CommandLine.Documentation;

/// <summary>
///	Represents a container for documentation information.
/// </summary>
public sealed class DocumentationInfo(IDocumentationNode summary, IDocumentationNode? remarks) : IDocumentationInfo
{
	#region Properties
	/// <inheritdoc/>
	public IDocumentationNode Summary { get; } = summary;

	/// <inheritdoc/>
	public IDocumentationNode? Remarks { get; } = remarks;
	#endregion
}
