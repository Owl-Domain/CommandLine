namespace OwlDomain.CommandLine.Tests.Discovery;

[TestClass]
public sealed class NameExtractorTests
{
	#region Command name tests
	[DynamicData(nameof(GetNamesToTest), DynamicDataSourceType.Method)]
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

	[DynamicData(nameof(GetNamesToTest), DynamicDataSourceType.Method)]
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

	[DynamicData(nameof(GetNamesToTest), DynamicDataSourceType.Method)]
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

	[DynamicData(nameof(GetNamesToTest), DynamicDataSourceType.Method)]
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
	private static IEnumerable<object?[]> GetNamesToTest()
	{
		string[] characters = ["", "-", "_", "----", "____"];

		string[] prefixes = characters;
		string[] suffixes = characters;
		string[] separators = characters;

		foreach (string prefix in prefixes)
		{
			foreach (string suffix in suffixes)
			{
				yield return [prefix + "name" + suffix, "name"];
				yield return [prefix + "Name" + suffix, "name"];
				yield return [prefix + "NAME" + suffix, "name"];

				foreach (string separator in separators)
				{
					if (separator is not "")
					{
						yield return [prefix + "long" + separator + "name" + suffix, "long-name"];
						yield return [prefix + "LONG" + separator + "NAME" + suffix, "long-name"];
					}

					yield return [prefix + "long" + separator + "Name" + suffix, "long-name"];
					yield return [prefix + "Long" + separator + "Name" + suffix, "long-name"];
					yield return [prefix + "LONG" + separator + "Name" + suffix, "long-name"];
					yield return [prefix + "Long" + separator + "NAME" + suffix, "long-name"];
				}
			}
		}
	}
	#endregion
}
