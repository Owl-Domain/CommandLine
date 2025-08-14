using System.Net;

namespace OwlDomain.CommandLine.Tests.Parsing.Values.Networking;

[TestClass]
public sealed class DnsEndPointValueParserTests
{
	#region Tests
	[DynamicData(nameof(VariousTests), DynamicDataSourceType.Method)]
	[TestMethod]
	public void Parse_Various_Successful(string[] fragments, string expectedHost, int expectedPort, bool isLazy)
	{
		// Arrange
		IFlagValueParseContext context = Substitute.For<IFlagValueParseContext>();
		TextParser parser = new(fragments, isLazy);
		DnsEndPointValueParser sut = new();

		// Act
		IValueParseResult<DnsEndPoint> parseResult = sut.Parse(context, parser);

		// Assert
		CheckFailedResult(fragments, new(expectedHost, expectedPort), isLazy, parseResult);
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private static void CheckFailedResult(string[] fragments, DnsEndPoint expectedValue, bool isLazy, IValueParseResult<DnsEndPoint> result)
	{
		if (result.Successful is false)
		{
			string message = isLazy ? $"Lazy parsing failed for the command: {fragments[0]}" : $"Greedy parsing failed for the command: {string.Join("|", fragments)}";
			message += "\n\nDiagnostics:";
			if (result.Error is not null)
				message += $"\n- [{result.Location}]: {result.Error}";

			Assert.That.Fail(message + "\n");
		}

		TextToken[] tokens = [.. result.EnumerateTokens()];
		TextTokenKind[] resultTokens = [.. tokens.Select(t => t.Kind)];
		if (resultTokens.Length is not 1)
		{
			string message = isLazy ? $"Lazy parsing failed for the command: {fragments[0]}" : $"Greedy parsing failed for the command: {string.Join("|", fragments)}";
			message += $"\n\nExpected tokens:\n{TextTokenKind.Value}";
			message += $"\n\nResult tokens:\n{string.Join(' ', resultTokens)}";

			Assert.That.Fail(message + "\n");
		}

		Debug.Assert(result.Value is not null);

		bool areEqual =
			result.Value.Host == expectedValue.Host &&
			result.Value.Port == expectedValue.Port;

		if (areEqual is false)
		{
			string message = isLazy ? $"Lazy parsing failed for the command: {fragments[0]}" : $"Greedy parsing failed for the command: {string.Join("|", fragments)}";
			message += $"\nExpected value: {expectedValue}";
			message += $"\nActual value: {result.Value}";

			Assert.That.Fail(message + "\n");
		}
	}

	[ExcludeFromCodeCoverage]
	private static IEnumerable<object?[]> VariousTests()
	{
		string[] hosts =
		[
			"127.0.0.1",
			"[0:0:0:0:0:0:0:1]",
			"[::1]",
			"localhost",
			"example.com"
		];

		string[] separators = [":", ": ", " "];

		string[] ports =
		[
			"80",
			"http"
		];

		foreach (string host in hosts)
			foreach (string separator in separators)
				foreach (string port in ports)
				{
					string command = string.Concat(host, separator, port);

					Port p = Port.Parse(port, null);

					yield return
					[
						new string[] { command },
						host,
						p.Number,
						true
					];

					yield return
					[
						command.Split(' ', StringSplitOptions.RemoveEmptyEntries),
						host,
						p.Number,
						false
					];
				}
	}
	#endregion
}
