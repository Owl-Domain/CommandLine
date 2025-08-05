namespace OwlDomain.CommandLine.Validation;

/// <summary>
/// 	Represents a validator for parsed commands.
/// </summary>
public sealed class CommandValidator : ICommandValidator
{
	#region Methods
	/// <inheritdoc/>
	public ICommandValidatorResult Validate(ICommandParserResult parserResult)
	{
		if (parserResult.Diagnostics.Any())
			Throw.New.ArgumentException(nameof(parserResult), $"Validation cannot be performed if there were parsing errors.");

		DiagnosticBag diagnostics = [];
		CommandValidatorResult result = new(parserResult.Engine, parserResult, diagnostics);

		return result;
	}
	#endregion
}
