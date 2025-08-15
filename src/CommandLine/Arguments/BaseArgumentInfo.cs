namespace OwlDomain.CommandLine.Arguments;

/// <summary>
/// 	Represents the base implementation for an engine command's argument.
/// </summary>
/// <typeparam name="T">The type of the argument's value.</typeparam>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public abstract class BaseArgumentInfo<T> : IArgumentInfo<T>
{
	#region Properties
	/// <inheritdoc/>
	public string Name { get; }

	/// <inheritdoc/>
	public int Position { get; }

	/// <inheritdoc/>
	public IValueInfo<T> ValueInfo { get; }

	/// <inheritdoc/>
	public IDefaultValueInfo? DefaultValueInfo { get; }

	/// <inheritdoc/>
	public IDocumentationInfo? Documentation { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="BaseArgumentInfo{T}"/>.</summary>
	/// <param name="name">The name of the argument.</param>
	/// <param name="position">The position of the argument.</param>
	/// <param name="valueInfo">The information about the argument's value.</param>
	/// <param name="defaultValueInfo">The information aboue the argument's default value.</param>>
	/// <param name="documentation">The documentation for the argument.</param>
	protected BaseArgumentInfo(
		string name,
		int position,
		IValueInfo<T> valueInfo,
		IDefaultValueInfo? defaultValueInfo,
		IDocumentationInfo? documentation)
	{
		name.ThrowIfEmptyOrWhitespace(nameof(name));
		position.ThrowIfLessThan(0, nameof(position));

		Name = name;
		Position = position;
		ValueInfo = valueInfo;
		Documentation = documentation;
		DefaultValueInfo = defaultValueInfo;
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay() => $"Argument {{ Name = ({Name}), ValueType = ({typeof(T)}) }}";
	#endregion
}
