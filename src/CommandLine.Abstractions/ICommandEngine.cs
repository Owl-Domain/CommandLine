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
	/// <summary>Parses and executes the given command <paramref name="fragments"/>.</summary>
	/// <param name="fragments">The fragments to parse and execute.</param>
	void Run(string[] fragments);

	/// <summary>Parses and executes the given <paramref name="command"/> text.</summary>
	/// <param name="command">The command text to parse and execute.</param>
	void Run(string command);
	#endregion
}
