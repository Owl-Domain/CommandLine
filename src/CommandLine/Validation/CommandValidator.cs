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
		if (parserResult.Successful is false)
			return new CommandValidatorResult(false, parserResult, new DiagnosticBag(), default);

		Stopwatch watch = Stopwatch.StartNew();
		DiagnosticBag diagnostics = [];

		watch.Stop();
		CommandValidatorResult result = new(diagnostics.Any() is false, parserResult, diagnostics, watch.Elapsed);

		return result;
	}
	#endregion
}
