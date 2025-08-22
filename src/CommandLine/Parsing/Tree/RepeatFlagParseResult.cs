namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a repeat flag parse operation.
/// </summary>
/// <param name="flagInfo">The parsed flag.</param>
/// <param name="prefix">The parsed flag prefix.</param>
/// <param name="name">The name of the parsed flag.</param>
/// <param name="repetition">The repetition count of the flag.</param>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class RepeatFlagParseResult(IFlagInfo flagInfo, TextToken prefix, TextToken name, int repetition)
: BaseFlagParseResult(prefix, name), IRepeatFlagParseResult
{
	#region Properties
	/// <inheritdoc/>
	public IFlagInfo FlagInfo { get; } = flagInfo;

	/// <inheritdoc/>
	public int Repetition { get; } = repetition;

	/// <inheritdoc/>
	public override IReadOnlyCollection<IFlagInfo> AffectedFlags { get; } = [flagInfo];
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string typeName = nameof(ValueFlagParseResult);
		const string nameName = nameof(Name);
		const string repetitionName = nameof(Repetition);

		return $"{typeName} {{ {nameName} = ({Name.Value}), {repetitionName} = ({Repetition:n0}) }}";
	}
	#endregion
}
