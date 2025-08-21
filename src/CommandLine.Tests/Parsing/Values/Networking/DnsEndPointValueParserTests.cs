using System.Net;

namespace OwlDomain.CommandLine.Tests.Parsing.Values.Networking;

[TestClass]
public sealed class DnsEndPointValueParserTests
{
	#region Tests
	[DynamicData(nameof(VariousTests), DynamicDataSourceType.Method)]
	[TestMethod]
	public void Parse_Various_Successful(string[] fragments, string expectedHost, int expectedPort, ParsingMode mode)
	{
		// Arrange
		IFlagValueParseContext context = Substitute.For<IFlagValueParseContext>();
		TextParser parser = new(fragments, mode);
		DnsEndPointValueParser sut = new();

		// Act
		IValueParseResult<DnsEndPoint> parseResult = sut.Parse(context, parser);

		// Assert
		CheckFailedResult(fragments, new(expectedHost, expectedPort), mode, parseResult);
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private static void CheckFailedResult(string[] fragments, DnsEndPoint expectedValue, ParsingMode mode, IValueParseResult<DnsEndPoint> result)
	{
		if (result.Successful is false)
		{
			string message = mode switch
			{
				ParsingMode.Lazy => $"Lazy parsing failed for the command: {fragments[0]}",
				ParsingMode.Greedy => $"Greedy parsing failed for the command: {string.Join("|", fragments)}",

				_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, $"Unknown parsing mode.")
			};

			message += "\n\nDiagnostics:";

			if (result.Error is not null)
				message += $"\n- [{result.Location}]: {result.Error}";

			Assert.That.Fail(message + "\n");
		}

		TextToken[] tokens = [.. result.EnumerateTokens()];
		TextTokenKind[] resultTokens = [.. tokens.Select(t => t.Kind)];
		if (resultTokens.Length is not 1)
		{
			string message = mode switch
			{
				ParsingMode.Lazy => $"Lazy parsing failed for the command: {fragments[0]}",
				ParsingMode.Greedy => $"Greedy parsing failed for the command: {string.Join("|", fragments)}",

				_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, $"Unknown parsing mode.")
			};

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
			string message = mode switch
			{
				ParsingMode.Lazy => $"Lazy parsing failed for the command: {fragments[0]}",
				ParsingMode.Greedy => $"Greedy parsing failed for the command: {string.Join("|", fragments)}",

				_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, $"Unknown parsing mode.")
			};

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
						ParsingMode.Lazy
					];

					yield return
					[
						command.Split(' ', StringSplitOptions.RemoveEmptyEntries),
						host,
						p.Number,
						ParsingMode.Greedy
					];
				}
	}
	#endregion
}
