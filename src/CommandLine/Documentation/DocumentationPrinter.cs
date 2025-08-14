using System.Text;
using OwlDomain.Documentation.Document.Nodes;

namespace OwlDomain.CommandLine.Documentation;

/// <summary>
/// 	Represents a printer for documentation info.
/// </summary>
public sealed class DocumentationPrinter : IDocumentationPrinter
{
	#region Methods
	/// <inheritdoc/>
	public void Print(ICommandEngine engine)
	{
		Print(engine, engine.RootGroup);
	}

	/// <inheritdoc/>
	public void Print(ICommandEngine engine, ICommandGroupInfo group)
	{
		if (group.Groups.Count > 0)
		{
			Console.WriteLine("[Command groups]");
			foreach (ICommandGroupInfo subGroup in group.Groups.Values)
			{
				Debug.Assert(subGroup.Name is not null);
				Console.WriteLine($"{subGroup.Name} \t {GetBasicSummary(subGroup.Documentation)}");
			}

			if (group.SharedFlags.Count > 0)
				Console.WriteLine();
		}

		if (group.SharedFlags.Count > 0)
		{
			Console.WriteLine("[Shared flags]");
			foreach (IFlagInfo flag in group.SharedFlags)
			{
				if (flag.ShortName is not null)
					Console.Write($"{engine.Settings.ShortFlagPrefix}{flag.ShortName}");

				if (flag.LongName is not null)
				{
					if (flag.ShortName is not null)
						Console.Write(' ');
					Console.Write($"{engine.Settings.LongFlagPrefix}{flag.LongName}");
				}

				Console.WriteLine($" \t {GetBasicSummary(flag.Documentation)}");
			}

			if (group.Commands.Count > 0)
				Console.WriteLine();
		}

		if (group.Commands.Count > 0)
		{
			Console.WriteLine("[Commands]");
			foreach (ICommandInfo command in group.Commands.Values)
			{
				Debug.Assert(command.Name is not null);
				Console.WriteLine($"{command.Name} \t {GetBasicSummary(command.Documentation)}");
			}
		}
	}

	/// <inheritdoc/>
	public void Print(ICommandEngine engine, ICommandInfo command)
	{
		if (command.Flags.Count > 0)
		{
			Console.WriteLine("[Flags]");
			foreach (IFlagInfo flag in command.Flags)
			{
				if (flag.ShortName is not null)
					Console.Write($"{engine.Settings.ShortFlagPrefix}{flag.ShortName}");

				if (flag.LongName is not null)
				{
					if (flag.ShortName is not null)
						Console.Write(' ');
					Console.Write($"{engine.Settings.LongFlagPrefix}{flag.LongName}");
				}

				Console.WriteLine($" \t {GetBasicSummary(flag.Documentation)}");
			}

			if (command.Arguments.Count > 0)
				Console.WriteLine();
		}

		if (command.Arguments.Count > 0)
		{
			Console.WriteLine("[Arguments]");
			foreach (IArgumentInfo argument in command.Arguments)
				Console.WriteLine($"{argument.Name} \t {GetBasicSummary(argument.Documentation)}");
		}
	}
	#endregion

	#region Helpers
	private static string? GetBasicSummary(IDocumentationInfo? info)
	{
		if (info is null)
			return null;

		if (info.Summary is ITextDocumentationNode rootText)
			return rootText.Text;

		if (info.Summary is IDocumentationNodeCollection collection)
		{
			StringBuilder builder = new();

			foreach (IDocumentationNode node in collection.Children)
			{
				if (node is ITextDocumentationNode text) builder.Append(text.Text);
			}

			return builder.ToString();
		}

		return null;
	}
	#endregion
}
