namespace OwlDomain.CommandLine.Tests.Discovery;

[TestClass]
public sealed class NameExtractorTests
{
	#region Command name tests
	[DynamicData(nameof(GetTestNames), DynamicDataSourceType.Method)]
	[TestMethod]
	public void GetCommandName(string originalName, string expectedName)
	{
		// Arrange
		NameExtractor sut = new();

		// Act
		string result = sut.GetCommandName(originalName);

		// Assert
		Assert.That.AreEqual(result, expectedName);
	}

	[DynamicData(nameof(GetTestNames), DynamicDataSourceType.Method)]
	[TestMethod]
	public void GetArgumentName(string originalName, string expectedName)
	{
		// Arrange
		NameExtractor sut = new();

		// Act
		string result = sut.GetArgumentName(originalName);

		// Assert
		Assert.That.AreEqual(result, expectedName);
	}

	[DynamicData(nameof(GetTestNames), DynamicDataSourceType.Method)]
	[TestMethod]
	public void GetLongFlagName(string originalName, string expectedName)
	{
		// Arrange
		NameExtractor sut = new();

		// Act
		string result = sut.GetLongFlagName(originalName);

		// Assert
		Assert.That.AreEqual(result, expectedName);
	}

	[DynamicData(nameof(GetTestNames), DynamicDataSourceType.Method)]
	[TestMethod]
	public void GetShortFlagName(string originalName, string expectedName)
	{
		// Arrange
		char expectedResult = expectedName[0];
		NameExtractor sut = new();

		// Act
		char result = sut.GetShortFlagName(originalName);

		// Assert
		Assert.That.AreEqual(result, expectedResult);
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private static IEnumerable<object?[]> GetTestNames()
	{
		return [
			["Name", "name"],
			["NAME", "name"],
			["LongName", "long-name"],
			["VeryLONGName", "very-long-name"]
		];
	}
	#endregion
}
