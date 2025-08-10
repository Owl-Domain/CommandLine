namespace OwlDomain.CommandLine.Commands;

/// <summary>
/// 	Represents information about a command that isn't linked to anything.
/// </summary>
/// <param name="name">The name associated with the command.</param>
/// <param name="group">The group that the command belongs to.</param>
/// <param name="flags">The flags that the command takes.</param>
/// <param name="arguments">The arguments that the command takes.</param>
/// <param name="documentation">The documentation for the command.</param>
/// <param name="hasResultValue">Whether the command has a result value.</param>
public sealed class VirtualCommandInfo(
	string? name,
	ICommandGroupInfo group,
	IReadOnlyCollection<IFlagInfo> flags,
	IReadOnlyList<IArgumentInfo> arguments,
	IDocumentationInfo? documentation,
	bool hasResultValue)
	: BaseCommandInfo(name, group, flags, arguments, documentation), IVirtualCommandInfo
{
	#region Properties
	/// <inheritdoc/>
	public override bool HasResultValue { get; } = hasResultValue;
	#endregion
}
