namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents a builder for the <see cref="ICommandEngine"/>.
/// </summary>
public interface ICommandEngineBuilder
{
	#region Methods
	/// <summary>Includes the commands from the given <paramref name="class"/> type.</summary>
	/// <param name="class">The class type to include the commands from.</param>
	/// <returns>The used builder instance.</returns>
	/// <exception cref="ArgumentException">Thrown if the given <paramref name="class"/> type is not a .NET <see langword="class"/>.</exception>
	ICommandEngineBuilder From(Type @class);

	/// <summary>Includes the commands from the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type to include the commands from.</typeparam>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder From<T>() where T : class;

	/// <summary>Builds a new instance of the command engine.</summary>
	/// <returns>The built command engine.</returns>
	ICommandEngine Build();
	#endregion
}
