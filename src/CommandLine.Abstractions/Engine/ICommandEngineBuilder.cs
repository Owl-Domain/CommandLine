namespace OwlDomain.CommandLine.Engine;

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
	ICommandEngineBuilder From<T>() where T : class;

	/// <summary>Includes the given <paramref name="selector"/>.</summary>
	/// <param name="selector">The value parse selector to use for selecting the value parsers.</param>
	/// <returns>The used builder instance.</returns>
	/// <remarks>The order in which the selectors are added is the order in which they'll be used.</remarks>
	ICommandEngineBuilder With(IValueParserSelector selector);

	/// <summary>Includes the given command <paramref name="injector"/> for injecting command values.</summary>
	/// <param name="injector">The injector to include.</param>
	/// <returns>The used builder instance.</returns>
	/// <remarks>The order in which the injectors are added is the order in which they'll be used.</remarks>
	ICommandEngineBuilder With(ICommandInjector injector);

	/// <summary>Includes the given default value label <paramref name="provider"/> for getting default value labels.</summary>
	/// <param name="provider">The provider to include.</param>
	/// <returns>The used builder instance.</returns>
	/// <remarks>The order in which the providers are added is the order in which they'll be used.</remarks>
	ICommandEngineBuilder With(IDefaultValueLabelProvider provider);

	/// <summary>Replaces the name extractor component of the engine.</summary>
	/// <param name="extractor">The name extractor to use instead of the default one.</param>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder With(INameExtractor extractor);

	/// <summary>Replaces the command parser component of the engine.</summary>
	/// <param name="parser">The command parser to use instead of the default one.</param>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder With(ICommandParser parser);

	/// <summary>Replaces the command validator component of the engine.</summary>
	/// <param name="validator">The command validator to use instead of the default one.</param>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder With(ICommandValidator validator);

	/// <summary>Replaces the command executor component of the engine.</summary>
	/// <param name="executor">The command executor to use instead of the default one.</param>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder With(ICommandExecutor executor);

	/// <summary>Replaces the documentation provider component of the engine.</summary>
	/// <param name="provider">The documentation provider to use instead of the default one.</param>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder With(IDocumentationProvider provider);

	/// <summary>Replaces the documentation printer component of the engine.</summary>
	/// <param name="printer">The documentation printer to use instead of the default one.</param>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder With(IDocumentationPrinter printer);

	/// <summary>Replaces the output printer component of the engine.</summary>
	/// <param name="printer">The output printer to use instead of the default one.</param>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder With(IOutputPrinter printer);

	/// <summary>Marks the builder to include the default value parsers in the engine.</summary>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder WithDefaultValueParsers();

	/// <summary>Marks the builder to exclude the default value parsers from the engine.</summary>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder WithoutDefaultValueParsers();

	/// <summary>Marks the builder to include the default command injector in the engine.</summary>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder WithDefaultInjector();

	/// <summary>Marks the builder to exclude the default command injector from the engine.</summary>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder WithoutDefaultInjector();

	/// <summary>Marks the builder to include the default value label provider in the engine.</summary>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder WithDefaultValueLabelProvider();

	/// <summary>Marks the builder to exclude the default value label provider from the engine.</summary>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder WithoutDefaultValueLabelProvider();

	/// <summary>Allows for customising the engine settings.</summary>
	/// <param name="callback">The callback which can be used to customise the engine settings.</param>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder Customise(Action<BuilderSettings> callback);

	/// <summary>Adds the given virtual <paramref name="command"/> to the engine.</summary>
	/// <param name="command">The virtual command to add.</param>
	/// <param name="predicate">The predicate used to decide which groups the virtual <paramref name="command"/> should be added to.</param>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder WithVirtualCommand(IVirtualCommandInfo command, Predicate<ICommandGroupInfo> predicate);

	/// <summary>Adds the given virtual <paramref name="flag"/> to the engine.</summary>
	/// <param name="flag">The virtual flag to add.</param>
	/// <param name="groupPredicate">The predicate used to decide which groups the virtual <paramref name="flag"/> should be added to.</param>
	/// <param name="commandPredicate">The predicate should to decide which commands the virtual <paramref name="flag"/> should be added to.</param>
	/// <returns>The used builder instance.</returns>
	ICommandEngineBuilder WirthVirtualFlag(IVirtualFlagInfo flag, Predicate<ICommandGroupInfo> groupPredicate, Predicate<ICommandInfo> commandPredicate);

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

	/// <summary>Creates and uses an instance of the value parser selector of the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of the value parser selector to create and use.</typeparam>
	/// <param name="builder">The builder to pass the created value parser selector to.</param>
	/// <returns>The used builder instance.</returns>
	/// <remarks>The order in which the selectors are added is the order in which they'll be used.</remarks>
	public static ICommandEngineBuilder WithValueParserSelector<T>(this ICommandEngineBuilder builder) where T : IValueParserSelector, new()
	{
		T selector = new();
		return builder.With(selector);
	}

	/// <summary>Creates and includes an instance of the command injector of the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of the command injector to create and include.</typeparam>
	/// <param name="builder">The builder to pass the created injector to.</param>
	/// <returns>The used builder instance.</returns>
	/// <remarks>The order in which the injectors are added is the order in which they'll be used.</remarks>
	public static ICommandEngineBuilder WithCommandInjector<T>(this ICommandEngineBuilder builder) where T : ICommandInjector, new()
	{
		T injector = new();
		return builder.With(injector);
	}

	/// <summary>Creates and includes an instance of the default value label provider of the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of the default value provider to create and include.</typeparam>
	/// <param name="builder">The builder to pass the created provider to.</param>
	/// <returns>The used builder instance.</returns>
	/// <remarks>The order in which the providers are added is the order in which they'll be used.</remarks>
	public static ICommandEngineBuilder WithDefaultValueLabelProvider<T>(this ICommandEngineBuilder builder) where T : IDefaultValueLabelProvider, new()
	{
		T provider = new();
		return builder.With(provider);
	}

	/// <summary>Creates and uses an instance of the name extractor of the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of the name extractor to create and use.</typeparam>
	/// <param name="builder">The builder to pass the created extractor to.</param>
	/// <returns>The used builder instance.</returns>
	public static ICommandEngineBuilder WithNameExtractor<T>(this ICommandEngineBuilder builder) where T : INameExtractor, new()
	{
		T extractor = new();
		return builder.With(extractor);
	}

	/// <summary>Creates and uses an instance of the command parser of the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of the command parser to create and use.</typeparam>
	/// <param name="builder">The builder to pass the created parser to.</param>
	/// <returns>The used builder instance.</returns>
	public static ICommandEngineBuilder WithCommandParser<T>(this ICommandEngineBuilder builder) where T : ICommandParser, new()
	{
		T parser = new();
		return builder.With(parser);
	}

	/// <summary>Creates and uses an instance of the command validator of the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of the command validator to create and use.</typeparam>
	/// <param name="builder">The builder to pass the created validator to.</param>
	/// <returns>The used builder instance.</returns>
	public static ICommandEngineBuilder WithCommandValidator<T>(this ICommandEngineBuilder builder) where T : ICommandValidator, new()
	{
		T validator = new();
		return builder.With(validator);
	}

	/// <summary>Creates and uses an instance of the command executor of the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of the command executor to create and use.</typeparam>
	/// <param name="builder">The builder to pass the created executor to.</param>
	/// <returns>The used builder instance.</returns>
	public static ICommandEngineBuilder WithCommandExecutor<T>(this ICommandEngineBuilder builder) where T : ICommandExecutor, new()
	{
		T executor = new();
		return builder.With(executor);
	}

	/// <summary>Creates and uses an instance of the documentation provider of the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of the documentation provider to create and use.</typeparam>
	/// <param name="builder">The builder to pass the created provider to.</param>
	/// <returns>The used builder instance.</returns>
	public static ICommandEngineBuilder WithDocumentationProvider<T>(this ICommandEngineBuilder builder) where T : IDocumentationProvider, new()
	{
		T provider = new();
		return builder.With(provider);
	}

	/// <summary>Creates and uses an instance of the documentation printer of the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of the documentation printer to create and use.</typeparam>
	/// <param name="builder">The builder to pass the created printer to.</param>
	/// <returns>The used builder instance.</returns>
	public static ICommandEngineBuilder WithDocumentationPrinter<T>(this ICommandEngineBuilder builder) where T : IDocumentationPrinter, new()
	{
		T printer = new();
		return builder.With(printer);
	}

	/// <summary>Creates and uses an instance of the output printer of the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of the output printer to create and use.</typeparam>
	/// <param name="builder">The builder to pass the created printer to.</param>
	/// <returns>The used builder instance.</returns>
	public static ICommandEngineBuilder WithOutputPrinter<T>(this ICommandEngineBuilder builder) where T : IOutputPrinter, new()
	{
		T printer = new();
		return builder.With(printer);
	}
	#endregion
}
