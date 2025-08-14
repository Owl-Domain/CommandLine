namespace OwlDomain.CommandLine.SpecialTypes;

/// <summary>
/// 	Represents a networking port.
/// </summary>
/// <param name="number">The port number.</param>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public readonly struct Port(ushort number) :
#if NET7_0_OR_GREATER
	IParsable<Port>,
	ISpanParsable<Port>,
	IEqualityOperators<Port, Port, bool>,
	IComparisonOperators<Port, Port, bool>,
#endif
	IEquatable<Port>,
	IComparable<Port>
{
	#region Nested types
	private readonly struct Pair(ushort number, string name)
	{
		#region Fields
		public readonly ushort Number = number;
		public readonly string Name = name;
		#endregion
	}
	#endregion

	#region Fields
	private static readonly Pair[] KnownPairs =
	[
		new(7, "echo"),
		new(17, "qotd"),
		new(22, "ssh"),
		new(23, "telnet"),
		new(25, "smtp"),
		new(37, "time"),
		new(43, "whois"),
		new(53, "dns"),
		new(70, "gopher"),
		new(80, "http"),
		new(123, "ntp"),
		new(194, "irc"),
		new(220, "imap"),
		new(443, "https"),
		new(530, "rpc"),
	];
	#endregion

	#region Properties
	/// <summary>The number of the port.</summary>
	public readonly ushort Number { get; } = number;

	/// <summary>The name of the port.</summary>
	public readonly string? Name => TryGetName(Number, out string? name) ? name : null;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public int CompareTo(Port other) => Number.CompareTo(other.Number);

	/// <inheritdoc/>
	public bool Equals(Port other) => Number.Equals(other.Number);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is Port other)
			return Equals(other);

		return false;
	}

	/// <inheritdoc/>
	public override int GetHashCode() => Number.GetHashCode();

	/// <inheritdoc/>
	public override string ToString() => Number.ToString();
	#endregion

	#region Functions
	/// <summary>Parses the given <paramref name="text"/> into a <see cref="Port"/> value.</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="provider">An object that provides culture-specific formatting about <paramref name="text"/>.</param>
	/// <returns>The parsed port value.</returns>
	/// <exception cref="FormatException">Thrown if the given <paramref name="text"/> <see langword="string"/> was not in the correct format.</exception>
	public static Port Parse(ReadOnlySpan<char> text, IFormatProvider? provider)
	{
		if (TryParse(text, provider, out Port result))
			return result;

		throw new FormatException($"Couldn't parse '{text.ToString()}' as a valid port number.");
	}

	/// <summary>Parses the given <paramref name="text"/> into a <see cref="Port"/> value.</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="provider">An object that provides culture-specific formatting about <paramref name="text"/>.</param>
	/// <returns>The parsed port value.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the given <paramref name="text"/> is <see langword="null"/>.</exception>
	/// <exception cref="FormatException">Thrown if the given <paramref name="text"/> <see langword="string"/> was not in the correct format.</exception>
	public static Port Parse(string text, IFormatProvider? provider)
	{
		if (TryParse(text.AsSpan(), provider, out Port result))
			return result;

		throw new FormatException($"Couldn't parse '{text}' as a valid port number.");
	}

	/// <summary>Tries to parse the given <paramref name="text"/> into a <see cref="Port"/> value.</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="provider">An object that provides culture-specific formatting about <paramref name="text"/>.</param>
	/// <param name="result">The parsed port value.</param>
	/// <returns><see langword="true"/> if the given <paramref name="text"/> was parsed correctly, <see langword="false"/> otherwise.</returns>
	public static bool TryParse(ReadOnlySpan<char> text, IFormatProvider? provider, [MaybeNullWhen(false)] out Port result)
	{
		if (ushort.TryParse(text, out ushort number))
		{
			result = number;
			return true;
		}

		foreach (Pair pair in KnownPairs)
		{
			if (text.Equals(pair.Name, StringComparison.OrdinalIgnoreCase))
			{
				result = pair.Number;
				return true;
			}
		}

		result = default;
		return false;
	}

	/// <summary>Tries to parse the given <paramref name="text"/> into a <see cref="Port"/> value.</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="provider">An object that provides culture-specific formatting about <paramref name="text"/>.</param>
	/// <param name="result">The parsed port value.</param>
	/// <returns><see langword="true"/> if the given <paramref name="text"/> was parsed correctly, <see langword="false"/> otherwise.</returns>
	public static bool TryParse([NotNullWhen(true)] string? text, IFormatProvider? provider, [MaybeNullWhen(false)] out Port result)
	{
		if (text is null)
		{
			result = default;
			return false;
		}

		return TryParse(text.AsSpan(), provider, out result);
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string typeName = nameof(Port);
		const string numberName = nameof(Number);
		const string nameName = nameof(Name);
		string? name = Name;

		if (name is not null)
			return $"{typeName} {{ {numberName} = ({Number:n0}), {nameName} = ({name}) }}";

		return $"{typeName} {{ {numberName} = ({Number:n0}) }}";
	}
	private static bool TryGetName(ushort port, [NotNullWhen(true)] out string? name)
	{
		foreach (Pair pair in KnownPairs)
		{
			if (pair.Number == port)
			{
				name = pair.Name;
				return true;
			}
		}

		name = default;
		return false;
	}
	#endregion

	#region Operators
	/// <summary>Compares two values to determine equality.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator ==(Port left, Port right) => left.Number == right.Number;

	/// <summary>Compares two values to determine inequality.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator !=(Port left, Port right) => left.Number != right.Number;

	/// <summary>Compares two values to determine which is greater.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator >(Port left, Port right) => left.Number > right.Number;

	/// <summary>Compares two values to determine which is greater or equal.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator >=(Port left, Port right) => left.Number >= right.Number;

	/// <summary>Compares two values to determine which is lesser.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator <(Port left, Port right) => left.Number < right.Number;

	/// <summary>Compares two values to determine which is lesser or equal.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator <=(Port left, Port right) => left.Number <= right.Number;
	#endregion

	#region Implicit operator
	/// <summary>Implicitly converts the given port <paramref name="number"/> into a <see cref="Port"/> value.</summary>
	/// <param name="number">The number to convert.</param>
	public static implicit operator Port(ushort number) => new(number);

	/// <summary>Implicitly converts the given <paramref name="port"/> into it's port number.</summary>
	/// <param name="port">The port to convert.</param>
	public static implicit operator ushort(Port port) => port.Number;
	#endregion
}
