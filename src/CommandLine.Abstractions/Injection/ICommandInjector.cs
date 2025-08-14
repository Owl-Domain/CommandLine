namespace OwlDomain.CommandLine.Injection;

/// <summary>
/// 	Represents an injector for command values.
/// </summary>
public interface ICommandInjector
{
	#region Methods
	/// <summary>Checks whether a value can be injected into the given <paramref name="property"/>.</summary>
	/// <param name="property">The property to check.</param>
	/// <returns><see langword="true"/> if a value can be injected into the given <paramref name="property"/>, <see langword="false"/> otherwise.</returns>
	bool CanInject(PropertyInfo property);

	/// <summary>Checks whether a value can be injected into the given <paramref name="parameter"/>.</summary>
	/// <param name="parameter">The parameter to check.</param>
	/// <returns><see langword="true"/> if a value can be injected into the given <paramref name="parameter"/>, <see langword="false"/> otherwise.</returns>
	bool CanInject(ParameterInfo parameter);

	/// <summary>Gets the value to inject into the given <paramref name="property"/>.</summary>
	/// <param name="context">The context for the command that will be executed.</param>
	/// <param name="property">The property to get the value for.</param>
	/// <returns>The value to inject into the <paramref name="property"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if a value cannot be injected into the given <paramref name="property"/>.</exception>
	object? Inject(ICommandExecutionContext context, PropertyInfo property);

	/// <summary>Gets the value to inject into the given <paramref name="parameter"/>.</summary>
	/// <param name="context">The context for the command that will be executed.</param>
	/// <param name="parameter">The parameter to get the value for.</param>
	/// <returns>The value to inject into the <paramref name="parameter"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if a value cannot be injected into the given <paramref name="parameter"/>.</exception>
	object? Inject(ICommandExecutionContext context, ParameterInfo parameter);
	#endregion
}
