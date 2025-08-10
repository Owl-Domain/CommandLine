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
	/// <exception cref="ArgumentException">
	/// Thrown if either:
	/// 	<list type="bullet">
	/// 		<item>The given <paramref name="class"/> type is not a .NET <see langword="class"/>.</item>
	/// 		<item>The given <paramref name="class"/> does not have a parameterless constructor.</item>
	/// 	</list>
	/// </exception>
	ICommandEngineBuilder From(Type @class);

	/// <summary>Includes the commands from the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type to include the commands from.</typeparam>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder From<T>() where T : class, new();

	/// <summary>Includes the given <paramref name="selector"/>.</summary>
	/// <param name="selector">The value parse selector to use for selecting the value parsers.</param>
	/// <returns>The used builder instance.</returns>
	/// <remarks>The order in which the selectors are added is the order in which they'll be used.</remarks>
	ICommandEngineBuilder WithSelector(IValueParserSelector selector);

	/// <summary>Creates and uses an instance of the value parser selector of the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of the value parser selector to create and use.</typeparam>
	/// <returns>The used builder instance.</returns>
	/// <remarks>The order in which the selectors are added is the order in which they'll be used.</remarks>
	ICommandEngineBuilder WithSelector<T>() where T : IValueParserSelector, new();

	/// <summary>Allows for customising the engine settings.</summary>
	/// <param name="callback">The callback which can be used to customise the engine settings.</param>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder Customise(Action<BuilderSettings> callback);

	/// <summary>Adds the given virtual <paramref name="command"/> to the engine.</summary>
	/// <param name="command">The virtual command to add.</param>
	/// <param name="predicate">The predicate used to decide in which groups the virtual command should be added to.</param>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder WithVirtualCommand(IVirtualCommandInfo command, Predicate<ICommandGroupInfo> predicate);

	/// <summary>Builds a new instance of the command engine.</summary>
	/// <returns>The built command engine.</returns>
	ICommandEngine Build();
	#endregion
}

/// <summary>
/// 	Contains various extension methods related to the <see cref="ICommandEngineBuilder"/>.
/// </summary>
public static class ICommandEngineBuilderExtensions
{
	#region Methods
	/// <summary>Builds a new instance of the command engine.</summary>
	/// <param name="builder">The command engine builder to use.</param>
	/// <param name="buildDuration">The amount of time it took to build the engine. This does not include the setup time.</param>
	/// <returns>The built command engine.</returns>
	public static ICommandEngine Build(this ICommandEngineBuilder builder, out TimeSpan buildDuration)
	{
		Stopwatch watch = Stopwatch.StartNew();

		ICommandEngine engine = builder.Build();

		watch.Stop();
		buildDuration = watch.Elapsed;

		return engine;
	}
	#endregion
}
