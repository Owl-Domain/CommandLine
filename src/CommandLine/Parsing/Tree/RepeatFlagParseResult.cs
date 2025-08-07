namespace OwlDomain.CommandLine.Parsing.Tree;

/// <summary>
/// 	Represents the result of a repeat flag parse operation.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class RepeatFlagParseResult : BaseFlagParseResult, IRepeatFlagParseResult
{
	#region Properties
	/// <inheritdoc/>
	public IFlagInfo FlagInfo { get; }

	/// <inheritdoc/>
	public int Repetition { get; }
	#endregion

	#region Constructors
	/// <param name="flagInfo">The parsed flag.</param>
	/// <param name="prefix">The parsed flag prefix.</param>
	/// <param name="name">The name of the parsed flag.</param>
	public RepeatFlagParseResult(IFlagInfo flagInfo, TextToken prefix, TextToken name) : base(prefix, name)
	{
		if (name.Value is not string)
			Throw.New.ArgumentException(nameof(name), $"Expected the {nameof(name)} token to have a string value.");

		FlagInfo = flagInfo;
		Repetition = ((string)name.Value).Length;
	}
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
