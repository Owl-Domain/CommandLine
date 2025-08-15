namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents information about a flag that is linked to a parameter.
/// </summary>
public interface IParameterFlagInfo : IFlagInfo, IHasAttributes
{
	#region Properties
	/// <summary>The parameter that represents the flag.</summary>
	ParameterInfo Parameter { get; }
	#endregion
}

/// <summary>
/// 	Represents information about a flag that is linked to a parameter.
/// </summary>
/// <typeparam name="T">The type of the flag's value.</typeparam>
public interface IParameterFlagInfo<out T> : IParameterFlagInfo, IFlagInfo<T>
{
}
