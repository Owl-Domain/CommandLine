namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents the command line engine.
/// </summary>
/// <param name="rootGroup">The root command group.</param>
public sealed class CommandEngine(ICommandGroupInfo rootGroup) : ICommandEngine
{
	#region Properties
	/// <inheritdoc/>
	public ICommandGroupInfo RootGroup { get; } = rootGroup;
	#endregion

	#region Functions
	/// <summary>Creates a builder for a new command engine.</summary>
	/// <returns>The builder which can be used to customise the built command engine.</returns>
	public static ICommandEngineBuilder New() => new CommandEngineBuilder();
	#endregion

	#region Methods
	/// <inheritdoc/>
	public void Run(string[] fragments) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void Run(string command) => throw new NotImplementedException();
	#endregion
}
