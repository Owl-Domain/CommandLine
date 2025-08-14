using System.Net;

namespace OwlDomain.CommandLine.Parsing.Values.Networking;

/// <summary>
/// 	Represents the value parser selector for networking types.
/// </summary>
public sealed class NetworkingValueParserSelector : BaseValueParserSelector
{
	#region Methods
	/// <inheritdoc/>
	protected override IValueParser? TrySelect(IRootValueParserSelector rootSelector, Type type)
	{
		if (type == typeof(IPAddress))
			return new IPAddressValueParser();

		if (type == typeof(IPEndPoint))
			return new IPEndPointValueParser();

		if (type == typeof(DnsEndPoint))
			return new DnsEndPointValueParser();

		if (type == typeof(Port))
			return new PortValueParser();

		return null;
	}
	#endregion
}
