namespace OwlDomain.CommandLine.Commands;

/// <summary>
/// 	Represents the base implementation for an engine command.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public abstract class BaseCommandInfo : ICommandInfo
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
	public abstract bool HasResultValue { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="BaseCommandInfo"/>.</summary>
	/// <param name="name">The name associated with the command.</param>
	/// <param name="group">The group that the command belongs to.</param>
	/// <param name="flags">The flags that the command takes.</param>
	/// <param name="arguments">The arguments that the command takes.</param>
	/// <param name="documentation">The documentation for the command.</param>
	protected BaseCommandInfo(
		string? name,
		ICommandGroupInfo group,
		IReadOnlyCollection<IFlagInfo> flags,
		IReadOnlyList<IArgumentInfo> arguments,
		IDocumentationInfo? documentation)
	{
		name?.ThrowIfEmptyOrWhitespace(nameof(name));

		Name = name;
		Group = group;
		Flags = flags;
		Arguments = arguments;
		Documentation = documentation;
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay() => $"Command {{ Name = ({Name}) }}";
	#endregion
}
