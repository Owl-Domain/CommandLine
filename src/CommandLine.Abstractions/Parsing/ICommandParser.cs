namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents a parser for commands.
/// </summary>
public interface ICommandParser
{
	#region Methods
	/// <summary>Parses the given <paramref name="command"/>.</summary>
	/// <param name="engine">The engine to use for the parsing operation.</param>
	/// <param name="command">The full <paramref name="command"/> to parse.</param>
	/// <returns>The result of the parsing operation.</returns>
	/// <remarks>
	/// 	This overload is intended to be used in REPL-like circumstances
	/// 	where you only have access to the full command.
	/// </remarks>
	ICommandParserResult Parse(ICommandEngine engine, string command);

	/// <summary>Parses the given command <paramref name="fragments"/>.</summary>
	/// <param name="engine">The engine to use for the parsing operation.</param>
	/// <param name="fragments">The command <paramref name="fragments"/> to parse.</param>
	/// <returns>The result of the parsing operation.</returns>
	/// <remarks>
	/// 	This overload is intended to be used when you have the tokenised command fragments,
	/// 	for example when you have access to the <see cref="Environment.GetCommandLineArgs"/>.
	/// </remarks>
	ICommandParserResult Parse(ICommandEngine engine, string[] fragments);
	#endregion
}
