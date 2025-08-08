namespace OwlDomain.CommandLine.Commands;

/// <summary>
/// 	Represents information about a command that isn't linked to anything.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class VirtualCommandInfo : IVirtualCommandInfo
{
	#region Properties
	/// <inheritdoc/>
	public string? Name { get; }

	/// <inheritdoc/>
	public ICommandGroupInfo Group { get; }

	/// <inheritdoc/>
	public IReadOnlyCollection<IFlagInfo> Flags { get; }

	/// <inheritdoc/>
	public IReadOnlyList<IArgumentInfo> Arguments { get; }

	/// <inheritdoc/>
	public IDocumentationInfo? Documentation { get; }

	/// <inheritdoc/>
	public bool HasResultValue { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="VirtualCommandInfo"/>.</summary>
	/// <param name="name">The name associated with the command.</param>
	/// <param name="group">The group that the command belongs to.</param>
	/// <param name="flags">The flags that the command takes.</param>
	/// <param name="arguments">The arguments that the command takes.</param>
	/// <param name="documentation">The documentation for the command.</param>
	/// <param name="hasResultValue">Whether the command has a result value.</param>
	public VirtualCommandInfo(
		string? name,
		ICommandGroupInfo group,
		IReadOnlyCollection<IFlagInfo> flags,
		IReadOnlyList<IArgumentInfo> arguments,
		IDocumentationInfo? documentation,
		bool hasResultValue)
	{
		name?.ThrowIfEmptyOrWhitespace(nameof(name));

		Group = group;
		Flags = flags;
		Arguments = arguments;
		Documentation = documentation;
		HasResultValue = hasResultValue;
	}
	#endregion

	#region Methods
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay() => $"Command {{ Name = ({Name}) }}";
	#endregion
}
