namespace OwlDomain.CommandLine.Commands;

/// <summary>
/// 	Represents information about a command that is linked to a method.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class MethodCommandInfo : IMethodCommandInfo
{
	#region Properties
	/// <inheritdoc/>
	public MethodInfo Method { get; }

	/// <inheritdoc/>
	public string? Name { get; }

	/// <inheritdoc/>
	public ICommandGroupInfo Group { get; }

	/// <inheritdoc/>
	public IReadOnlyCollection<IFlagInfo> Flags { get; }

	/// <inheritdoc/>
	public IReadOnlyList<IArgumentInfo> Arguments { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="MethodCommandInfo"/>.</summary>
	/// <param name="method">The method that the command is linked to.</param>
	/// <param name="name">The name associated with the command.</param>
	/// <param name="group">The group that the command belongs to.</param>
	/// <param name="flags">The flags that the command takes.</param>
	/// <param name="arguments">The arguments that the command takes.</param>
	public MethodCommandInfo(MethodInfo method, string? name, ICommandGroupInfo group, IReadOnlyCollection<IFlagInfo> flags, IReadOnlyList<IArgumentInfo> arguments)
	{
		name?.ThrowIfEmptyOrWhitespace(nameof(name));

		Method = method;
		Name = name;
		Group = group;
		Flags = flags;
		Arguments = arguments;
	}
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

	private string DebuggerDisplay() => $"Command {{ Name = ({Name}) }}";
	#endregion
}
