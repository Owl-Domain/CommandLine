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
	public IValueInfo<T> ValueInfo { get; }

	/// <inheritdoc/>
	public IDefaultValueInfo? DefaultValueInfo { get; }

	/// <inheritdoc/>
	public IDocumentationInfo? Documentation { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="VirtualFlagInfo{T}"/>.</summary>
	/// <param name="kind">The type of the flag.</param>
	/// <param name="longName">The long name of the flag.</param>
	/// <param name="shortName">The short name of the flag.</param>
	/// <param name="valueInfo">The information about the flag's value.</param>
	/// <param name="defaultValueInfo">The information aboue the flag's default value.</param>
	/// <param name="documentation">The documentation for the flag.</param>
	protected BaseFlagInfo(
		FlagKind kind,
		string? longName,
		char? shortName,
		IValueInfo<T> valueInfo,
		IDefaultValueInfo? defaultValueInfo,
		IDocumentationInfo? documentation)
	{
		kind.ThrowIfNotDefined(nameof(kind));
		longName?.ThrowIfEmptyOrWhitespace(nameof(longName));

		if (longName is null && shortName is null)
			Throw.New.ArgumentException(nameof(longName), "Either the long name or the short name of the flag must be specified at a minimum.");

		Kind = kind;
		LongName = longName;
		ShortName = shortName;
		ValueInfo = valueInfo;
		DefaultValueInfo = defaultValueInfo;
		Documentation = documentation;
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string longName = nameof(LongName);
		const string shortName = nameof(ShortName);
		const string valueTypeName = $"{nameof(ValueInfo)}.{nameof(IValueInfo.Type)}";
		string typeName = GetType().Name;

		if (typeName.Contains('`'))
			typeName = typeName[..typeName.IndexOf('`')];

		return $"{typeName} {{ {longName} = ({LongName}), {shortName} = ({ShortName}), {valueTypeName} = ({typeof(T)}) }}";
	}
	#endregion
}
