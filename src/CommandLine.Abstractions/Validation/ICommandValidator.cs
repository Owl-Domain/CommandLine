namespace OwlDomain.CommandLine.Validation;

/// <summary>
/// 	Represents a validator for parsed commands.
/// </summary>
public interface ICommandValidator
{
	#region Methods
	/// <summary>Validates the given <paramref name="parserResult"/>.</summary>
	/// <param name="parserResult">The parsing result to validate.</param>
	/// <returns>The result of validating the given <paramref name="parserResult"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if the given <paramref name="parserResult"/> cannot be validated.</exception>
	ICommandValidatorResult Validate(ICommandParserResult parserResult);
	#endregion
}
