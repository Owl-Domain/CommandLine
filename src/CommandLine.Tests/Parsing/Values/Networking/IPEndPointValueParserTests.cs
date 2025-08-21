using System.Net;

namespace OwlDomain.CommandLine.Tests.Parsing.Values.Networking;

[TestClass]
public sealed class IPEndPointValueParserTests
{
	#region Tests
	[DynamicData(nameof(VariousTests), DynamicDataSourceType.Method)]
	[TestMethod]
	public void Parse_Various_Successful(string[] fragments, string expectedAddress, int expectedPort, ParsingMode mode)
	{
		// Arrange
		IFlagValueParseContext context = Substitute.For<IFlagValueParseContext>();
		TextParser parser = new(fragments, mode);
		IPEndPointValueParser sut = new();

		// Act
		IValueParseResult<IPEndPoint> parseResult = sut.Parse(context, parser);

		// Assert
		CheckFailedResult(fragments, new(IPAddress.Parse(expectedAddress), expectedPort), mode, parseResult);
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private static void CheckFailedResult(string[] fragments, IPEndPoint expectedValue, ParsingMode mode, IValueParseResult<IPEndPoint> result)
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
			result.Value.Address.Equals(expectedValue.Address) &&
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
		string[] addresses =
		[
			"127.0.0.1",
			"[0:0:0:0:0:0:0:1]",
			"[::1]",
		];

		string[] separators = [":", ": ", " "];

		string[] ports =
		[
			"80",
			"http"
		];

		foreach (string address in addresses)
			foreach (string separator in separators)
				foreach (string port in ports)
				{
					string command = string.Concat(address, separator, port);

					Port p = Port.Parse(port, null);

					yield return
					[
						new string[] { command },
						address,
						p.Number,
						ParsingMode.Lazy
					];

					yield return
					[
						command.Split(' ', StringSplitOptions.RemoveEmptyEntries),
						address,
						p.Number,
						ParsingMode.Greedy
					];
				}
	}
	#endregion
}
