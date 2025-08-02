namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a command engine parse operation.
/// </summary>
public interface IEngineParseResult : IParseResult
{
	#region Properties
	/// <summary>The engine that was used for the parsing operation.</summary>
	ICommandEngine Engine { get; }

	/// <summary>The diagnostics that occurred during the parsing operation.</summary>
	IDiagnosticBag Diagnostics { get; }

	/// <summary>The result for the parsed command or command group.</summary>
	/// <remarks>This might be <see langword="null"/> if the parsing failed.</remarks>
	IParseResult? CommandOrGroup { get; }
	#endregion
}
