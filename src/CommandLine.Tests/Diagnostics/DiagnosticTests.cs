namespace OwlDomain.CommandLine.Tests.Diagnostics;

[TestClass]
public sealed class DiagnosticTests
{
	#region Constructor tests
	[TestMethod]
	public void Constructor_WithInvalidSource_ThrowsArgumentOutOfRange()
	{
		// Arrange
		const DiagnosticSource expectedSource = (DiagnosticSource)byte.MaxValue;
		const string expectedParameterName = "source";

		// Act
		static void Act() => _ = new Diagnostic(expectedSource, default, string.Empty);

		// Assert
		Assert.That
			.ThrowsExactException(Act, out ArgumentOutOfRangeException exception)
			.AreEqual(exception.ActualValue, expectedSource)
			.AreEqual(exception.ParamName, expectedParameterName);
	}

	[TestMethod]
	public void Constructor_WithValidValues_SetsExpectedProperties()
	{
		// Arrange
		const DiagnosticSource expectedSource = DiagnosticSource.Execution;
		const string expectedMessage = "message";

		TextFragment fragment = new("text", 0);
		TextLocation expectedLocation = new(new(fragment, 0), new(fragment, 1));

		// Act
		Diagnostic Act() => new(expectedSource, expectedLocation, expectedMessage);

		// Assert
		Assert.That
			.DoesNotThrowAnyException(Act, out Diagnostic result)
			.AreEqual(result.Source, expectedSource)
			.AreEqual(result.Location, expectedLocation)
			.AreEqual(result.Message, expectedMessage);
	}
	#endregion
}
