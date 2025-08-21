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
		TextParser parser = new([command], ParsingMode.Lazy);

		return Parse(engine, parser);
	}

	/// <inheritdoc/>
	public ICommandParserResult Parse(ICommandEngine engine, string[] fragments)
	{
		TextParser parser = new(fragments, ParsingMode.Greedy);

		return Parse(engine, parser);
	}

	/// <summary>Parses the command loaded into the given text <paramref name="parser"/>.</summary>
	/// <param name="engine">The engine to use for the parsing operation.</param>
	/// <param name="parser">The text parser to use for parsing.</param>
	/// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
	/// <returns>The result of the parsing operation.</returns>
	protected abstract ICommandParserResult Parse(ICommandEngine engine, ITextParser parser, CancellationToken cancellationToken);
	#endregion

	#region Helpers
	private ICommandParserResult Parse(ICommandEngine engine, TextParser parser)
	{
		TimeSpan timeout = engine.Settings.ParsingTimeout;

		if (timeout == TimeSpan.Zero)
			return Parse(engine, parser, default);

		Stopwatch watch = Stopwatch.StartNew();
		CancellationTokenSource source = new(timeout);
		try
		{
			return Parse(engine, parser, source.Token);
		}
		catch (OperationCanceledException)
		{
			watch.Stop();
			return new CommandParserResult(false, false, engine, this, new DiagnosticBag(), null, [], watch.Elapsed);
		}
	}
	#endregion
}
