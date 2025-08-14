using System.Net;

namespace OwlDomain.CommandLine.Tests.Parsing.Values.Networking;

[TestClass]
public sealed class IPAddressValueParserTests
{
	#region Tests
	[DataRow(true, DisplayName = "Lazy mode")]
	[DataRow(false, DisplayName = "Greedy mode")]
	[TestMethod]
	public void Parse_WithIPv4_Successful(bool isLazy)
	{
		// Arrange
		const string input = "127.0.0.1";
		IPAddress expectedValue = IPAddress.Parse(input);

		IFlagValueParseContext context = Substitute.For<IFlagValueParseContext>();
		TextParser parser = new([input], isLazy);
		IPAddressValueParser sut = new();

		// Act
		IValueParseResult<IPAddress> parseResult = sut.Parse(context, parser);

		// Assert
		Assert.That
			.IsTrue(parseResult.Successful)
			.IsNull(parseResult.Error)
			.AreEqual(parseResult.Value, expectedValue);
	}

	[DataRow(true, DisplayName = "Lazy mode")]
	[DataRow(false, DisplayName = "Greedy mode")]
	[TestMethod]
	public void Parse_WithLongIPv6_Successful(bool isLazy)
	{
		// Arrange
		const string input = "0:0:0:0:0:0:0:1";
		IPAddress expectedValue = IPAddress.Parse(input);

		IFlagValueParseContext context = Substitute.For<IFlagValueParseContext>();
		TextParser parser = new([input], isLazy);
		IPAddressValueParser sut = new();

		// Act
		IValueParseResult<IPAddress> parseResult = sut.Parse(context, parser);

		// Assert
		Assert.That
			.IsTrue(parseResult.Successful)
			.IsNull(parseResult.Error)
			.AreEqual(parseResult.Value, expectedValue);
	}

	[DataRow(true, DisplayName = "Lazy mode")]
	[DataRow(false, DisplayName = "Greedy mode")]
	[TestMethod]
	public void Parse_WithShortIPv6_Successful(bool isLazy)
	{
		// Arrange
		const string input = "::1";
		IPAddress expectedValue = IPAddress.Parse(input);

		IFlagValueParseContext context = Substitute.For<IFlagValueParseContext>();
		TextParser parser = new([input], isLazy);
		IPAddressValueParser sut = new();

		// Act
		IValueParseResult<IPAddress> parseResult = sut.Parse(context, parser);

		// Assert
		Assert.That
			.IsTrue(parseResult.Successful)
			.IsNull(parseResult.Error)
			.AreEqual(parseResult.Value, expectedValue);
	}
	#endregion
}

