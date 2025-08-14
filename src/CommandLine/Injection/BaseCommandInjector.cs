namespace OwlDomain.CommandLine.Injection;

/// <summary>
/// 	Represents the base implementation for a command injector.
/// </summary>
public abstract class BaseCommandInjector : ICommandInjector
{
	#region Methods
	/// <inheritdoc/>
	public virtual bool CanInject(PropertyInfo property) => CanInject(property.PropertyType);

	/// <inheritdoc/>
	public virtual bool CanInject(ParameterInfo parameter) => CanInject(parameter.ParameterType);

	/// <summary>Checks whether a value can be injected for the given <paramref name="type"/>.</summary>
	/// <param name="type">The type to check.</param>
	/// <returns><see langword="true"/> if a value can be injected for the given <paramref name="type"/>, <see langword="false"/> otherwise.</returns>
	protected virtual bool CanInject(Type type) => false;

	/// <inheritdoc/>
	public object? Inject(ICommandExecutionContext context, PropertyInfo property)
	{
		if (TryInject(context, property, out object? value))
		{
			if (property.PropertyType.IsAssignableFrom(value?.GetType()))
				return value;

			Throw.New.InvalidOperationException($"The {GetType()} returned a value ({value?.GetType()}) that wasn't valid for the given property ({property}).");
		}

		if (CanInject(property) is false)
			Throw.New.InvalidOperationException($"The ({GetType()}) type did not return a value even though it says that the given property ({property}) can be injected.");

		Throw.New.ArgumentException(nameof(property), $"Couldn't inject a value into the given property ({property}).");
		return default;
	}

	/// <inheritdoc/>
	public object? Inject(ICommandExecutionContext context, ParameterInfo parameter)
	{
		if (TryInject(context, parameter, out object? value))
		{
			if (parameter.ParameterType.IsAssignableFrom(value?.GetType()))
				return value;

			Throw.New.InvalidOperationException($"The {GetType()} type returned a value ({value?.GetType()}) that wasn't valid for the given parameter ({parameter}).");
		}

		if (CanInject(parameter) is false)
			Throw.New.InvalidOperationException($"The ({GetType()}) type did not return a value even though it says that the given parameter ({parameter}) can be injected.");

		Throw.New.ArgumentException(nameof(parameter), $"Couldn't inject a value into the given parameter ({parameter}).");
		return default;
	}

	/// <summary>Tries to get the value to inject into the given <paramref name="property"/>.</summary>
	/// <param name="context">The context for the command that will be executed.</param>
	/// <param name="property">The property to get the value for.</param>
	/// <param name="value">The value to inject into the given <paramref name="property"/>..</param>
	/// <returns><see langword="true"/> if a value can be injected into the given <paramref name="property"/>, <see langword="false"/> otherwise.</returns>
	protected virtual bool TryInject(ICommandExecutionContext context, PropertyInfo property, out object? value)
	{
		return TryInject(context, property.PropertyType, out value);
	}

	/// <summary>Tries to get the value to inject into the given <paramref name="parameter"/>.</summary>
	/// <param name="context">The context for the command that will be executed.</param>
	/// <param name="parameter">The parameter to get the value for.</param>
	/// <param name="value">The value to inject into the given <paramref name="parameter"/>..</param>
	/// <returns><see langword="true"/> if a value can be injected into the given <paramref name="parameter"/>, <see langword="false"/> otherwise.</returns>
	protected virtual bool TryInject(ICommandExecutionContext context, ParameterInfo parameter, out object? value)
	{
		return TryInject(context, parameter.ParameterType, out value);
	}

	/// <summary>Tries to get the value to inject for the given <paramref name="type"/>.</summary>
	/// <param name="context">The context for the command that will be executed.</param>
	/// <param name="type">The type to get the value for.</param>
	/// <param name="value">The value to inject for the given <paramref name="type"/>.</param>
	/// <returns><see langword="true"/> if a value can be injected for the given <paramref name="type"/>, <see langword="false"/> otherwise.</returns>
	protected virtual bool TryInject(ICommandExecutionContext context, Type type, out object? value)
	{
		value = default;
		return false;
	}
	#endregion
}
