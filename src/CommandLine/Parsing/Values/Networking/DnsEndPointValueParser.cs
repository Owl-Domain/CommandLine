using System.Net;

namespace OwlDomain.CommandLine.Parsing.Values.Networking;

/// <summary>
/// 	Represents a parser for the <see cref="DnsEndPoint"/> type.
/// </summary>
public sealed class DnsEndPointValueParser : BaseValueParser<DnsEndPoint>
{
	#region Methods
	/// <inheritdoc/>
	protected override DnsEndPoint? TryParse(IValueParseContext context, ITextParser parser, out string? error)
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

		ReadOnlySpan<char> hostSpan;
		ReadOnlySpan<char> portSpan;

		if (open is not -1 && close is not -1 && open != text.Length - 1 && close != 0)
		{
			hostSpan = text.AsSpan(open + 1, close - (open + 1));
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
			hostSpan = text.AsSpan(0, colon);
			portSpan = text.AsSpan(colon + 1);
		}
		else if (colon is -1)
		{
			hostSpan = text;
			parser.SkipTrivia();
			portSpan = parser.AdvanceUntilBreak();
		}
		else if (colon == text.Length - 1)
		{
			hostSpan = text.AsSpan(0, colon);
			parser.SkipTrivia();
			portSpan = parser.AdvanceUntilBreak();
		}
		else
		{
			error = $"Failed to parse '{text}' as an hostname & port pair.";
			return null;
		}

		if (hostSpan.Length >= 2 && hostSpan[0] is '[' && hostSpan[^1] is ']')
			hostSpan = hostSpan[1..^1];

		string host;
		if (hostSpan.Contains(':') && IPAddress.TryParse(hostSpan, out _))
			host = $"[{hostSpan.ToString()}]";
		else
			host = hostSpan.ToString();

		if (string.IsNullOrWhiteSpace(host))
		{
			error = $"Failed to parse '{hostSpan}' as an ip address.";
			return null;
		}

		if (Port.TryParse(portSpan, null, out Port port) is false)
		{
			error = $"Failed to parse '{portSpan}' as a valid port number.";
			return null;
		}

		error = default;
		return new(host, port);
	}
	#endregion
}
