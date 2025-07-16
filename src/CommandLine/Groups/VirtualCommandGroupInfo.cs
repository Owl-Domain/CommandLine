namespace OwlDomain.CommandLine.Groups;

/// <summary>
/// 	Represents information about a command group that is associated with a class.
/// </summary>
public sealed class VirtualCommandGroupInfo : IVirtualCommandGroupInfo
{
	#region Properties
	/// <inheritdoc/>
	public string? Name { get; }

	/// <inheritdoc/>
	public ICommandGroupInfo? Parent { get; }

	/// <inheritdoc/>
	public IReadOnlyCollection<IFlagInfo> Flags { get; }

	/// <inheritdoc/>
	public IReadOnlyDictionary<string, ICommandGroupInfo> Groups { get; }

	/// <inheritdoc/>
	public IReadOnlyDictionary<string, ICommandInfo> Commands { get; }

	/// <inheritdoc/>
	public ICommandInfo? ImplicitCommand { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="VirtualCommandGroupInfo"/>.</summary>
	/// <param name="name">The name associated with the command group.</param>
	/// <param name="parent">The parent command group.</param>
	/// <param name="flags">The flags that can be passed to all of the commands in the group.</param>
	/// <param name="groups">The child command groups in the group.</param>
	/// <param name="commands">The child commands in the group.</param>
	/// <param name="implicitCommand">The implicit command for the group.</param>
	public VirtualCommandGroupInfo(
		string? name,
		ICommandGroupInfo? parent,
		IReadOnlyCollection<IFlagInfo> flags,
		IReadOnlyDictionary<string, ICommandGroupInfo> groups,
		IReadOnlyDictionary<string, ICommandInfo> commands,
		ICommandInfo? implicitCommand)
	{
		name?.ThrowIfEmptyOrWhitespace(nameof(name));

		Name = name;
		Parent = parent;
		Flags = flags;
		Groups = groups;
		Commands = commands;
		ImplicitCommand = implicitCommand;
	}
	#endregion
}
