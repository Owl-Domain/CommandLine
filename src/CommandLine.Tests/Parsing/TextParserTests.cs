namespace OwlDomain.CommandLine.Tests.Parsing;

[TestClass]
public sealed class TextParserTests
{
	#region Constructor tests
	[DataRow(false, DisplayName = "Greedy")]
	[DataRow(true, DisplayName = "Lazy")]
	[TestMethod]
	public void Constructor_WithTextFragments_SetsExpectedProperties(bool expectedIsLazy)
	{
		// Arrange
		TextFragment fragmentA = new("a", 0);
		TextFragment fragmentB = new("b", 1);
		TextFragment[] expectedFragments = [fragmentA, fragmentB];
		const int expectedOffset = 0;

		const char expectedCurrent = 'a';
		const char expectedNext = '\0';
		const bool expectedIsLastFragment = false;
		const bool expectedIsAtEnd = false;
		TextPoint expectedPoint = new(fragmentA, 0);

		// Act
		TextParser Act() => new(expectedFragments, expectedIsLazy);

		// Assert
		Assert.That
			.DoesNotThrowAnyException(Act, out TextParser result)
			.AreEqual(result.IsLazy, expectedIsLazy)
			.AreEqual(result.CurrentFragment, fragmentA)
			.AreEqual(result.Fragments, expectedFragments)
			.AreEqual(result.Offset, expectedOffset)
			.AreEqual(result.Current, expectedCurrent)
			.AreEqual(result.Next, expectedNext)
			.AreEqual(result.IsLastFragment, expectedIsLastFragment)
			.AreEqual(result.IsAtEnd, expectedIsAtEnd)
			.AreEqual(result.Point, expectedPoint);
	}

	[TestMethod]
	public void Constructor_WithEmptyTextFragments_ThrowsArgumentException()
	{
		// Arrange
		TextFragment[] fragments = [];
		const string expectedParameterName = "fragments";

		// Act
		void Act() => _ = new TextParser(fragments, default);

		// Assert
		Assert.That
			.ThrowsExactException(Act, out ArgumentException exception)
			.AreEqual(exception.ParamName, expectedParameterName);
	}

	[TestMethod]
	public void Constructor_WithTextFragmentsNotStartingAtZero_ThrowsArgumentException()
	{
		// Arrange
		TextFragment[] fragments = [
			new("a", 1),
			new("b", 2),
		];
		const string expectedParameterName = "fragments";

		// Act
		void Act() => _ = new TextParser(fragments, default);

		// Assert
		Assert.That
			.ThrowsExactException(Act, out ArgumentException exception)
			.AreEqual(exception.ParamName, expectedParameterName);
	}

	[TestMethod]
	public void Constructor_WithOutOfOrderTextFragments_ThrowsArgumentException()
	{
		// Arrange
		TextFragment[] fragments = [
			new("a", 1),
			new("b", 0),
		];
		const string expectedParameterName = "fragments";

		// Act
		void Act() => _ = new TextParser(fragments, default);

		// Assert
		Assert.That
			.ThrowsExactException(Act, out ArgumentException exception)
			.AreEqual(exception.ParamName, expectedParameterName);
	}

	[DataRow(false, DisplayName = "Greedy")]
	[DataRow(true, DisplayName = "Lazy")]
	[TestMethod]
	public void Constructor_WithStringFragments_SetsExpectedProperties(bool expectedIsLazy)
	{
		// Arrange
		string fragmentTextA = "a", fragmentTextB = "b";
		TextFragment expectedFragmentA = new(fragmentTextA, 0);
		TextFragment expectedFragmentB = new(fragmentTextB, 1);
		const int expectedFragmentCount = 2;
		const int expectedOffset = 0;

		const char expectedCurrent = 'a';
		const char expectedNext = '\0';
		const bool expectedIsLastFragment = false;
		const bool expectedIsAtEnd = false;
		TextPoint expectedPoint = new(expectedFragmentA, 0);

		// Act
		TextParser Act() => new([fragmentTextA, fragmentTextB], expectedIsLazy);

		// Assert
		Assert.That
			.DoesNotThrowAnyException(Act, out TextParser result)
			.AreEqual(result.IsLazy, expectedIsLazy)
			.AreEqual(result.Fragments.Count, expectedFragmentCount)
			.AreEqual(result.CurrentFragment, expectedFragmentA)
			.AreEqual(result.Fragments[0], expectedFragmentA)
			.AreEqual(result.Fragments[1], expectedFragmentB)
			.AreEqual(result.Offset, expectedOffset)
			.AreEqual(result.Current, expectedCurrent)
			.AreEqual(result.Next, expectedNext)
			.AreEqual(result.IsLastFragment, expectedIsLastFragment)
			.AreEqual(result.IsAtEnd, expectedIsAtEnd)
			.AreEqual(result.Point, expectedPoint);
	}

	[TestMethod]
	public void Constructor_WithEmptyStringFragments_ThrowsArgumentException()
	{
		// Arrange
		string[] fragments = [];
		const string expectedParameterName = "fragments";

		// Act
		void Act() => _ = new TextParser(fragments, default);

		// Assert
		Assert.That
			.ThrowsExactException(Act, out ArgumentException exception)
			.AreEqual(exception.ParamName, expectedParameterName);
	}
	#endregion

	#region Advance tests
	[DataRow(0, DisplayName = "Zero")]
	[DataRow(-1, DisplayName = "Negative")]
	[TestMethod]
	public void Advance_WithInvalidValue_ThrowsArgumentOutOfRangeException(int expectedAmount)
	{
		// Arrange
		const string expectedParameterName = "amount";
		TextParser sut = new(["a"], default);

		// Act
		void Act() => sut.Advance(expectedAmount);

		// Assert
		Assert.That
			.ThrowsExactException(Act, out ArgumentOutOfRangeException exception)
			.AreEqual(exception.ActualValue, expectedAmount)
			.AreEqual(exception.ParamName, expectedParameterName);
	}

	[DataRow(1, DisplayName = "Advance by one")]
	[DataRow(2, DisplayName = "Advance by two")]
	[TestMethod]
	public void Advance_WithSmallAmount_AdvancesAppropriately(int expectedAmount)
	{
		// Arrange
		const string fragmentText = "test";
		TextFragment fragment = new(fragmentText, 0);

		TextPoint expectedPoint = new(fragment, expectedAmount);
		string expectedText = fragmentText[expectedAmount..];
		char expectedCurrent = fragmentText[expectedAmount];
		char expectedNext = fragmentText[expectedAmount + 1];

		TextParser sut = new([fragment], default);

		// Arrange assert
		Assert.IsConclusiveIf
			.IsLessThan(expectedAmount, fragmentText.Length)
			.AreEqual(sut.Offset, 0)
			.AreEqual(sut.Point, new(fragment, 0))
			.AreEqual(sut.Current, 't')
			.AreEqual(sut.Next, 'e')
			.AreEqual(sut.IsAtEnd, false)
			.AreEqual(sut.Text.ToString(), fragmentText)
			.AreEqual(sut.TextUntilBreak.ToString(), fragmentText);

		// Act
		sut.Advance(expectedAmount);

		// Assert
		Assert.That
			.AreEqual(sut.Offset, expectedAmount)
			.AreEqual(sut.Point, expectedPoint)
			.AreEqual(sut.Current, expectedCurrent)
			.AreEqual(sut.Next, expectedNext)
			.AreEqual(sut.Text.ToString(), expectedText)
			.AreEqual(sut.TextUntilBreak.ToString(), expectedText)
			.IsFalse(sut.IsAtEnd);
	}

	[TestMethod]
	public void Advance_WithBigAmount_StaysWithinBounds()
	{
		// Arrange
		const string fragmentText = "test";
		TextFragment fragment = new(fragmentText, 0);

		int amount = fragmentText.Length + 5;
		int expectedOffset = fragmentText.Length;
		TextPoint expectedPoint = new(fragment, expectedOffset);

		TextParser sut = new([fragment], default);

		// Arrange assert
		Assert.IsConclusiveIf
			.AreEqual(sut.Offset, 0)
			.AreEqual(sut.Point, new(fragment, 0))
			.AreEqual(sut.Current, 't')
			.AreEqual(sut.Next, 'e')
			.AreEqual(sut.IsAtEnd, false)
			.AreEqual(sut.Text.ToString(), fragmentText)
			.AreEqual(sut.TextUntilBreak.ToString(), fragmentText);

		// Act
		sut.Advance(amount);

		// Assert
		Assert.That
			.AreEqual(sut.Offset, expectedOffset)
			.AreEqual(sut.Point, expectedPoint)
			.AreEqual(sut.Current, '\0')
			.AreEqual(sut.Next, '\0')
			.AreEqual(sut.Text.ToString(), "")
			.AreEqual(sut.TextUntilBreak.ToString(), "")
			.IsTrue(sut.IsAtEnd);
	}
	#endregion

	#region Next fragment tests
	[TestMethod]
	public void NextFragment_AtLastFragment_ThrowsInvalidOperationException()
	{
		// Arrange
		TextParser sut = new(["a"], default);

		// Arrange assert
		Assert.IsConclusiveIf.IsTrue(sut.IsLastFragment);

		// Act
		void Act() => sut.NextFragment();

		// Assert
		Assert.That.ThrowsExactException<InvalidOperationException>(Act);
	}

	[TestMethod]
	public void NextFragment_WithRemainingFragments_AdvancesToNextFragment()
	{
		// Arrange
		TextFragment fragmentA = new("a", 0);
		TextFragment fragmentB = new("b", 1);

		TextParser sut = new([fragmentA, fragmentB], default);
		sut.Advance();

		// Arrange assert
		Assert.IsConclusiveIf
			.AreEqual(sut.Fragments.Count, 2)
			.AreEqual(sut.CurrentFragment, fragmentA)
			.IsFalse(sut.IsLastFragment)
			.AreEqual(sut.Offset, 1);

		// Act
		sut.NextFragment();

		// Assert
		Assert.That
			.AreEqual(sut.Fragments.Count, 2)
			.AreEqual(sut.CurrentFragment, fragmentB)
			.IsTrue(sut.IsLastFragment)
			.AreEqual(sut.Offset, 0);
	}
	#endregion

	#region Restore tests
	[DataRow(-1, DisplayName = "Negative")]
	[DataRow(1, DisplayName = "Too big")]
	[TestMethod]
	public void Restore_WithInvalidFragmentIndex_ThrowsArgumentOutOfRangeException(int expectedFragmentIndex)
	{
		// Arrange
		const string expectedParameterName = "fragmentIndex";
		TextParser sut = new(["a"], default);

		// Act
		void Act() => sut.Restore(expectedFragmentIndex, default);

		// Assert
		Assert.That
			.ThrowsExactException(Act, out ArgumentOutOfRangeException exception)
			.AreEqual(exception.ActualValue, expectedFragmentIndex)
			.AreEqual(exception.ParamName, expectedParameterName);
	}

	[DataRow(-1, DisplayName = "Negative")]
	[DataRow(2, DisplayName = "Too big")]
	[TestMethod]
	public void Restore_WithInvalidOffset_ThrowsArgumentOutOfRangeException(int expectedOffset)
	{
		// Arrange
		const string expectedParameterName = "offset";
		TextParser sut = new(["a"], default);

		// Act
		void Act() => sut.Restore(0, expectedOffset);

		// Assert
		// Assert
		Assert.That
			.ThrowsExactException(Act, out ArgumentOutOfRangeException exception)
			.AreEqual(exception.ActualValue, expectedOffset)
			.AreEqual(exception.ParamName, expectedParameterName);
	}

	[TestMethod]
	public void Restore_WithValidRestorePoint_RestoresAsExpected()
	{
		// Arrange
		const int expectedOffset = 1;

		TextFragment fragmentA = new("a", 0);
		TextFragment fragmentB = new("bb", 1);

		TextParser sut = new([fragmentA, fragmentB], default);
		sut.NextFragment();
		sut.Advance(2);


		// Arrange assert
		Assert.IsConclusiveIf
			.AreEqual(sut.CurrentFragment, fragmentB)
			.AreEqual(sut.Offset, 2);

		// Act
		sut.Restore(0, expectedOffset);

		// Assert
		Assert.That
			.AreEqual(sut.CurrentFragment, fragmentA)
			.AreEqual(sut.Offset, expectedOffset);
	}
	#endregion

	#region Skip whitespace tests
	[DataRow("", DisplayName = "Empty fragment")]
	[DataRow("a", DisplayName = "No whitespace")]
	[TestMethod]
	public void SkipWhitespace_NotAtWhitespace_DoesNotMove(string fragmentText)
	{
		// Arrange
		TextParser sut = new([fragmentText], default);

		// Arrange assert
		Assert.IsConclusiveIf.AreEqual(sut.Offset, 0);

		// Act
		sut.SkipWhitespace();

		// Assert
		Assert.That.AreEqual(sut.Offset, 0);
	}

	[DataRow(" a", "a", DisplayName = "Single space")]
	[DataRow("   a", "a", DisplayName = "Multiple spaces")]
	[DataRow("   ", "", DisplayName = "Only spaces")]
	[DataRow(" a b", "a b", DisplayName = "Single space, with extra space after stop point")]
	[DataRow("   a   b", "a   b", DisplayName = "Multiple spaces, with extra spaces after stop point")]
	[DataRow("  \t   a", "a", DisplayName = "Mixed whitespace")]
	[TestMethod]
	public void SkipWhitespace_AtWhitespace_SkipsAllWhitespace(string fragmentText, string expectedFragmentText)
	{
		// Arrange
		TextParser sut = new([fragmentText], default);

		// Arrange assert
		Assert.IsConclusiveIf.AreEqual(sut.Text.ToString(), fragmentText);

		// Act
		sut.SkipWhitespace();

		// Assert
		Assert.That.AreEqual(sut.Text.ToString(), expectedFragmentText);
	}
	#endregion

	#region Skip trivia tests
	[TestMethod]
	public void SkipTrivia_NotAtTrivia_DoesNotMove()
	{
		// Arrange
		TextParser sut = new(["a"], default);

		// Arrange assert
		Assert.IsConclusiveIf.AreEqual(sut.Offset, 0);

		// Act
		sut.SkipTrivia();

		// Assert
		Assert.That.AreEqual(sut.Offset, 0);
	}

	[TestMethod]
	public void SkipTrivia_AtWhitespace_SkipsWhitespace()
	{
		// Arrange
		const string fragmentText = "  \t  a";
		const string expectedText = "a";

		TextParser sut = new([fragmentText], default);

		// Arrange assert
		Assert.IsConclusiveIf.AreEqual(sut.Text.ToString(), fragmentText);

		// Act
		sut.SkipTrivia();

		// Assert
		Assert.That.AreEqual(sut.Text.ToString(), expectedText);
	}

	[TestMethod]
	public void SkipTrivia_AtEndOfFragment_MovesToNextFragment()
	{
		// Arrange
		TextFragment fragmentA = new("a", 0);
		TextFragment fragmentB = new("b", 1);

		TextParser sut = new([fragmentA, fragmentB], default);
		sut.Advance();

		// Arrange assert
		Assert.IsConclusiveIf
			.AreEqual(sut.CurrentFragment, fragmentA)
			.IsTrue(sut.IsAtEnd)
			.IsFalse(sut.IsLastFragment);

		// Act
		sut.SkipTrivia();

		// Assert
		Assert.That
			.AreEqual(sut.CurrentFragment, fragmentB)
			.AreEqual(sut.Offset, 0)
			.IsTrue(sut.IsLastFragment);
	}

	[TestMethod]
	public void SkipTrivia_AtWhitespaceBeforeEndOfFragment_SkipsWhitespaceAndMovesToNextFragment()
	{
		// Arrange
		TextFragment fragmentA = new("a  \t  ", 0);

		// Note(Nightowl): The white-space at the start of the next fragment should specifically not be skipped;
		TextFragment fragmentB = new(" \t b", 1);

		TextParser sut = new([fragmentA, fragmentB], default);
		sut.Advance();

		// Arrange assert
		Assert.IsConclusiveIf
			.AreEqual(sut.CurrentFragment, fragmentA)
			.IsFalse(sut.IsAtEnd)
			.AreEqual(sut.Current, ' ')
			.IsFalse(sut.IsLastFragment);

		// Act
		sut.SkipTrivia();

		// Assert
		Assert.That
			.AreEqual(sut.CurrentFragment, fragmentB)
			.AreEqual(sut.Offset, 0)
			.IsTrue(sut.IsLastFragment);
	}
	#endregion

	#region Text until break tests
	[TestMethod]
	public void TextUntilBreak_GreedyMode_ReturnsFullText()
	{
		// Arrange
		const string expectedResult = "a";
		TextParser parser = new([expectedResult], false);

		// Act
		string result = parser.TextUntilBreak.ToString();

		// Assert
		Assert.That.AreEqual(result, expectedResult);
	}

	[TestMethod]
	public void TextUntilBreak_LazyMode_WithNoBreaks_ReturnsFullText()
	{
		// Arrange
		const string expectedResult = "a";
		TextParser parser = new([expectedResult], true);

		// Act
		string result = parser.TextUntilBreak.ToString();

		// Assert
		Assert.That.AreEqual(result, expectedResult);
	}

	[TestMethod]
	public void TextUntilBreak_LazyMode_WithOneBreak_ReturnsTextUntilBreak()
	{
		// Arrange
		const string fragmentText = "a b";
		const string expectedResult = "a";
		TextParser parser = new([fragmentText], true);

		// Act
		string result = parser.TextUntilBreak.ToString();

		// Assert
		Assert.That.AreEqual(result, expectedResult);
	}

	[TestMethod]
	public void TextUntilBreak_LazyMode_WithMultipleBreaks_ReturnsTextUntilFirstBreak()
	{
		// Arrange
		const string fragmentText = "a b c";
		const string expectedResult = "a";
		TextParser parser = new([fragmentText], true);

		// Act
		string result = parser.TextUntilBreak.ToString();

		// Assert
		Assert.That.AreEqual(result, expectedResult);
	}
	#endregion
}
