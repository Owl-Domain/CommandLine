namespace OwlDomain.CommandLine.Tests.Parsing;

[TestClass]
public sealed class TextPointTests
{
	#region Constructor tests
	[TestMethod]
	public void Constructor_WithValidValues_SetsExpectedProperties()
	{
		// Arrange
		TextFragment expectedFragment = new("test", 1);
		const int expectedOffset = 1;

		// Act
		TextPoint Act() => new(expectedFragment, expectedOffset);

		// Assert
		Assert.That
			.DoesNotThrowAnyException(Act, out TextPoint result)
			.AreEqual(result.Fragment, expectedFragment)
			.AreEqual(result.Offset, expectedOffset);
	}

	[DataRow(-1, DisplayName = "Negative offset")]
	[DataRow(5, DisplayName = "Offset past fragment length")]
	[TestMethod]
	public void Constructor_OffsetOutOfRange_ThrowsArgumentOutOfRangeException(int offset)
	{
		// Arrange
		TextFragment fragment = new("test", 1);
		const string expectedParameter = "offset";

		// Act
		void Act() => _ = new TextPoint(fragment, offset);

		// Assert
		Assert.That
			.ThrowsExactException(Act, out ArgumentOutOfRangeException exception)
			.AreEqual(exception.ParamName, expectedParameter)
			.AreEqual(exception.ActualValue, offset);
	}

	[TestMethod]
	public void Constructor_WithEmptyFragment_OffsetAtZero_IsSuccessful()
	{
		// Arrange
		TextFragment expectedFragment = new("", 0);
		const int expectedOffset = 0;

		// Act
		TextPoint Act() => new(expectedFragment, expectedOffset);

		// Assert
		Assert.That
			.DoesNotThrowAnyException(Act, out TextPoint result)
			.AreEqual(result.Fragment, expectedFragment)
			.AreEqual(result.Offset, expectedOffset);
	}

	[TestMethod]
	public void Constructor_WithEmptyFragment_OffsetPastFragment_ThrowsArgumentOutOfRangeException()
	{
		// Arrange
		TextFragment fragment = new("", 0);
		const int offset = 1;
		const string expectedParameter = "offset";

		// Act
		void Act() => _ = new TextPoint(fragment, offset);

		// Assert
		Assert.That
			.ThrowsExactException(Act, out ArgumentOutOfRangeException exception)
			.AreEqual(exception.ParamName, expectedParameter)
			.AreEqual(exception.ActualValue, offset);
	}
	#endregion

	#region Equality tests
	[TestMethod]
	public void Equals_Typed_WithEqualPoints_ReturnsTrue()
	{
		// Arrange
		const string text = "test";
		const int offset = 1;

		TextPoint sut = new(new(text, 1), offset);
		TextPoint other = new(new(text, 1), offset);

		// Act
		bool result = sut.Equals(other);

		// Assert
		Assert.That.IsTrue(result);
	}

	[DataRow("test1", 1, "test2", 1, DisplayName = "Different fragment")]
	[DataRow("test", 1, "test", 2, DisplayName = "Different offset")]
	[DataRow("test1", 1, "test2", 2, DisplayName = "Completely different")]
	[TestMethod]
	public void Equals_Typed_WithDifferentPoints_ReturnsFalse(string sutFragmentText, int sutOffset, string otherFragmentText, int otherOffset)
	{
		// Arrange
		TextPoint sut = new(new(sutFragmentText, 1), sutOffset);
		TextPoint other = new(new(otherFragmentText, 1), otherOffset);

		// Act
		bool result = sut.Equals(other);

		// Assert
		Assert.That.IsFalse(result);
	}

	[TestMethod]
	public void Equals_Untyped_WithEqualPoints_ReturnsTrue()
	{
		// Arrange
		const string text = "test";
		const int offset = 1;

		TextPoint sut = new(new(text, 1), offset);
		object other = new TextPoint(new(text, 1), offset);

		// Act
		bool result = sut.Equals(other);

		// Assert
		Assert.That.IsTrue(result);
	}

	[DataRow("test1", 1, "test2", 1, DisplayName = "Different fragment")]
	[DataRow("test", 1, "test", 2, DisplayName = "Different offset")]
	[DataRow("test1", 1, "test2", 2, DisplayName = "Completely different")]
	[TestMethod]
	public void Equals_Untyped_WithDifferentPoints_ReturnsFalse(string sutFragmentText, int sutOffset, string otherFragmentText, int otherOffset)
	{
		// Arrange
		TextPoint sut = new(new(sutFragmentText, 1), sutOffset);
		object other = new TextPoint(new(otherFragmentText, 1), otherOffset);

		// Act
		bool result = sut.Equals(other);

		// Assert
		Assert.That.IsFalse(result);
	}

	[TestMethod]
	public void Equals_Untyped_WithDifferentType_ReturnsFalse()
	{
		// Arrange
		TextPoint sut = new(new("test", 1), 1);
		object? other = new();

		// Act
		bool result = sut.Equals(other);

		// Assert
		Assert.That.IsFalse(result);
	}

	[TestMethod]
	public void GetHashCode_WithEqualPoints_ReturnsSameHashCode()
	{
		// Arrange
		const string text = "test";
		const int offset = 1;

		TextPoint sut = new(new(text, 1), offset);
		TextPoint other = new(new(text, 1), offset);

		// Act
		int sutHashCode = sut.GetHashCode();
		int otherHashCode = other.GetHashCode();

		// Assert
		Assert.That.AreEqual(sutHashCode, otherHashCode);
	}

	[DataRow("test1", 1, "test2", 1, DisplayName = "Different fragment")]
	[DataRow("test", 1, "test", 2, DisplayName = "Different offset")]
	[DataRow("test1", 1, "test2", 2, DisplayName = "Completely different")]
	[TestMethod]
	public void GetHashCode_WithDifferentPoints_ReturnsDifferentHashCode(string sutFragmentText, int sutOffset, string otherFragmentText, int otherOffset)
	{
		// Note(Nightowl): This test is only a best guess as it is possible (and valid), for two
		// points to have the same hash code even if they differ, because of a hash collision;

		// Arrange
		TextPoint sut = new(new(sutFragmentText, 1), sutOffset);
		TextPoint other = new(new(otherFragmentText, 1), otherOffset);

		// Arrange assert
		Assert.IsConclusiveIf.AreNotEqual(sut, other);

		// Act
		int sutHashCode = sut.GetHashCode();
		int otherHashCode = other.GetHashCode();

		// Assert
		Assert.IsConclusiveIf.AreNotEqual(sutHashCode, otherHashCode);
	}

	[TestMethod]
	public void EqualityOperator_WithEqualPoints_ReturnsTrue()
	{
		// Arrange
		const string text = "test";
		const int offset = 1;

		TextPoint sut = new(new(text, 1), offset);
		TextPoint other = new(new(text, 1), offset);

		// Act
		bool result = sut == other;

		// Assert
		Assert.That.IsTrue(result);
	}

	[DataRow("test1", 1, "test2", 1, DisplayName = "Different fragment")]
	[DataRow("test", 1, "test", 2, DisplayName = "Different offset")]
	[DataRow("test1", 1, "test2", 2, DisplayName = "Completely different")]
	[TestMethod]
	public void EqualityOperator_WithDifferentPoints_ReturnsFalse(string sutFragmentText, int sutOffset, string otherFragmentText, int otherOffset)
	{
		// Arrange
		TextPoint sut = new(new(sutFragmentText, 1), sutOffset);
		TextPoint other = new(new(otherFragmentText, 1), otherOffset);

		// Act
		bool result = sut == other;

		// Assert
		Assert.That.IsFalse(result);
	}

	[TestMethod]
	public void InequalityOperator_WithEqualPoints_ReturnsFalse()
	{
		// Arrange
		const string text = "test";
		const int offset = 1;

		TextPoint sut = new(new(text, 1), offset);
		TextPoint other = new(new(text, 1), offset);

		// Act
		bool result = sut != other;

		// Assert
		Assert.That.IsFalse(result);
	}

	[DataRow("test1", 1, "test2", 1, DisplayName = "Different fragment")]
	[DataRow("test", 1, "test", 2, DisplayName = "Different offset")]
	[DataRow("test1", 1, "test2", 2, DisplayName = "Completely different")]
	[TestMethod]
	public void InequalityOperator_WithDifferentPoints_ReturnsTrue(string sutFragmentText, int sutOffset, string otherFragmentText, int otherOffset)
	{
		// Arrange
		TextPoint sut = new(new(sutFragmentText, 1), sutOffset);
		TextPoint other = new(new(otherFragmentText, 1), otherOffset);

		// Act
		bool result = sut != other;

		// Assert
		Assert.That.IsTrue(result);
	}
	#endregion

	#region Comparison tests
	[TestMethod]
	public void CompareTo_WithEqualPoints_ReturnsZero()
	{
		// Arrange
		const int expectedResult = 0;
		const int offset = 1;

		TextFragment fragment = new("test", 1);

		TextPoint sut = new(fragment, offset);
		TextPoint other = new(fragment, offset);

		// Act
		int result = sut.CompareTo(other);

		// Assert
		Assert.That.AreEqual(result, expectedResult);
	}

	[DataRow(1, 1, DisplayName = "Same index")]
	[DataRow(1, 2, DisplayName = "Greater index")]
	[TestMethod]
	public void CompareTo_WithGreaterPoints_ReturnsNegative(int sutIndex, int otherIndex)
	{
		// Arrange
		const int sutOffset = 1;
		const int otherOffset = 2;

		TextFragment sutFragment = new("test", sutIndex);
		TextFragment otherFragment = new("test", otherIndex);

		TextPoint sut = new(sutFragment, sutOffset);
		TextPoint other = new(otherFragment, otherOffset);

		// Act
		int result = sut.CompareTo(other);

		// Assert
		Assert.That.IsLessThan(result, 0);
	}

	[DataRow(2, 2, DisplayName = "Same index")]
	[DataRow(2, 1, DisplayName = "Lower index")]
	[TestMethod]
	public void CompareTo_WithLowerPoints_ReturnsPositive(int sutIndex, int otherIndex)
	{
		// Arrange
		const int sutOffset = 2;
		const int otherOffset = 1;

		TextFragment sutFragment = new("test", sutIndex);
		TextFragment otherFragment = new("test", otherIndex);

		TextPoint sut = new(sutFragment, sutOffset);
		TextPoint other = new(otherFragment, otherOffset);

		// Act
		int result = sut.CompareTo(other);

		// Assert
		Assert.That.IsGreaterThan(result, 0);
	}

	[TestMethod]
	public void LessThanOperator_WithEqualPoints_ReturnsFalse()
	{
		// Arrange
		const int offset = 1;

		TextFragment fragment = new("test", 1);

		TextPoint sut = new(fragment, offset);
		TextPoint other = new(fragment, offset);

		// Act
		bool result = sut < other;

		// Assert
		Assert.That.IsFalse(result);
	}

	[DataRow(1, 1, DisplayName = "Same index")]
	[DataRow(1, 2, DisplayName = "Greater index")]
	[TestMethod]
	public void LessThanOperator_WithGreaterPoints_ReturnsTrue(int sutIndex, int otherIndex)
	{
		// Arrange
		const int sutOffset = 1;
		const int otherOffset = 2;

		TextFragment sutFragment = new("test", sutIndex);
		TextFragment otherFragment = new("test", otherIndex);

		TextPoint sut = new(sutFragment, sutOffset);
		TextPoint other = new(otherFragment, otherOffset);

		// Act
		bool result = sut < other;

		// Assert
		Assert.That.IsTrue(result);
	}

	[DataRow(2, 2, DisplayName = "Same index")]
	[DataRow(2, 1, DisplayName = "Lower index")]
	[TestMethod]
	public void LessThanOperator_WithLowerPoints_ReturnsFalse(int sutIndex, int otherIndex)
	{
		// Arrange
		const int sutOffset = 2;
		const int otherOffset = 1;

		TextFragment sutFragment = new("test", sutIndex);
		TextFragment otherFragment = new("test", otherIndex);

		TextPoint sut = new(sutFragment, sutOffset);
		TextPoint other = new(otherFragment, otherOffset);

		// Act
		bool result = sut < other;

		// Assert
		Assert.That.IsFalse(result);
	}

	[TestMethod]
	public void LessThanOrEqualToOperator_WithEqualPoints_ReturnsTrue()
	{
		// Arrange
		const int offset = 1;

		TextFragment fragment = new("test", 1);

		TextPoint sut = new(fragment, offset);
		TextPoint other = new(fragment, offset);

		// Act
		bool result = sut <= other;

		// Assert
		Assert.That.IsTrue(result);
	}

	[DataRow(1, 1, DisplayName = "Same index")]
	[DataRow(1, 2, DisplayName = "Greater index")]
	[TestMethod]
	public void LessThanOrEqualToOperator_WithGreaterPoints_ReturnsTrue(int sutIndex, int otherIndex)
	{
		// Arrange
		const int sutOffset = 1;
		const int otherOffset = 2;

		TextFragment sutFragment = new("test", sutIndex);
		TextFragment otherFragment = new("test", otherIndex);

		TextPoint sut = new(sutFragment, sutOffset);
		TextPoint other = new(otherFragment, otherOffset);

		// Act
		bool result = sut <= other;

		// Assert
		Assert.That.IsTrue(result);
	}

	[DataRow(2, 2, DisplayName = "Same index")]
	[DataRow(2, 1, DisplayName = "Lower index")]
	[TestMethod]
	public void LessThanOrEqualToOperator_WithLowerPoints_ReturnsFalse(int sutIndex, int otherIndex)
	{
		// Arrange
		const int sutOffset = 2;
		const int otherOffset = 1;

		TextFragment sutFragment = new("test", sutIndex);
		TextFragment otherFragment = new("test", otherIndex);

		TextPoint sut = new(sutFragment, sutOffset);
		TextPoint other = new(otherFragment, otherOffset);

		// Act
		bool result = sut <= other;

		// Assert
		Assert.That.IsFalse(result);
	}

	[TestMethod]
	public void GreaterThanOperator_WithEqualPoints_ReturnsFalse()
	{
		// Arrange
		const int offset = 1;

		TextFragment fragment = new("test", 1);

		TextPoint sut = new(fragment, offset);
		TextPoint other = new(fragment, offset);

		// Act
		bool result = sut > other;

		// Assert
		Assert.That.IsFalse(result);
	}

	[DataRow(1, 1, DisplayName = "Same index")]
	[DataRow(1, 2, DisplayName = "Greater index")]
	[TestMethod]
	public void GreaterThanOperator_WithGreaterPoints_ReturnsFalse(int sutIndex, int otherIndex)
	{
		// Arrange
		const int sutOffset = 1;
		const int otherOffset = 2;

		TextFragment sutFragment = new("test", sutIndex);
		TextFragment otherFragment = new("test", otherIndex);

		TextPoint sut = new(sutFragment, sutOffset);
		TextPoint other = new(otherFragment, otherOffset);

		// Act
		bool result = sut > other;

		// Assert
		Assert.That.IsFalse(result);
	}

	[DataRow(2, 2, DisplayName = "Same index")]
	[DataRow(2, 1, DisplayName = "Lower index")]
	[TestMethod]
	public void GreaterThanOperator_WithLowerPoints_ReturnsTrue(int sutIndex, int otherIndex)
	{
		// Arrange
		const int sutOffset = 2;
		const int otherOffset = 1;

		TextFragment sutFragment = new("test", sutIndex);
		TextFragment otherFragment = new("test", otherIndex);

		TextPoint sut = new(sutFragment, sutOffset);
		TextPoint other = new(otherFragment, otherOffset);

		// Act
		bool result = sut > other;

		// Assert
		Assert.That.IsTrue(result);
	}

	[TestMethod]
	public void GreaterThanOrEqualToOperator_WithEqualPoints_ReturnsTrue()
	{
		// Arrange
		const int offset = 1;

		TextFragment fragment = new("test", 1);

		TextPoint sut = new(fragment, offset);
		TextPoint other = new(fragment, offset);

		// Act
		bool result = sut >= other;

		// Assert
		Assert.That.IsTrue(result);
	}

	[DataRow(1, 1, DisplayName = "Same index")]
	[DataRow(1, 2, DisplayName = "Greater index")]
	[TestMethod]
	public void GreaterThanOrEqualToOperator_WithGreaterPoints_ReturnsFalse(int sutIndex, int otherIndex)
	{
		// Arrange
		const int sutOffset = 1;
		const int otherOffset = 2;

		TextFragment sutFragment = new("test", sutIndex);
		TextFragment otherFragment = new("test", otherIndex);

		TextPoint sut = new(sutFragment, sutOffset);
		TextPoint other = new(otherFragment, otherOffset);

		// Act
		bool result = sut >= other;

		// Assert
		Assert.That.IsFalse(result);
	}

	[DataRow(2, 2, DisplayName = "Same index")]
	[DataRow(2, 1, DisplayName = "Lower index")]
	[TestMethod]
	public void GreaterThanOrEqualToOperator_WithLowerPoints_ReturnsTrue(int sutIndex, int otherIndex)
	{
		// Arrange
		const int sutOffset = 2;
		const int otherOffset = 1;

		TextFragment sutFragment = new("test", sutIndex);
		TextFragment otherFragment = new("test", otherIndex);

		TextPoint sut = new(sutFragment, sutOffset);
		TextPoint other = new(otherFragment, otherOffset);

		// Act
		bool result = sut >= other;

		// Assert
		Assert.That.IsTrue(result);
	}
	#endregion
}
