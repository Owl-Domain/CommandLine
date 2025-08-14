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
			return new CommandValidatorResult(false, parserResult.WasCancelled, parserResult, new DiagnosticBag(), default);

		Stopwatch watch = Stopwatch.StartNew();
		DiagnosticBag diagnostics = [];

		TimeSpan timeout = parserResult.Engine.Settings.ValidationTimeout;
		if (timeout == TimeSpan.Zero)
			return Validate(parserResult, diagnostics, watch, default);

		try
		{
			CancellationTokenSource source = new(timeout);

			return Validate(parserResult, diagnostics, watch, source.Token);
		}
		catch (OperationCanceledException)
		{
			watch.Stop();
			return new CommandValidatorResult(false, true, parserResult, diagnostics, watch.Elapsed);
		}
	}
	private ICommandValidatorResult Validate(
		ICommandParserResult parserResult,
		DiagnosticBag diagnostics,
		Stopwatch watch,
		CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		watch.Stop();

		return new CommandValidatorResult(true, false, parserResult, diagnostics, watch.Elapsed);
	}
	#endregion
}
