namespace OwlDomain.CommandLine.Parsing;

/// <summary>
/// 	Represents the base implementation for the <see cref="ICommandParser"/>.
/// </summary>
public abstract class BaseCommandParser : ICommandParser
{
	#region Methods
	/// <inheritdoc/>
	public ICommandParserResult Parse(ICommandEngine engine, string command)
	{
		TextParser parser = new([command], true);

		return Parse(engine, parser);
	}

	/// <inheritdoc/>
	public ICommandParserResult Parse(ICommandEngine engine, string[] fragments)
	{
		TextParser parser = new(fragments, false);

		return Parse(engine, parser);
	}

	/// <summary>Parses the command loaded into the given text <paramref name="parser"/>.</summary>
	/// <param name="engine">The engine to use for the parsing operation.</param>
	/// <param name="parser">The text parser to use for parsing.</param>
	/// <returns>The result of the parsing operation.</returns>
	protected abstract ICommandParserResult Parse(ICommandEngine engine, ITextParser parser);
	#endregion
}
