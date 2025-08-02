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
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="VirtualCommandInfo"/>.</summary>
	/// <param name="name">The name associated with the command.</param>
	/// <param name="group">The group that the command belongs to.</param>
	/// <param name="flags">The flags that the command takes.</param>
	/// <param name="arguments">The arguments that the command takes.</param>
	public VirtualCommandInfo(string? name, ICommandGroupInfo group, IReadOnlyCollection<IFlagInfo> flags, IReadOnlyList<IArgumentInfo> arguments)
	{
		name?.ThrowIfEmptyOrWhitespace(nameof(name));

		Group = group;
		Flags = flags;
		Arguments = arguments;
	}
	#endregion

	#region Methods
	private string DebuggerDisplay() => $"Command {{ Name = ({Name}) }}";
	#endregion
}
