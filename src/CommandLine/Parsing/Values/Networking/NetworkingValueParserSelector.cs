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

		return null;
	}
	#endregion
}
