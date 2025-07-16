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

	/// <summary>The type of the argument's value.</summary>
	Type ValueType { get; }

	/// <summary>Whether the argument has to be set when executing a command.</summary>
	bool IsRequired { get; }

	/// <summary>The default value for the argument.</summary>
	object? DefaultValue { get; }
	#endregion
}

/// <summary>
/// 	Represents information about an argument.
/// </summary>
/// <typeparam name="T">The type of the argument's value.</typeparam>
public interface IArgumentInfo<out T> : IArgumentInfo
{
	#region Properties
	Type IArgumentInfo.ValueType => typeof(T);

	/// <summary>The default value for the argument.</summary>
	new T? DefaultValue { get; }
	object? IArgumentInfo.DefaultValue => DefaultValue;
	#endregion
}