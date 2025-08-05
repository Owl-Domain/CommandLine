using System.Text;

namespace OwlDomain.CommandLine.Discovery;

/// <summary>
/// 	Represents an extractor for extracting command/flag/argument names.
/// </summary>
public class NameExtractor : INameExtractor
{
	#region Fields
	[ThreadStatic]
	private static StringBuilder? Builder;
	private static NameExtractor? Singleton;
	#endregion

	#region Properties
	private static StringBuilder SharedBuilder
	{
		get
		{
			if (Builder is null)
				return Builder = new();

			return Builder.Clear();
		}
	}

	/// <summary>The shared instance of the name extractor.</summary>
	public static NameExtractor Instance => Singleton ??= new();
	#endregion

	#region Methods
	/// <inheritdoc/>
	public virtual string GetCommandName(string originalName) => Convert(originalName);

	/// <inheritdoc/>
	public virtual string GetArgumentName(string originalName) => Convert(originalName);

	/// <inheritdoc/>
	public virtual char GetShortFlagName(string originalName) => char.ToLower(originalName[0]);

	/// <inheritdoc/>
	public virtual string GetLongFlagName(string originalName) => Convert(originalName);

	/// <inheritdoc/>
	public virtual string? GetCommandName(MethodInfo method)
	{
		if (method.Name is null)
			return null;

		return GetCommandName(method.Name);
	}

	/// <inheritdoc/>
	public virtual string? GetArgumentName(ParameterInfo parameter)
	{
		if (parameter.Name is null)
			return null;

		return GetArgumentName(parameter.Name);
	}

	/// <inheritdoc/>
	public virtual char? GetShortFlagName(ParameterInfo parameter)
	{
		if (parameter.Name is null || parameter.Name.Length is 0)
			return null;

		return GetShortFlagName(parameter.Name);
	}

	/// <inheritdoc/>
	public virtual char? GetShortFlagName(PropertyInfo property)
	{
		if (property.Name is null || property.Name.Length is 0)
			return null;

		return GetShortFlagName(property.Name);
	}

	/// <inheritdoc/>
	public virtual string? GetLongFlagName(ParameterInfo parameter)
	{
		if (parameter.Name is null)
			return null;

		return GetLongFlagName(parameter.Name);
	}

	/// <inheritdoc/>
	public virtual string? GetLongFlagName(PropertyInfo property)
	{
		if (property.Name is null)
			return null;

		return GetLongFlagName(property.Name);
	}
	#endregion

	#region Helpers
	[return: NotNullIfNotNull(nameof(name))]
	private static string? Convert(string? name)
	{
		if (name is null)
			return null;

		StringBuilder builder = SharedBuilder;

		for (int i = 0; i < name.Length; i++)
		{
			char current = name[i];
			char lower = char.ToLower(current);

			if (i == name.Length - 1)
			{
				builder.Append(lower);
				break;
			}

			if (i < name.Length - 2 && char.IsLower(current) && char.IsUpper(name[i + 1]))
			{
				builder
				.Append(lower)
				.Append('-')
				.Append(char.ToLower(name[i + 1]));

				i++;
				continue;
			}

			if (i < name.Length - 3 && char.IsUpper(current) && char.IsUpper(name[i + 1]) && char.IsLower(name[i + 2]))
			{
				builder
					.Append(lower)
					.Append('-')
					.Append(char.ToLower(name[i + 1]))
					.Append(char.ToLower(name[i + 2]));

				i += 2;
				continue;
			}

			builder.Append(lower);
		}

		return builder.ToString();
	}
	#endregion
}
