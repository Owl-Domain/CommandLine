namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents information about a flag that is linked to a property.
/// </summary>
public interface IPropertyFlagInfo : IFlagInfo
{
	#region Properties
	Type IFlagInfo.ValueType => Property.PropertyType;

	/// <summary>The property that represents the flag.</summary>
	PropertyInfo Property { get; }
	#endregion
}

/// <summary>
/// 	Represents information about a flag that is linked to a property.
/// </summary>
/// <typeparam name="T">The type of the flag's value.</typeparam>
public interface IPropertyFlagInfo<out T> : IPropertyFlagInfo, IFlagInfo<T> { }
