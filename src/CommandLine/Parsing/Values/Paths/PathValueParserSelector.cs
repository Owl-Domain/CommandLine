namespace OwlDomain.CommandLine.Parsing.Values.Paths;

/// <summary>
/// 	Represents the value parser selector for path-like types.
/// </summary>
public sealed class PathValueParserSelector : BaseValueParserSelector
{
	#region Methods
	/// <inheritdoc/>
	protected override IValueParser? TrySelect(IRootValueParserSelector rootSelector, Type type)
	{
		if (type == typeof(Uri))
			return new UriValueParser();

		return default;
	}
	#endregion
}
