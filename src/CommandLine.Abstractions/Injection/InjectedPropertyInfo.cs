namespace OwlDomain.CommandLine.Injection;

/// <summary>
/// 	Represents information about the property that will be injected.
/// </summary>
/// <param name="property">The property that will be injected.</param>
/// <param name="injector">The injector that will be used to inject the value.</param>
[DebuggerDisplay($"{{{nameof(Property)}}}")]
public readonly struct InjectedPropertyInfo(PropertyInfo property, ICommandInjector injector)
{
	#region Properties
	/// <summary>The paramter that will be injected.</summary>
	public readonly PropertyInfo Property { get; } = property;

	/// <summary>The injector that will be used to inject the value.</summary>
	public readonly ICommandInjector Injector { get; } = injector;
	#endregion
}
