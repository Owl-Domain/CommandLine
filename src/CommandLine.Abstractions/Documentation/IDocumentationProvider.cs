namespace OwlDomain.CommandLine.Documentation;

/// <summary>
/// 	Represents a provider for documentation information.
/// </summary>
public interface IDocumentationProvider
{
	#region Methods
	/// <summary>Gets the documentation information for the given <paramref name="type"/>.</summary>
	/// <param name="type">The type to get the documentation information for.</param>
	/// <returns>The documentation information for the given <paramref name="type"/>.</returns>
	IDocumentationInfo? GetInfo(Type type);

	/// <summary>Gets the documentation information for the given <paramref name="property"/>.</summary>
	/// <param name="property">The property to get the documentation information for.</param>
	/// <returns>The documentation information for the given <paramref name="property"/>.</returns>
	IDocumentationInfo? GetInfo(PropertyInfo property);

	/// <summary>Gets the documentation information for the given <paramref name="parameter"/>.</summary>
	/// <param name="parameter">The parameter to get the documentation information for.</param>
	/// <returns>The documentation information for the given <paramref name="parameter"/>.</returns>
	IDocumentationInfo? GetInfo(ParameterInfo parameter);

	/// <summary>Gets the documentation information for the given <paramref name="method"/>.</summary>
	/// <param name="method">The method to get the documentation information for.</param>
	/// <returns>The documentation information for the given <paramref name="method"/>.</returns>
	IDocumentationInfo? GetInfo(MethodInfo method);
	#endregion
}
