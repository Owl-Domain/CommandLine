namespace OwlDomain.CommandLine.Commands;

/// <summary>
/// 	Represents information about a command.
/// </summary>
public interface ICommandInfo
{
	#region Properties
	/// <summary>The name associated with this command.</summary>
	/// <remarks>Implicit group commands do not have a name.</remarks>
	string? Name { get; }

	/// <summary>The group that the command belongs to.</summary>
	ICommandGroupInfo Group { get; }

	/// <summary>The flags that the command takes.</summary>
	IReadOnlyCollection<IFlagInfo> Flags { get; }

	/// <summary>The arguments that the command takes.</summary>
	IReadOnlyList<IArgumentInfo> Arguments { get; }

	/// <summary>The documentation for the command.</summary>
	IDocumentationInfo? Documentation { get; }

	/// <summary>Whether the command has a return value.</summary>
	bool HasResultValue { get; }
	#endregion
}
