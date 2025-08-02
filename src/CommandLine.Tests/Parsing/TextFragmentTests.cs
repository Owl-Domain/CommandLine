namespace OwlDomain.CommandLine.Tests.Parsing;

[TestClass]
public sealed class TextFragmentTests
{
	#region Constructor tests
	[TestMethod]
	public void Constructor_Default_HasEmptyText()
	{
		// Arrange
		const string expectedText = "";
		const int expectedIndex = 0;
		const int expectedLength = 0;

		// Act
		static TextFragment Act() => default;

		// Assert
		Assert.That
			.DoesNotThrowAnyException(Act, out TextFragment sut)
			.AreEqual(sut.Text, expectedText)
			.AreEqual(sut.Index, expectedIndex)
			.AreEqual(sut.Length, expectedLength);
	}

	[TestMethod]
	public void Constructor_Parameterless_HasEmptyText()
	{
		// Arrange
		const string expectedText = "";
		const int expectedIndex = 0;
		const int expectedLength = 0;

		// Act
		static TextFragment Act() => new();

		// Assert
		Assert.That
			.DoesNotThrowAnyException(Act, out TextFragment sut)
			.AreEqual(sut.Text, expectedText)
			.AreEqual(sut.Index, expectedIndex)
			.AreEqual(sut.Length, expectedLength);
	}

	[DataRow("", DisplayName = "Empty text")]
	[DataRow("test", DisplayName = "Regular text")]
	[DataRow("test text here", DisplayName = "Text with spaces")]
	[TestMethod]
	public void Constructor_WithText_Successful(string text)
	{
		// Arrange
		const int expectedIndex = 1;
		int expectedLength = text.Length;

		// Act
		TextFragment Act() => new(text, expectedIndex);

		// Assert
		Assert.That
			.DoesNotThrowAnyException(Act, out TextFragment result)
			.AreEqual(result.Text, text)
			.AreEqual(result.Index, expectedIndex)
			.AreEqual(result.Length, expectedLength);
	}

	[DataRow(0, DisplayName = "Index: 0")]
	[DataRow(1, DisplayName = "Index: 1")]
	[DataRow(int.MaxValue, DisplayName = "Index: Max")]
	[TestMethod]
	public void Constructor_WithValidIndex_Successful(int index)
	{
		// Arrange
		const string expectedText = "test";
		int expectedLength = expectedText.Length;

		// Act
		TextFragment Act() => new(expectedText, index);

		// Assert
		Assert.That
			.DoesNotThrowAnyException(Act, out TextFragment sut)
			.AreEqual(sut.Index, index)
			.AreEqual(sut.Text, expectedText)
			.AreEqual(sut.Length, expectedLength);
	}

	[TestMethod]
	public void Constructor_WithNegativeIndex_ThrowsArgumentOutOfRangeException()
	{
		// Arrange
		const int index = -1;
		const string expectedParameter = "index";

		// Act
		static void Act() => _ = new TextFragment("test", index);

		// Assert
		Assert.That
			.ThrowsExactException(Act, out ArgumentOutOfRangeException exception)
			.AreEqual(exception.ParamName, expectedParameter)
			.AreEqual(exception.ActualValue, index);
	}
	#endregion

	#region Equality tests
	[TestMethod]
	public void Equals_Typed_WithEqualItems_ReturnsTrue()
	{
		// Arrange
		const string text = "test";
		const int offset = 1;

		TextFragment sut = new(text, offset);
		TextFragment other = new(text, offset);

		// Act
		bool result = sut.Equals(other);

		// Assert
		Assert.That.IsTrue(result);
	}

	[DataRow("test1", 1, "test2", 1, DisplayName = "Different text")]
	[DataRow("test", 1, "test", 2, DisplayName = "Different index")]
	[DataRow("test1", 1, "test2", 2, DisplayName = "Completely different")]
	[TestMethod]
	public void Equals_Typed_WithDifferentItems_ReturnsFalse(string sutText, int sutIndex, string otherText, int otherIndex)
	{
		// Arrange
		TextFragment sut = new(sutText, sutIndex);
		TextFragment other = new(otherText, otherIndex);

		// Act
		bool result = sut.Equals(other);

		// Assert
		Assert.That.IsFalse(result);
	}

	[TestMethod]
	public void Equals_Untyped_WithEqualItems_ReturnsTrue()
	{
		// Arrange
		const string text = "test";
		const int offset = 1;

		TextFragment sut = new(text, offset);
		object other = new TextFragment(text, offset);

		// Act
		bool result = sut.Equals(other);

		// Assert
		Assert.That.IsTrue(result);
	}

	[DataRow("test1", 1, "test2", 1, DisplayName = "Different text")]
	[DataRow("test", 1, "test", 2, DisplayName = "Different index")]
	[DataRow("test1", 1, "test2", 2, DisplayName = "Completely different")]
	[TestMethod]
	public void Equals_Untyped_WithDifferentItems_ReturnsFalse(string sutText, int sutIndex, string otherText, int otherIndex)
	{
		// Arrange
		TextFragment sut = new(sutText, sutIndex);
		object other = new TextFragment(otherText, otherIndex);

		// Act
		bool result = sut.Equals(other);

		// Assert
		Assert.That.IsFalse(result);
	}

	[TestMethod]
	public void Equals_Untyped_WithDifferentType_ReturnsFalse()
	{
		// Arrange
		TextFragment sut = new("test", 1);
		object? other = new();

		// Act
		bool result = sut.Equals(other);

		// Assert
		Assert.That.IsFalse(result);
	}

	[TestMethod]
	public void GetHashCode_WithEqualItems_ReturnsSameHashCode()
	{
		// Arrange
		const string text = "test";
		const int offset = 1;

		TextFragment sut = new(text, offset);
		TextFragment other = new(text, offset);

		// Act
		int sutHashCode = sut.GetHashCode();
		int otherHashCode = other.GetHashCode();

		// Assert
		Assert.That.AreEqual(sutHashCode, otherHashCode);
	}

	[DataRow("test1", 1, "test2", 1, DisplayName = "Different text")]
	[DataRow("test", 1, "test", 2, DisplayName = "Different index")]
	[DataRow("test1", 1, "test2", 2, DisplayName = "Completely different")]
	[TestMethod]
	public void GetHashCode_WithDifferentItems_ReturnsDifferentHashCode(string sutText, int sutIndex, string otherText, int otherIndex)
	{
		// Note(Nightowl): This test is only a best guess as it is possible (and valid), for two
		// items to have the same hash code even if they differ, because of a hash collision;

		// Arrange
		TextFragment sut = new(sutText, sutIndex);
		TextFragment other = new(otherText, otherIndex);

		// Arrange assert
		Assert.IsConclusiveIf.AreNotEqual(sut, other);

		// Act
		int sutHashCode = sut.GetHashCode();
		int otherHashCode = other.GetHashCode();

		// Assert
		Assert.IsConclusiveIf.AreNotEqual(sutHashCode, otherHashCode);
	}

	[TestMethod]
	public void EqualityOperator_WithEqualItems_ReturnsTrue()
	{
		// Arrange
		const string text = "test";
		const int offset = 1;

		TextFragment sut = new(text, offset);
		TextFragment other = new(text, offset);

		// Act
		bool result = sut == other;

		// Assert
		Assert.That.IsTrue(result);
	}

	[DataRow("test1", 1, "test2", 1, DisplayName = "Different text")]
	[DataRow("test", 1, "test", 2, DisplayName = "Different index")]
	[DataRow("test1", 1, "test2", 2, DisplayName = "Completely different")]
	[TestMethod]
	public void EqualityOperator_WithDifferentItems_ReturnsFalse(string sutText, int sutIndex, string otherText, int otherIndex)
	{
		// Arrange
		TextFragment sut = new(sutText, sutIndex);
		TextFragment other = new(otherText, otherIndex);

		// Act
		bool result = sut == other;

		// Assert
		Assert.That.IsFalse(result);
	}

	[TestMethod]
	public void InequalityOperator_WithEqualItems_ReturnsFalse()
	{
		// Arrange
		const string text = "test";
		const int offset = 1;

		TextFragment sut = new(text, offset);
		TextFragment other = new(text, offset);

		// Act
		bool result = sut != other;

		// Assert
		Assert.That.IsFalse(result);
	}

	[DataRow("test1", 1, "test2", 1, DisplayName = "Different text")]
	[DataRow("test", 1, "test", 2, DisplayName = "Different index")]
	[DataRow("test1", 1, "test2", 2, DisplayName = "Completely different")]
	[TestMethod]
	public void InequalityOperator_WithDifferentItems_ReturnsTrue(string sutText, int sutIndex, string otherText, int otherIndex)
	{
		// Arrange
		TextFragment sut = new(sutText, sutIndex);
		TextFragment other = new(otherText, otherIndex);

		// Act
		bool result = sut != other;

		// Assert
		Assert.That.IsTrue(result);
	}
	#endregion
}
