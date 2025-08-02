namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of an argument parse operation.
/// </summary>
/// <param name="argumentInfo">The argument that was parsed.</param>
/// <param name="value">The value parsed for the argument.</param>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class ArgumentParseResult(IArgumentInfo argumentInfo, IValueParseResult value) : IArgumentParseResult
{
	#region Properties
	/// <inheritdoc/>
	public IArgumentInfo ArgumentInfo { get; } = argumentInfo;

	/// <inheritdoc/>
	public IValueParseResult Value { get; } = value;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public IEnumerable<TextToken> EnumerateTokens() => Value.EnumerateTokens();
	#endregion

	#region Helpers
	private string DebuggerDisplay()
	{
		const string typeName = nameof(ArgumentParseResult);
		const string argumentInfoName = $"{nameof(ArgumentInfo)}.{nameof(ArgumentInfo.Name)}";
		const string valueName = nameof(Value);

		return $"{typeName} {{ {argumentInfoName} = ({ArgumentInfo.Name}), {valueName} = ({Value.Value}) }}";
	}
	#endregion
}
