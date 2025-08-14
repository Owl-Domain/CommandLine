namespace OwlDomain.CommandLine.Commands;

/// <summary>
/// 	Represents information about a command that is linked to a method.
/// </summary>
public interface IMethodCommandInfo : ICommandInfo, IHasAttributes
{
	#region Properties
	/// <summary>The method that the command is linked to.</summary>
	MethodInfo Method { get; }

	/// <summary>The parameters that will be injected.</summary>
	IReadOnlyCollection<InjectedParameterInfo> InjectedParameters { get; }

	/// <summary>The properties on the method container that will be injected.</summary>
	IReadOnlyCollection<InjectedPropertyInfo> InjectedProperties { get; }
	#endregion
}
