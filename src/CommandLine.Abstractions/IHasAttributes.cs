namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents that the implementing type can have attributes.
/// </summary>
public interface IHasAttributes
{
	#region Methods
	/// <summary>Tries to get an attribute of the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of the attribute to get.</typeparam>
	/// <returns>The obtained attribute, or <see langword="null"/>.</returns>
	T? GetAttribute<T>() where T : Attribute;

	/// <summary>Tries to get an attribute of the given type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of the attribute to try and get.</typeparam>
	/// <param name="attribute">The obtained attribute.</param>
	/// <returns><see langword="true"/> if the <paramref name="attribute"/> could be obtained, <see langword="false"/> otherwise.</returns>
	bool TryGetAttribute<T>([NotNullWhen(true)] out T? attribute) where T : Attribute;

	/// <summary>Tries to get all of the attributes of the given <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of the attributes to get.</typeparam>
	/// <returns>The obtained attributes.</returns>
	IEnumerable<T> GetAttributes<T>() where T : Attribute;

	/// <summary>Tries to get all of the <paramref name="attributes"/> of the given <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of the attributes to get.</typeparam>
	/// <param name="attributes">The obtained attributes.</param>
	/// <returns><see langword="true"/> if the <paramref name="attributes"/> could be found, <see langword="false"/> otherwise.</returns>
	bool TryGetAttributes<T>([NotNullWhen(true)] out IEnumerable<T>? attributes) where T : Attribute;
	#endregion
}
