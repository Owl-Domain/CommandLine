namespace OwlDomain.CommandLine.Output;

/// <summary>
/// 	Represents a command output printer.
/// </summary>
public interface IOutputPrinter
{
	#region Methods
	/// <summary>Prints the given <paramref name="result"/>.</summary>
	/// <param name="result">The result to print.</param>
	void Print(ICommandRunResult result);
	#endregion
}
