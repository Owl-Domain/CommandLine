using System.Text;
using OwlDomain.Documentation.Document.Nodes;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace OwlDomain.CommandLine.Documentation;

/// <summary>
/// 	Represents a printer for documentation info.
/// </summary>
public sealed class DocumentationPrinter : IDocumentationPrinter
{
	#region Constants
	private const string HeaderStyle = "bold purple";
	private const string SymbolStyle = "yellow";
	private const string FlagPrefixStyle = SymbolStyle;
	private const string FlagNameStyle = "blue";
	private const string FlagValueStyle = "italic cyan";
	private const string RequiredStyle = "bold yellow";
	private const string OptionalStyle = "italic gray";
	private const string GroupNameStyle = "green";
	private const string CommandNameStyle = "lime";
	private const string DefaultLabelStyle = "seagreen1";
	private const string ArgumentStyle = "cyan";
	#endregion

	#region Methods
	/// <inheritdoc/>
	public void Print(ICommandEngine engine)
	{
		List<IRenderable> items = [];

		TryGetProjectInfo(engine.Settings, items);
		TryGetGroups(engine.RootGroup.Groups, items);
		TryGetFlags(engine.Settings, engine.RootGroup.SharedFlags, items);
		TryGetCommands(engine.RootGroup.Commands, items);

		PrintItems(items);
	}

	/// <inheritdoc/>
	public void Print(ICommandEngine engine, ICommandGroupInfo group)
	{
		if (group == engine.RootGroup)
		{
			Print(engine);
			return;
		}

		List<IRenderable> items = [];

		TryGetDocumentation(group.Documentation, items);
		TryGetGroups(group.Groups, items);
		TryGetFlags(engine.Settings, group.SharedFlags, items);
		TryGetCommands(group.Commands, items);

		PrintItems(items);
	}

	/// <inheritdoc/>
	public void Print(ICommandEngine engine, ICommandInfo command)
	{
		List<IRenderable> items = [];

		TryGetDocumentation(command.Documentation, items);
		TryGetExampleCommand(engine.Settings, command, items);
		TryGetFlags(engine.Settings, command.Flags, items);
		TryGetArguments(command.Arguments, items);

		PrintItems(items);
	}

	private static void TryGetProjectInfo(IEngineSettings settings, List<IRenderable> items)
	{
		if (settings.Name is null && settings.Description is null && settings.Version is null)
			return;

		string? version = settings.Version;
		if (version is not null && version.Length > 0 && char.IsDigit(version[0]))
			version = "v" + version;

		string header = (settings.Name, version) switch
		{
			(string name, string) => $"{name.EscapeMarkup()} - {version.EscapeMarkup()}",
			(null, string) => $"program - {version.EscapeMarkup()}",
			(string name, null) => name.EscapeMarkup(),

			_ => "program",
		};

		string descriptionText = settings.Description?.EscapeMarkup() ?? "No description.";
		Markup description = new(descriptionText.PadRight(header.Length + 2));

		IRenderable container = GetContainer(header, description);

		items.Add(container);
	}
	private static void TryGetDocumentation(IDocumentationInfo? info, List<IRenderable> items)
	{
		if (info is null)
			return;

		TryGetDocumentation("Summary", info.Summary, items);
		TryGetDocumentation("Remarks", info.Remarks, items);
	}
	private static void TryGetDocumentation(string header, IDocumentationNode? node, List<IRenderable> items)
	{
		if (node is null)
			return;

		IRenderable content = GetDocumentation(node);
		IRenderable container = GetContainer(header, content);

		items.Add(container);
	}

	private static void TryGetExampleCommand(IEngineSettings settings, ICommandInfo command, List<IRenderable> items)
	{
		IFlagInfo[] required = [.. command.Flags.Where(f => f.ValueInfo.IsRequired)];
		IFlagInfo[] chained = settings.MergeLongAndShortFlags ? [] : [.. required.Where(f => f.Kind is FlagKind.Toggle or FlagKind.Repeat && f.ShortName is not null)];
		IFlagInfo[] full = [.. required.Except(chained)];
		IArgumentInfo[] arguments = [.. command.Arguments.Where(arg => arg.ValueInfo.IsRequired)];

		List<string> parts = [];

		ICommandGroupInfo? parent = command.Group;
		while (parent is not null && parent.Name is not null)
		{
			parts.Insert(0, $"[{GroupNameStyle}]{parent.Name}[/]");
			parent = parent.Parent;
		}

		if (command.Name is not null)
			parts.Add($"[{CommandNameStyle}]{command.Name.EscapeMarkup()}[/]");

		string shortPrefix = $"[{FlagPrefixStyle}]{settings.ShortFlagPrefix.EscapeMarkup()}[/]";
		string longPrefix = $"[{FlagPrefixStyle}]{settings.LongFlagPrefix.EscapeMarkup()}[/]";
		string separator = $"[{SymbolStyle}]{settings.FlagValueSeparators.First()}[/]";

		if (chained.Length > 0)
		{
			string current = shortPrefix;
			foreach (IFlagInfo flag in chained)
			{
				string name = $"{flag.ShortName}".EscapeMarkup();
				current += $"[{FlagNameStyle}]{name}[/]";
			}

			parts.Add(current);
		}

		foreach (IFlagInfo flag in full)
		{
			string current = flag.LongName is not null ? longPrefix : shortPrefix;
			string? name = flag.LongName is not null ? flag.LongName : $"{flag.ShortName}";
			Debug.Assert(name is not null);

			current += $"[{FlagNameStyle}]{name.EscapeMarkup()}[/]";

			if (flag.Kind is FlagKind.Regular)
			{
				string value = flag.LongName is not null ? flag.LongName.EscapeMarkup() : "value";
				current += separator;
				current += $"[gray]<[/][{FlagValueStyle}]{value}[/][gray]>[/]";
			}

			parts.Add(current);
		}

		foreach (IArgumentInfo argument in arguments)
			parts.Add($"[gray]<[/][{ArgumentStyle}]{argument.Name.EscapeMarkup()}[/][gray]>[/]");

		string text = string.Join(' ', parts);
		Markup markup = new(text);

		IRenderable container = GetContainer("Minimal example", markup);
		items.Add(container);
	}
	private static void TryGetGroups(IReadOnlyDictionary<string, ICommandGroupInfo> groups, List<IRenderable> items)
	{
		if (groups.Count is 0)
			return;

		Table table = GetTable();
		IRenderable container = GetContainer("Groups", table);
		items.Add(container);

		table.AddColumns([
			GetTableColumn("name").LeftAligned(),
			GetTableColumn("summary").LeftAligned()
		]);

		foreach (KeyValuePair<string, ICommandGroupInfo> group in groups)
		{
			Markup name = GetGroupName(group.Key);
			Markup summary = GetDocumentation(group.Value.Documentation?.Summary);

			table.AddRow(name, summary);
		}
	}
	private static void TryGetCommands(IReadOnlyDictionary<string, ICommandInfo> commands, List<IRenderable> items)
	{
		if (commands.Count is 0)
			return;

		Table table = GetTable();
		IRenderable container = GetContainer("Commands", table);
		items.Add(container);

		table.AddColumns([
			GetTableColumn("name").LeftAligned(),
			GetTableColumn("summary").LeftAligned()
		]);

		foreach (KeyValuePair<string, ICommandInfo> command in commands)
		{
			Markup name = GetCommandName(command.Key);
			Markup summary = GetDocumentation(command.Value.Documentation?.Summary);

			table.AddRow(name, summary);
		}
	}
	private static void TryGetFlags(IEngineSettings settings, IReadOnlyCollection<IFlagInfo> flags, List<IRenderable> items)
	{
		if (flags.Count is 0)
			return;

		Table table = GetTable();
		IRenderable container = GetContainer("Flags", table);
		items.Add(container);

		table.AddColumns([
			GetTableColumn("short").LeftAligned(),
			GetTableColumn("long").LeftAligned(),
			GetTableColumn("required?").Centered(),
			GetTableColumn("default").Centered(),
			GetTableColumn("summary").LeftAligned()
		]);

		foreach (IFlagInfo flag in flags)
		{
			IRenderable shortName = GetShortFlag(settings, flag);
			IRenderable longName = GetLongFlag(settings, flag);
			IRenderable isRequired = GetIsRequired(flag.ValueInfo.IsRequired);
			IRenderable defaultValueLabel = GetDefaultLabel(flag.DefaultValueInfo);
			IRenderable summary = GetDocumentation(flag.Documentation?.Summary);

			table.AddRow(shortName, longName, isRequired, defaultValueLabel, summary);
		}
	}
	private static void TryGetArguments(IReadOnlyList<IArgumentInfo> arguments, List<IRenderable> items)
	{
		if (arguments.Count is 0)
			return;

		Table table = GetTable();
		IRenderable container = GetContainer("Arguments", table);
		items.Add(container);

		table.AddColumns([
			GetTableColumn("name").LeftAligned(),
			GetTableColumn("required?").Centered(),
			GetTableColumn("default").Centered(),
			GetTableColumn("summary").LeftAligned()
		]);

		foreach (IArgumentInfo argument in arguments)
		{
			IRenderable name = GetArgumentName(argument.Name);
			IRenderable isRequired = GetIsRequired(argument.ValueInfo.IsRequired);
			IRenderable defaultValueLabel = GetDefaultLabel(argument.DefaultValueInfo);
			IRenderable summary = GetDocumentation(argument.Documentation?.Summary);

			table.AddRow(name, isRequired, defaultValueLabel, summary);
		}
	}
	private static void PrintItems(IEnumerable<IRenderable> items)
	{
		List<IRenderable> actualItems = [];

		foreach (IRenderable item in items)
		{
			if (actualItems.Count > 0)
				actualItems.Add(Text.Empty);

			actualItems.Add(item);
		}

		Rows rows = new(actualItems);
		AnsiConsole.Write(rows);
	}
	#endregion

	#region Format helpers
	private static Table GetTable()
	{
		Table table = new();

		table.SimpleHeavyBorder();

		return table;
	}
	private static TableColumn GetTableColumn(string header)
	{
		string escaped = header.EscapeMarkup();
		IRenderable markup = new Markup($"[bold white]{escaped}[/]");
		return new(markup);
	}
	private static IRenderable GetContainer(string header, IRenderable content)
	{
		string escaped = $"[{header}]".EscapeMarkup();

		Panel panel = new(content)
		{
			Header = new($"[{HeaderStyle}]{escaped}[/]")
		};

		panel.RoundedBorder();

		return panel;
	}
	private static Markup GetDefaultLabel(IDefaultValueInfo? info)
	{
		if (info is null)
			return new("");

		string escaped = info.Label.EscapeMarkup();
		return new($"[{DefaultLabelStyle}]{escaped}[/]");
	}
	private static Markup GetArgumentName(string name)
	{
		string escaped = name.EscapeMarkup();
		return new($"[{ArgumentStyle}]{escaped}[/]");
	}
	private static Markup GetCommandName(string? name)
	{
		if (name is null)
			return new("");

		string escaped = name.EscapeMarkup();
		return new($"[{CommandNameStyle}]{escaped}[/]");
	}
	private static Markup GetGroupName(string? name)
	{
		if (name is null)
			return new("");

		string escaped = name.EscapeMarkup();
		return new($"[{GroupNameStyle}]{escaped}[/]");
	}
	private static Markup GetIsRequired(bool isRequired)
	{
		if (isRequired)
			return new($"[{RequiredStyle}]required[/]");

		return new($"[{OptionalStyle}]optional[/]");
	}
	private static Markup GetShortFlag(IEngineSettings settings, IFlagInfo flag)
	{
		if (flag.ShortName is null)
			return new("");

		string prefix = settings.ShortFlagPrefix.EscapeMarkup();
		string name = $"{flag.ShortName}".EscapeMarkup();

		return new Markup($"[{FlagPrefixStyle}]{prefix}[/][{FlagNameStyle}]{name}[/]");
	}
	private static Markup GetLongFlag(IEngineSettings settings, IFlagInfo flag)
	{
		if (flag.LongName is null)
			return new("");

		string prefix = settings.LongFlagPrefix.EscapeMarkup();
		string name = flag.LongName.EscapeMarkup();

		return new Markup($"[{FlagPrefixStyle}]{prefix}[/][{FlagNameStyle}]{name}[/]");
	}
	private static Markup GetDocumentation(IDocumentationNode? node)
	{
		if (node is null)
			return new("");

		if (node is ITextDocumentationNode rootText)
		{
			string escaped = rootText.Text.EscapeMarkup();
			return new(escaped);
		}

		if (node is IDocumentationNodeCollection collection)
		{
			StringBuilder builder = new();

			foreach (IDocumentationNode child in collection.Children)
			{
				if (child is ITextDocumentationNode text) builder.Append(text.Text);
			}

			string fullText = builder.ToString();
			string escaped = fullText.EscapeMarkup();

			return new(escaped);
		}

		return new("");
	}
	#endregion
}
