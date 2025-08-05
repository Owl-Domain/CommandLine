namespace OwlDomain.CommandLine.Validation;

/// <summary>
/// 	Represents the validation result for a parsed command.
/// </summary>
public interface ICommandValidatorResult : IStageResult
{
	#region Properties
	/// <summary>The parsing result that was validated.</summary>
	ICommandParserResult ParserResult { get; }
	#endregion
}
