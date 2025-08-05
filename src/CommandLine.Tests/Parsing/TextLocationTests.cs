namespace OwlDomain.CommandLine.Tests.Parsing;

[TestClass]
public sealed class TextLocationTests
{
	#region Constructor tests
	[DataRow(1, 2, DisplayName = "Start before end")]
	[DataRow(2, 2, DisplayName = "Start at end")]
	[TestMethod]
	public void Constructor_WithValidValues_SetsExpectedProperties(int startOffset, int endOffset)
	{
		// Arrange
		TextFragment fragment = new("test", 1);
		TextPoint expectedStart = new(fragment, startOffset);
		TextPoint expectedEnd = new(fragment, endOffset);

		// Act
		TextLocation Act() => new(expectedStart, expectedEnd);

		// Assert
		Assert.That
			.DoesNotThrowAnyException(Act, out TextLocation result)
			.AreEqual(result.Start, expectedStart)
			.AreEqual(result.End, expectedEnd);
	}

	[TestMethod]
	public void Constructor_StartAfterEnd_ThrowsArgumentOutOfRangeException()
	{
		// Arrange
		const string expectedParameter = "end";

		TextFragment fragment = new("test", 1);
		TextPoint start = new(fragment, 2);
		TextPoint expectedEnd = new(fragment, 1);

		// Act
		void Act() => _ = new TextLocation(start, expectedEnd);

		// Assert
		Assert.That
			.ThrowsExactException(Act, out ArgumentOutOfRangeException exception)
			.AreEqual(exception.ActualValue, expectedEnd)
			.AreEqual(exception.ParamName, expectedParameter);
	}
	#endregion
}
