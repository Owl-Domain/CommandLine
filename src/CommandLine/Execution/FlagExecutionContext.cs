namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents the flag context for the execution of a command.
/// </summary>
/// <param name="engine">The engine that is handling the execution.</param>
/// <param name="flags">The flags that will be passed to the target upon execution.</param>
public sealed class FlagExecutionContext(ICommandEngine engine, IReadOnlyDictionary<IFlagInfo, object?> flags) : IFlagExecutionContext
{
	#region Fields
	private readonly ICommandEngine _engine = engine;
	private readonly IReadOnlyDictionary<IFlagInfo, object?> _flags = flags;
	#endregion

	#region Properties
	/// <inheritdoc/>
	public IEnumerable<IFlagInfo> Keys => _flags.Keys;

	/// <inheritdoc/>
	public IEnumerable<object?> Values => _flags.Values;

	/// <inheritdoc/>
	public int Count => _flags.Count;

	/// <inheritdoc/>
	public bool Help => TryGet(_engine.VirtualFlags.Help, false);
	#endregion

	#region Indexers
	/// <inheritdoc/>
	public object? this[IFlagInfo key] => _flags[key];
	#endregion

	#region Methods
	/// <inheritdoc/>
	public bool ContainsKey(IFlagInfo key) => _flags.ContainsKey(key);

	/// <inheritdoc/>
	public bool TryGetValue(IFlagInfo key, [MaybeNullWhen(false)] out object? value)
	{
		return _flags.TryGetValue(key, out value);
	}

	/// <inheritdoc/>
	public IEnumerator<KeyValuePair<IFlagInfo, object?>> GetEnumerator() => _flags.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_flags).GetEnumerator();
	#endregion

	#region Helpers
	private T? TryGet<T>(IFlagInfo? flag, T? fallback)
	{
		if (flag is null || (_flags.TryGetValue(flag, out object? value) is false))
			return fallback;

		return (T?)value;
	}
	#endregion
}
