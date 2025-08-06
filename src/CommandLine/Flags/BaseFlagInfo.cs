namespace OwlDomain.CommandLine.Flags;

/// <summary>
/// 	Represents the base implementation for a flag.
/// </summary>
/// <typeparam name="T">The type of the flag's value.</typeparam>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public abstract class BaseFlagInfo<T> : IFlagInfo<T>
{
	#region Properties
	/// <inheritdoc/>
	public FlagKind Kind { get; }

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
	/// <param name="kind">The type of the flag.</param>
	/// <param name="longName">The long name of the flag.</param>
	/// <param name="shortName">The short name of the flag.</param>
	/// <param name="isRequired">Whether the flag has to be set when executing the command.</param>
	/// <param name="defaultValue">The default value for the flag.</param>
	/// <param name="parser">The value parser selected for the flag.</param>
	protected BaseFlagInfo(FlagKind kind, string? longName, char? shortName, bool isRequired, T? defaultValue, IValueParser<T> parser)
	{
		kind.ThrowIfNotDefined(nameof(kind));
		longName?.ThrowIfEmptyOrWhitespace(nameof(longName));

		if (longName is null && shortName is null)
			Throw.New.ArgumentException(nameof(longName), "Either the long name or the short name of the flag must be specified at a minimum.");

		if (isRequired && defaultValue is not null)
			Throw.New.ArgumentException(nameof(defaultValue), "Default values are not allowed for required flags.");

		Kind = kind;
		LongName = longName;
		ShortName = shortName;
		IsRequired = isRequired;
		DefaultValue = defaultValue;
		Parser = parser;
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string longName = nameof(LongName);
		const string shortName = nameof(ShortName);
		const string valueTypeName = nameof(IFlagInfo.ValueType);
		string typeName = GetType().Name;

		if (typeName.Contains('`'))
			typeName = typeName[..typeName.IndexOf('`')];

		return $"{typeName} {{ {longName} = ({LongName}), {shortName} = ({ShortName}), {valueTypeName} = ({typeof(T)}) }}";
	}
	#endregion
}
