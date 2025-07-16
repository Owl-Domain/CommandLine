namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents a builder for the <see cref="ICommandEngine"/>.
/// </summary>
public interface ICommandEngineBuilder
{
	#region Methods
	/// <summary>Includes the commands from the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type to include the commands from.</typeparam>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder From<T>() where T : class;

	/// <summary>Builds a new instance of the command engine.</summary>
	/// <returns>The built command engine.</returns>
	ICommandEngine Build();
	#endregion
}
