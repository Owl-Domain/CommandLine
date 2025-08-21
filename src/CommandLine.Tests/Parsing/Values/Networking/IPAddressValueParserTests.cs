using System.Net;

namespace OwlDomain.CommandLine.Tests.Parsing.Values.Networking;

[TestClass]
public sealed class IPAddressValueParserTests
{
	#region Tests
	[DataRow(ParsingMode.Lazy)]
	[DataRow(ParsingMode.Greedy)]
	[TestMethod]
	public void Parse_WithIPv4_Successful(ParsingMode mode)
	{
		// Arrange
		const string input = "127.0.0.1";
		IPAddress expectedValue = IPAddress.Parse(input);

		IFlagValueParseContext context = Substitute.For<IFlagValueParseContext>();
		TextParser parser = new([input], mode);
		IPAddressValueParser sut = new();

		// Act
		IValueParseResult<IPAddress> parseResult = sut.Parse(context, parser);

		// Assert
		Assert.That
			.IsTrue(parseResult.Successful)
			.IsNull(parseResult.Error)
			.AreEqual(parseResult.Value, expectedValue);
	}

	[DataRow(ParsingMode.Lazy)]
	[DataRow(ParsingMode.Greedy)]
	[TestMethod]
	public void Parse_WithLongIPv6_Successful(ParsingMode mode)
	{
		// Arrange
		const string input = "0:0:0:0:0:0:0:1";
		IPAddress expectedValue = IPAddress.Parse(input);

		IFlagValueParseContext context = Substitute.For<IFlagValueParseContext>();
		TextParser parser = new([input], mode);
		IPAddressValueParser sut = new();

		// Act
		IValueParseResult<IPAddress> parseResult = sut.Parse(context, parser);

		// Assert
		Assert.That
			.IsTrue(parseResult.Successful)
			.IsNull(parseResult.Error)
			.AreEqual(parseResult.Value, expectedValue);
	}

	[DataRow(ParsingMode.Lazy)]
	[DataRow(ParsingMode.Greedy)]
	[TestMethod]
	public void Parse_WithShortIPv6_Successful(ParsingMode mode)
	{
		// Arrange
		const string input = "::1";
		IPAddress expectedValue = IPAddress.Parse(input);

		IFlagValueParseContext context = Substitute.For<IFlagValueParseContext>();
		TextParser parser = new([input], mode);
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

