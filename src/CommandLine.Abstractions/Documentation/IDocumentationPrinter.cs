namespace OwlDomain.CommandLine.Documentation;

/// <summary>
/// 	Represents a printer for documentation info.
/// </summary>
public interface IDocumentationPrinter
{
	#region Methods
	/// <summary>Prints the documentation information for the given command <paramref name="engine"/>.</summary>
	/// <param name="engine">The command engine to print the documentation information for.</param>
	void Print(ICommandEngine engine);

	/// <summary>Prints the documentation information for the given command <paramref name="group"/>.</summary>
	/// <param name="engine">The command engine that the given command <paramref name="group"/> belongs to.</param>
	/// <param name="group">The command group to print the documentation for.</param>
	void Print(ICommandEngine engine, ICommandGroupInfo group);

	/// <summary>Prints the documentation information for the given <paramref name="command"/>.</summary>
	/// <param name="engine">The command engine that the given <paramref name="command"/> belongs to.</param>
	/// <param name="command">The command to print the documentation for.</param>
	void Print(ICommandEngine engine, ICommandInfo command);
	#endregion
}
