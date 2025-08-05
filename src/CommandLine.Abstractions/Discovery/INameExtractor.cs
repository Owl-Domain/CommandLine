namespace OwlDomain.CommandLine.Discovery;

/// <summary>
/// 	Represents an extractor for extracting command/flag/argument names.
/// </summary>
public interface INameExtractor
{
	#region Methods
	/// <summary>Gets the command name from the given <paramref name="originalName"/>.</summary>
	/// <param name="originalName">The original name to extract the command name from.</param>
	/// <returns>The extracted command name.</returns>
	string GetCommandName(string originalName);

	/// <summary>Gets the argument name from the given <paramref name="originalName"/>.</summary>
	/// <param name="originalName">The original name to extract the argument name from.</param>
	/// <returns>The extracted argument name.</returns>
	string GetArgumentName(string originalName);

	/// <summary>Gets the short flag name from the given <paramref name="originalName"/>.</summary>
	/// <param name="originalName">The original name to extract the short flag name from.</param>
	/// <returns>The extracted short flag name.</returns>
	char GetShortFlagName(string originalName);

	/// <summary>Gets the long flag name from the given <paramref name="originalName"/>.</summary>
	/// <param name="originalName">The original name to extract the long flag name from.</param>
	/// <returns>The extracted long flag name.</returns>
	string GetLongFlagName(string originalName);

	/// <summary>Gets the command name from the given <paramref name="method"/>.</summary>
	/// <param name="method">The method to extract the command name from.</param>
	/// <returns>The extracted command name, or <see langword="null"/> if a name couldn't be extracted.</returns>
	string? GetCommandName(MethodInfo method);

	/// <summary>Gets the argument name from the given <paramref name="parameter"/>.</summary>
	/// <param name="parameter">The parameter to extract the argument name from.</param>
	/// <returns>The extracted argument name, or <see langword="null"/> if a name couldn't be extracted.</returns>
	string? GetArgumentName(ParameterInfo parameter);

	/// <summary>Gets the short flag name from the given <paramref name="parameter"/>.</summary>
	/// <param name="parameter">The parameter to extract the short flag name from.</param>
	/// <returns>The extracted short flag name, or <see langword="null"/> if a name couldn't be extracted.</returns>
	char? GetShortFlagName(ParameterInfo parameter);

	/// <summary>Gets the short flag name from the given <paramref name="property"/>.</summary>
	/// <param name="property">The property to extract the short flag name from.</param>
	/// <returns>The extracted short flag name, or <see langword="null"/> if a name couldn't be extracted.</returns>
	char? GetShortFlagName(PropertyInfo property);

	/// <summary>Gets the long flag name from the given <paramref name="parameter"/>.</summary>
	/// <param name="parameter">The parameter to extract the long flag name from.</param>
	/// <returns>The extracted long flag name, or <see langword="null"/> if a name couldn't be extracted.</returns>
	string? GetLongFlagName(ParameterInfo parameter);

	/// <summary>Gets the long flag name from the given <paramref name="property"/>.</summary>
	/// <param name="property">The property to extract the long flag name from.</param>
	/// <returns>The extracted long flag name, or <see langword="null"/> if a name couldn't be extracted.</returns>
	string? GetLongFlagName(PropertyInfo property);
	#endregion
}
