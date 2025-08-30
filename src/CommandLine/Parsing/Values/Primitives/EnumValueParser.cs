namespace OwlDomain.CommandLine.Parsing.Values.Primitives;

/// <summary>
/// 	Represents a value parser for an <see langword="enum"/> of the given type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the <see langword="enum"/> to parse.</typeparam>
/// <remarks>This parser will not handle flag enums, for that use the <see cref="FlagEnumValueParser{T}"/> instead.</remarks>
public sealed class EnumValueParser<T> : BaseValueParser<T>
	where T : struct, Enum
{
	#region Fields
	private static readonly string[] Names = Enum.GetNames<T>();
	private static readonly bool IgnoreCase = IsCaseSensitive() is false;
	#endregion

	#region Methods
	/// <inheritdoc/>
	protected override T TryParse(IValueParseContext context, ITextParser parser, out string? error)
	{
		string text = parser.AdvanceUntilBreak();

		// Note(Nightowl): Prevent the parsing automatically merging enum values;
		if (IsDefined(text) is false)
		{
			error = $"Couldn't parse '{text}' as a {typeof(T).Name} value.";
			return default;
		}

		if (Enum.TryParse(text, IgnoreCase, out T value))
		{
			error = default;
			return value;
		}

		error = $"Couldn't parse '{text}' as a {typeof(T).Name} value.";
		return default;
	}
	#endregion

	#region Helpers
	private static bool IsDefined(string text)
	{
		foreach (string name in Names)
		{
			if (text.Equals(name, StringComparison.OrdinalIgnoreCase))
				return true;
		}

		return false;
	}
	private static bool IsCaseSensitive()
	{
		HashSet<string> names = new(StringComparer.OrdinalIgnoreCase);

		foreach (string name in Enum.GetNames<T>())
		{
			if (names.Add(name) is false)
				return true;
		}

		return false;
	}
	#endregion
}
