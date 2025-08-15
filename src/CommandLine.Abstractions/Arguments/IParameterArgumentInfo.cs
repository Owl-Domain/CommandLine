namespace OwlDomain.CommandLine.Arguments;

/// <summary>
/// 	Represents information about an argument that is linked to a parameter.
/// </summary>
public interface IParameterArgumentInfo : IArgumentInfo, IHasAttributes
{
	#region Properties
	/// <summary>The parameter that represents the argument.</summary>
	ParameterInfo Parameter { get; }
	#endregion
}

/// <summary>
/// 	Represents information about an argument that is linked to a parameter.
/// </summary>
/// <typeparam name="T">The type of the argument's value.</typeparam>
public interface IParameterArgumentInfo<out T> : IParameterArgumentInfo, IArgumentInfo<T>
{
}
