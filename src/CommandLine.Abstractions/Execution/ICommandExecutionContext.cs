namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents the context for the execution of a command.
/// </summary>
public interface ICommandExecutionContext
{
	#region Properties
	/// <summary>Whether the execution has been handled.</summary>
	/// <remarks>Settings this to <see langword="true"/> will short-circuit the execution of the command.</remarks>
	bool Handled { get; set; }

	/// <summary>A collection of the diagnostics reported during the execution.</summary>
	DiagnosticBag Diagnostics { get; }

	/// <summary>The engine that is handling the execution.</summary>
	ICommandEngine Engine { get; }

	/// <summary>The targetted group.</summary>
	/// <remarks>If this is <see langword="null"/> then <see cref="CommandTarget"/> will not be <see langword="null"/>.</remarks>
	ICommandGroupInfo? GroupTarget { get; }

	/// <summary>The targetted command.</summary>
	/// <remarks>If this is <see langword="null"/> then <see cref="GroupTarget"/> will not be <see langword="null"/>.</remarks>
	ICommandInfo? CommandTarget { get; }

	/// <summary>The arguments that will be passed to the target upon execution.</summary>
	IReadOnlyDictionary<IArgumentInfo, object?> Arguments { get; }

	/// <summary>The flags that will be passed to the target upon execution.</summary>
	IReadOnlyDictionary<IFlagInfo, object?> Flags { get; }
	#endregion
}
