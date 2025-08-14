namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents a parsing context.
/// </summary>
public interface IParseContext
{
	#region Properties
	/// <summary>The command engine that the parse context belongs to.</summary>
	ICommandEngine Engine { get; }

	/// <summary>A cancellation token that can be used to cancel the operation.</summary>
	CancellationToken CancellationToken { get; }
	#endregion
}
