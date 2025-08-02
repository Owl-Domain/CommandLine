namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents the command line engine.
/// </summary>
public interface ICommandEngine
{
	#region Properties
	/// <summary>The root command group.</summary>
	ICommandGroupInfo RootGroup { get; }
	#endregion

	#region Methods
	/// <summary>Parses the given <paramref name="fragments"/>.</summary>
	/// <param name="fragments">The fragments to parse.</param>
	/// <returns>The result of the parse operation.</returns>
	IEngineParseResult Parse(string[] fragments);

	/// <summary>Parses the given command <paramref name="text"/>.</summary>
	/// <param name="text">The command text to parse.</param>
	/// <returns>The result of the parse operation.</returns>
	IEngineParseResult Parse(string text);
	#endregion
}
