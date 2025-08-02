namespace OwlDomain.CommandLine.Groups;

/// <summary>
/// 	Represents information about a command group.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class CommandGroupInfo : ICommandGroupInfo
{
	#region Properties
	/// <inheritdoc/>
	public string? Name { get; }

	/// <inheritdoc/>
	public ICommandGroupInfo? Parent { get; }

	/// <inheritdoc/>
	public IReadOnlyCollection<IFlagInfo> SharedFlags { get; }

	/// <inheritdoc/>
	public IReadOnlyDictionary<string, ICommandGroupInfo> Groups { get; }

	/// <inheritdoc/>
	public IReadOnlyDictionary<string, ICommandInfo> Commands { get; }

	/// <inheritdoc/>
	public ICommandInfo? ImplicitCommand { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="CommandGroupInfo"/>.</summary>
	/// <param name="name">The name associated with the command group.</param>
	/// <param name="parent">The parent command group.</param>
	/// <param name="flags">The flags that can be passed to all of the commands in the group.</param>
	/// <param name="groups">The child command groups in the group.</param>
	/// <param name="commands">The child commands in the group.</param>
	/// <param name="implicitCommand">The implicit command for the group.</param>
	public CommandGroupInfo(
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
		SharedFlags = flags;
		Groups = groups;
		Commands = commands;
		ImplicitCommand = implicitCommand;
	}
	#endregion

	#region Methods
	private string DebuggerDisplay() => $"Group {{ Name = ({Name}) }}";
	#endregion
}
