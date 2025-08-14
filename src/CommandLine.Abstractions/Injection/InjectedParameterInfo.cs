namespace OwlDomain.CommandLine.Injection;

/// <summary>
/// 	Represents information about the parameter that will be injected.
/// </summary>
/// <param name="parameter">The parameter that will be injected.</param>
/// <param name="injector">The injector that will be used to inject the value.</param>
[DebuggerDisplay($"{{{nameof(Parameter)}}}")]
public readonly struct InjectedParameterInfo(ParameterInfo parameter, ICommandInjector injector)
{
	#region Properties
	/// <summary>The paramter that will be injected.</summary>
	public readonly ParameterInfo Parameter { get; } = parameter;

	/// <summary>The injector that will be used to inject the value.</summary>
	public readonly ICommandInjector Injector { get; } = injector;
	#endregion
}
