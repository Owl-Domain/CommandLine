using OwlDomain.Documentation.Document.Nodes;

namespace OwlDomain.CommandLine.Documentation;

/// <summary>
///	Represents a container for documentation information.
/// </summary>
/// <param name="summary">The summary information.</param>
/// <param name="remarks">The remarks information.</param>
/// <param name="example">The example informatino.</param>
/// <param name="returns">The return value information.</param>
public sealed class DocumentationInfo(
	IDocumentationNode? summary = null,
	IDocumentationNode? remarks = null,
	IDocumentationNode? example = null,
	IDocumentationNode? returns = null)
	: IDocumentationInfo
{
	#region Properties
	/// <inheritdoc/>
	public IDocumentationNode? Summary { get; } = summary;

	/// <inheritdoc/>
	public IDocumentationNode? Remarks { get; } = remarks;

	/// <inheritdoc/>
	public IDocumentationNode? Example { get; } = example;

	/// <inheritdoc/>
	public IDocumentationNode? Returns { get; } = returns;
	#endregion
}
