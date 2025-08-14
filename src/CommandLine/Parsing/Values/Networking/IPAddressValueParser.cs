using System.Net;

namespace OwlDomain.CommandLine.Parsing.Values.Networking;

/// <summary>
/// 	Represents a parser for the <see cref="IPAddress"/> type.
/// </summary>
public sealed class IPAddressValueParser : BaseValueParser<IPAddress>
{
	#region Methods
	/// <inheritdoc/>
	protected override IPAddress? TryParse(IValueParseContext context, ITextParser parser, out string? error)
	{
		string text = parser.AdvanceUntilBreak();

		if (IPAddress.TryParse(text, out IPAddress? address))
		{
			error = default;
			return address;
		}

		error = $"Failed to parse '{text}' as an ip address.";
		return null;
	}
	#endregion
}
