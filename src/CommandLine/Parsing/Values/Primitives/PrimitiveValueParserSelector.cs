namespace OwlDomain.CommandLine.Parsing.Values.Primitives;

/// <summary>
/// 	Represents a selector for primitive value parsers.
/// </summary>
public sealed class PrimitiveValueParserSelector : BaseValueParserSelector
{
	#region Methods
	/// <inheritdoc/>
	protected override IValueParser? TrySelect(Type type)
	{
		if (type == typeof(string))
			return new StringValueParser();

		return null;
	}
	#endregion
}
