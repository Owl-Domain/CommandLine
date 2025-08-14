using System.Net;

namespace OwlDomain.CommandLine.Parsing.Values.Networking;

/// <summary>
/// 	Represents a parser for the <see cref="IPEndPoint"/> type.
/// </summary>
public sealed class IPEndPointValueParser : BaseValueParser<IPEndPoint>
{
	#region Methods
	/// <inheritdoc/>
	protected override IPEndPoint? TryParse(IValueParseContext context, ITextParser parser, out string? error)
	{
		string text = parser.AdvanceUntilBreak();

		int colon = text.LastIndexOf(':');
		int open = text.IndexOf('[');
		int close = text.LastIndexOf(']');

		if (open is not -1 ^ close is not -1)
		{
			error = $"Failed to parse '{text}' as an ip & port pair.";
			return null;
		}

		if (open is not -1 && close is not -1)
		{
			if (colon > close && IPEndPoint.TryParse(text, out IPEndPoint? result))
			{
				error = default;
				return result;
			}
		}
		else if (colon is not -1 && IPEndPoint.TryParse(text, out IPEndPoint? result))
		{
			error = default;
			return result;
		}

		ReadOnlySpan<char> addressSpan;
		ReadOnlySpan<char> portSpan;

		if (open is not -1 && close is not -1 && open != text.Length - 1 && close != 0)
		{
			addressSpan = text.AsSpan(open + 1, close - (open + 1));
			if (colon > close && colon != text.Length - 1)
				portSpan = text.AsSpan(colon + 1);
			else
			{
				parser.SkipTrivia();
				portSpan = parser.AdvanceUntilBreak();
			}
		}
		else if (colon > 0 && colon != text.Length - 1)
		{
			addressSpan = text.AsSpan(0, colon);
			portSpan = text.AsSpan(colon + 1);
		}
		else if (colon is -1)
		{
			addressSpan = text;
			parser.SkipTrivia();
			portSpan = parser.AdvanceUntilBreak();
		}
		else if (colon == text.Length - 1)
		{
			addressSpan = text.AsSpan(0, colon);
			parser.SkipTrivia();
			portSpan = parser.AdvanceUntilBreak();
		}
		else
		{
			error = $"Failed to parse '{text}' as an ip & port pair.";
			return null;
		}

		if (IPAddress.TryParse(addressSpan, out IPAddress? address) is false)
		{
			error = $"Failed to parse '{addressSpan}' as an ip address.";
			return null;
		}

		if (Port.TryParse(portSpan, null, out Port port) is false)
		{
			error = $"Failed to parse '{portSpan}' as a valid port number.";
			return null;
		}

		error = default;
		return new(address, port);
	}
	#endregion
}
