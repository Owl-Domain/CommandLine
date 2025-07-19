
namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents information about a flag that isn't linked to anything.
/// </summary>
/// <typeparam name="T">The type of the flag's value.</typeparam>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public class VirtualFlagInfo<T> : IVirtualFlagInfo<T>
{
	#region Properties
	/// <inheritdoc/>
	public string? LongName { get; }

	/// <inheritdoc/>
	public char? ShortName { get; }

	/// <inheritdoc/>
	public bool IsRequired { get; }

	/// <inheritdoc/>
	public T? DefaultValue { get; }

	/// <inheritdoc/>
	public IValueParser<T> Parser { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="VirtualFlagInfo{T}"/>.</summary>
	/// <param name="longName">The long name of the flag.</param>
	/// <param name="shortName">The short name of the flag.</param>
	/// <param name="isRequired">Whether the flag has to be set when executing the command.</param>
	/// <param name="defaultValue">The default value for the flag.</param>
	/// <param name="parser">The value parser selected for the flag.</param>
	public VirtualFlagInfo(string? longName, char? shortName, bool isRequired, T? defaultValue, IValueParser<T> parser)
	{
		longName?.ThrowIfEmptyOrWhitespace(nameof(longName));

		if (longName is null && shortName is null)
			Throw.New.ArgumentException(nameof(longName), "Either the long name or the short name of the flag must be specified at a minimum.");

		if (isRequired && defaultValue is not null)
			Throw.New.ArgumentException(nameof(defaultValue), "Default values are not allowed for required flags.");

		LongName = longName;
		ShortName = shortName;
		IsRequired = isRequired;
		DefaultValue = defaultValue;
		Parser = parser;
	}
	#endregion

	#region Methods
	private string DebuggerDisplay() => $"Flag {{ LongName = ({LongName}), ShortName = ({ShortName}), ValueType = ({typeof(T)}) }}";
	#endregion
}
