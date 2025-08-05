namespace OwlDomain.CommandLine.Tests.Parsing;

[TestClass]
public sealed class TextTokenTests
{
	#region Constructor tests
	[TestMethod]
	public void Constructor_WithValidValues_SetsExpectedProperties()
	{
		// Arrange
		TextTokenKind expectedKind = TextTokenKind.CommandName;
		object? expectedValue = "test";

		TextFragment fragment = new("test", 1);
		TextLocation expectedLocation = new(new(fragment, 1), new(fragment, 2));

		// Act
		TextToken Act() => new(expectedKind, expectedLocation, expectedValue);

		// Assert
		Assert.That
			.DoesNotThrowAnyException(Act, out TextToken result)
			.AreEqual(result.Kind, expectedKind)
			.AreEqual(result.Location, expectedLocation)
			.AreEqual(result.Value, expectedValue);
	}

	[TestMethod]
	public void Constructor_WithInvalidKind_ThrowsArgumentOutOfRangeException()
	{
		// Arrange
		TextFragment fragment = new("test", 1);
		TextLocation location = new(new(fragment, 1), new(fragment, 2));

		TextTokenKind expectedKind = (TextTokenKind)byte.MaxValue;
		const string expectedParameter = "kind";

		// Act
		void Act() => _ = new TextToken(expectedKind, location);

		// Assert
		Assert.That
			.ThrowsExactException(Act, out ArgumentOutOfRangeException exception)
			.AreEqual(exception.ActualValue, expectedKind)
			.AreEqual(exception.ParamName, expectedParameter);
	}
	#endregion
}
