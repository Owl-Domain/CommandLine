namespace OwlDomain.CommandLine.Parsing.Values;

/// <summary>
/// 	Represents a parsing context for an argument value.
/// </summary>
/// <param name="engine">The command engine that the parse context belongs to.</param>
/// <param name="argument">The argument that the value is being parsed for.</param>
/// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
public sealed class ArgumentValueParseContext(
	ICommandEngine engine,
	IArgumentInfo argument,
	CancellationToken cancellationToken)
	: IArgumentValueParseContext
{
	#region Properties
	/// <inheritdoc/>
	public ICommandEngine Engine { get; } = engine;

	/// <inheritdoc/>
	public IArgumentInfo Argument { get; } = argument;

	/// <inheritdoc/>
	public IValueInfo ValueInfo => Argument.ValueInfo;

	/// <inheritdoc/>
	public CancellationToken CancellationToken { get; } = cancellationToken;
	#endregion
}
