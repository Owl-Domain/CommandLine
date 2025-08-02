namespace OwlDomain.CommandLine.Parsing.Values;

/// <summary>
/// 	Represents a parsing context for a flag value.
/// </summary>
/// <param name="engine">The command engine that the parse context belongs to.</param>
/// <param name="flag">The flag that the value is being parsed for.</param>
public sealed class FlagValueParseContext(ICommandEngine engine, IFlagInfo flag) : IFlagValueParseContext
{
	#region Properties
	/// <inheritdoc/>
	public ICommandEngine Engine { get; } = engine;

	/// <inheritdoc/>
	public IFlagInfo Flag { get; } = flag;

	/// <inheritdoc/>
	public Type ValueType => Flag.ValueType;
	#endregion
}
