namespace OwlDomain.CommandLine.Groups;

/// <summary>
/// 	Represents information about a command group.
/// </summary>
public interface ICommandGroupInfo
{
	#region Properties
	/// <summary>The name associated with the command group.</summary>
	/// <remarks>Root command groups do not have a name.</remarks>
	string? Name { get; }

	/// <summary>The parent command group.</summary>
	/// <remarks>Root command groups do not have a parent.</remarks>
	ICommandGroupInfo? Parent { get; }

	/// <summary>The flags that can be passed to all of the commands in the group.</summary>
	IReadOnlyCollection<IFlagInfo> SharedFlags { get; }

	/// <summary>The child command groups in the group.</summary>
	IReadOnlyDictionary<string, ICommandGroupInfo> Groups { get; }

	/// <summary>The child commands in the group.</summary>
	IReadOnlyDictionary<string, ICommandInfo> Commands { get; }

	/// <summary>The implicit command for the group.</summary>
	ICommandInfo? ImplicitCommand { get; }
	#endregion
}
