namespace OwlDomain.CommandLine.Commands;

/// <summary>
/// 	Represents information about a command that is linked to a method.
/// </summary>
/// <param name="method">The method that the command is linked to.</param>
/// <param name="name">The name associated with the command.</param>
/// <param name="group">The group that the command belongs to.</param>
/// <param name="flags">The flags that the command takes.</param>
/// <param name="arguments">The arguments that the command takes.</param>
/// <param name="documentation">The documentation for the command.</param>
/// <param name="injectedParameters">The parameters that will be injected.</param>
/// <param name="injectedProperties">The properties on the <paramref name="method"/> container that will be injected.</param>
public sealed class MethodCommandInfo(
	MethodInfo method,
	string? name,
	ICommandGroupInfo group,
	IReadOnlyCollection<IFlagInfo> flags,
	IReadOnlyList<IArgumentInfo> arguments,
	IDocumentationInfo? documentation,
	IReadOnlyCollection<InjectedParameterInfo> injectedParameters,
	IReadOnlyCollection<InjectedPropertyInfo> injectedProperties)
	: BaseCommandInfo(name, group, flags, arguments, documentation), IMethodCommandInfo
{
	#region Properties
	/// <inheritdoc/>
	public MethodInfo Method { get; } = method;

	/// <inheritdoc/>
	public override bool HasResultValue => Method.ReturnType != typeof(void);

	/// <inheritdoc/>
	public IReadOnlyCollection<InjectedParameterInfo> InjectedParameters { get; } = injectedParameters;

	/// <inheritdoc/>
	public IReadOnlyCollection<InjectedPropertyInfo> InjectedProperties { get; } = injectedProperties;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public TAttribute? GetAttribute<TAttribute>()
		where TAttribute : Attribute
	{
		return Method.GetCustomAttribute<TAttribute>();
	}

	/// <inheritdoc/>
	public bool TryGetAttribute<TAttribute>([NotNullWhen(true)] out TAttribute? attribute)
		where TAttribute : Attribute
	{
		return Method.TryGetCustomAttribute(out attribute);
	}

	/// <inheritdoc/>
	public IEnumerable<TAttribute> GetAttributes<TAttribute>()
		where TAttribute : Attribute
	{
		return Method.GetCustomAttributes<TAttribute>();
	}

	/// <inheritdoc/>
	public bool TryGetAttributes<TAttribute>([NotNullWhen(true)] out IEnumerable<TAttribute>? attributes)
		where TAttribute : Attribute
	{
		return Method.TryGetCustomAttributes(out attributes);
	}
	#endregion
}
