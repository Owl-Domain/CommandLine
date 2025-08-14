namespace OwlDomain.CommandLine.Execution;

/// <summary>
/// 	Represents the context for the execution of a command.
/// </summary>
public interface ICommandExecutionContext
{
	#region Properties
	/// <summary>Whether the execution has been handled.</summary>
	bool Handled { get; }

	/// <summary>The result of executing the command.</summary>
	object? ResultValue { get; }

	/// <summary>A collection of the diagnostics reported during the execution.</summary>
	DiagnosticBag Diagnostics { get; }

	/// <summary>The engine that is handling the execution.</summary>
	ICommandEngine Engine { get; }

	/// <summary>The targetted group.</summary>
	ICommandGroupInfo GroupTarget { get; }

	/// <summary>The targetted command.</summary>
	ICommandInfo? CommandTarget { get; }

	/// <summary>The arguments that will be passed to the target upon execution.</summary>
	IReadOnlyDictionary<IArgumentInfo, object?> Arguments { get; }

	/// <summary>The flags that will be passed to the target upon execution.</summary>
	IReadOnlyDictionary<IFlagInfo, object?> Flags { get; }

	/// <summary>The result of validating the command that will be executed.</summary>
	ICommandValidatorResult ValidatorResult { get; }
	#endregion

	#region Methods
	/// <summary>Marks the execution operation as being handled.</summary>
	/// <param name="resultValue">The result of executing the command.</param>
	/// <exception cref="InvalidOperationException">Thrown if the execution has already been handled.</exception>
	void Handle(object? resultValue);
	#endregion
}
