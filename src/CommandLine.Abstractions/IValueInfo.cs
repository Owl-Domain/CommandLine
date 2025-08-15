namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents information about the value of a flag/argument.
/// </summary>
public interface IValueInfo
{
	#region Properties
	/// <summary>The type of the value.</summary>
	Type Type { get; }

	/// <summary>Whether the value is required.</summary>
	/// <remarks>If a value is not required then a default value must be available.</remarks>
	bool IsRequired { get; }

	/// <summary>whether <see langword="null"/> values are allowed.</summary>
	bool IsNullable { get; }

	/// <summary>The parser selected for the value.</summary>
	IValueParser Parser { get; }
	#endregion
}

/// <summary>
/// 	Represents information about the value of a flag/argument.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public interface IValueInfo<out T> : IValueInfo
{
	#region Properties
	Type IValueInfo.Type => typeof(T);

	/// <summary>The parser selected for the value.</summary>
	new IValueParser<T> Parser { get; }
	IValueParser IValueInfo.Parser => Parser;
	#endregion
}
