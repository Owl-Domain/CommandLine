namespace OwlDomain.CommandLine.Arguments;

/// <summary>
/// 	Represents information about an argument.
/// </summary>
public interface IArgumentInfo
{
	#region Properties
	/// <summary>The name of the argument.</summary>
	string Name { get; }

	/// <summary>The position of the argument.</summary>
	int Position { get; }

	/// <summary>The information about the argument's value.</summary>
	IValueInfo ValueInfo { get; }

	/// <summary>The information about the argument's default value.</summary>
	/// <remarks>If this value is <see langword="null"/> then the argument must be required.</remarks>
	IDefaultValueInfo? DefaultValueInfo { get; }

	/// <summary>The documentation for the argument.</summary>
	IDocumentationInfo? Documentation { get; }
	#endregion
}

/// <summary>
/// 	Represents information about an argument.
/// </summary>
/// <typeparam name="T">The type of the argument's value.</typeparam>
public interface IArgumentInfo<out T> : IArgumentInfo
{
	#region Properties
	/// <summary>The information about the flargument's value.</summary>
	new IValueInfo<T> ValueInfo { get; }
	IValueInfo IArgumentInfo.ValueInfo => ValueInfo;
	#endregion
}
