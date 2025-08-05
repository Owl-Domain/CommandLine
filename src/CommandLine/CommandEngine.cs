namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents the command line engine.
/// </summary>
/// <param name="rootGroup">The root command group.</param>
/// <param name="parser">The parser to use for parsing the commands.</param>
/// <param name="validator">The validator to use for validating the parsing commands.</param>
public sealed class CommandEngine(ICommandGroupInfo rootGroup, ICommandParser parser, ICommandValidator validator) : ICommandEngine
{
	#region Properties
	/// <inheritdoc/>
	public ICommandGroupInfo RootGroup { get; } = rootGroup;

	/// <inheritdoc/>
	public ICommandParser Parser { get; } = parser;

	/// <inheritdoc/>
	public ICommandValidator Validator { get; } = validator;
	#endregion

	#region Functions
	/// <summary>Creates a builder for a new command engine.</summary>
	/// <returns>The builder which can be used to customise the built command engine.</returns>
	public static ICommandEngineBuilder New() => new CommandEngineBuilder();
	#endregion

	#region Methods
	/// <inheritdoc/>
	public ICommandParserResult Parse(string[] fragments) => Parser.Parse(this, fragments);

	/// <inheritdoc/>
	public ICommandParserResult Parse(string command) => Parser.Parse(this, command);

	/// <inheritdoc/>
	public ICommandValidatorResult Validate(ICommandParserResult parserResult) => Validator.Validate(parserResult);

	/// <inheritdoc/>
	public IEngineExecutionResult Execute(ICommandValidatorResult validatorResult)
	{
		if (validatorResult.Diagnostics.Any())
			Throw.New.ArgumentException(nameof(validatorResult), $"Execution cannot be performed if there were validation errors.");

		if (validatorResult.ParserResult.LeafCommand is not ICommandParseResult command)
		{
			Throw.New.ArgumentException(nameof(validatorResult), $"The given validation result did not have a parsed command to execute.");
			return default; // Note(Nightowl): Never happens, needed for analysis to know the 'command' variable is always assigned later on;
		}

		DiagnosticBag diagnostics = [];

		Debug.Assert(command.Arguments.Count == command.CommandInfo.Arguments.Count);

		if (command.CommandInfo is IMethodCommandInfo methodCommand)
		{
			object? container = null;
			if (methodCommand.Method.IsStatic is false)
			{
				Type? containerType = methodCommand.Method.ReflectedType;
				Debug.Assert(containerType is not null);

				container = Activator.CreateInstance(containerType);
				Debug.Assert(container is not null);
			}

			object?[] parameters = new object?[methodCommand.Method.GetParameters().Length];

			foreach (IArgumentParseResult argument in command.Arguments)
				parameters[argument.ArgumentInfo.Position] = argument.Value.Value;

			_ = methodCommand.Method.Invoke(container, parameters);
		}
		else if (command.CommandInfo is IVirtualCommandInfo)
			Throw.New.NotImplementedException($"Executing virtual commands has not been implemented yet.");
		else
			Throw.New.InvalidOperationException($"Unknown command type ({command.CommandInfo?.GetType()}).");

		EngineExecutionResult result = new(validatorResult, diagnostics);

		return result;
	}
	#endregion
}
