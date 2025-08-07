using System.Text;

namespace OwlDomain.CommandLine.Parsing.Values.Primitives;

/// <summary>
/// 	Represents a parser for <see langword="string"/> values.
/// </summary>
public sealed class StringValueParser : BaseValueParser<string>
{
	#region Methods
	/// <inheritdoc/>
	protected override string? TryParse(IValueParseContext context, ITextParser parser, out string? error)
	{
		if (parser.IsGreedy)
		{
			error = null;
			return parser.AdvanceText();
		}

		if (parser.Match('"'))
		{
			StringBuilder builder = new();

			bool closed = false;
			while (parser.Current is not '\0')
			{
				if (parser.Match('"'))
				{
					closed = true;
					break;
				}

				if (parser.Match('\\', '"')) builder.Append('"');
				else if (parser.Match('\\', '\\')) builder.Append('\\');
				else if (parser.Match('\\', 'a')) builder.Append('\a');
				else if (parser.Match('\\', 'b')) builder.Append('\b');
				else if (parser.Match('\\', 't')) builder.Append('\t');
				else if (parser.Match('\\', 'n')) builder.Append('\n');
				else if (parser.Match('\\', 'f')) builder.Append('\f');
				else if (parser.Match('\\', 'e')) builder.Append('\e');
				else if (parser.Match('\\'))
				{
					if (parser.Current is not '\0')
					{
						builder.Append(parser.Current);
						parser.Advance();
					}

					continue;
				}

				builder.Append(parser.Current);
				parser.Advance();
			}

			if (closed is false)
			{
				error = "Quoted string was unclosed.";
				return default;
			}

			error = default;
			return builder.ToString();
		}

		string value = parser.AdvanceUntilBreak();

		error = value.Length is 0 ? string.Empty : null;
		return value;
	}
	#endregion
}
