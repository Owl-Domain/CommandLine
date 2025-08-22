using System.Text;
using OwlDomain.Documentation;
using OwlDomain.Documentation.Document.Nodes;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace OwlDomain.CommandLine.Documentation;

/// <summary>
/// 	Represents a printer for documentation info.
/// </summary>
public sealed class DocumentationPrinter : IDocumentationPrinter
{
	#region Nested types
	private sealed class Lookup : Dictionary<string, object>
	{
		#region Fields
		private static readonly DocumentationIdGenerator Generator = new();
		#endregion

		#region Properties
		public ICommandGroupInfo Group { get; }
		#endregion

		#region Constructors
		public Lookup(ICommandGroupInfo group)
		{
			Group = group;
			Populate(group);
		}
		#endregion

		#region Methods
		private void Populate(ICommandGroupInfo group)
		{
			Populate(group.SharedFlags);
			Populate(group.Groups.Values);
			Populate(group.Commands.Values);

			if (group.ImplicitCommand is not null)
				Populate(group.ImplicitCommand);
		}
		private void Populate(ICommandInfo command)
		{
			Populate(command.Flags);

			if (command is IMethodCommandInfo method)
				Populate(method.Method, command);
		}
		private void Populate(IFlagInfo flag)
		{
			if (flag is IPropertyFlagInfo property)
				Populate(property.Property, flag);
		}
		#endregion

		#region Helpers
		private void Populate(PropertyInfo property, object target)
		{
			string id = Generator.Get(property);
			TryAdd(id, target);
		}
		private void Populate(MethodInfo method, object target)
		{
			string id = Generator.Get(method);
			TryAdd(id, target);
		}
		private void Populate(IEnumerable<ICommandGroupInfo> groups)
		{
			foreach (ICommandGroupInfo group in groups)
				Populate(group);
		}
		private void Populate(IEnumerable<ICommandInfo> commands)
		{
			foreach (ICommandInfo command in commands)
				Populate(command);
		}
		private void Populate(IEnumerable<IFlagInfo> flags)
		{
			foreach (IFlagInfo flag in flags)
				Populate(flag);
		}
		#endregion
	}
	private sealed class ParameterLookup : Dictionary<string, object>
	{
		#region Properties
		public ICommandInfo Command { get; }
		#endregion

		#region Constructors
		public ParameterLookup(ICommandInfo command)
		{
			Command = command;
			Populate(command);
		}
		#endregion

		#region Methods
		private void Populate(ICommandInfo command)
		{
			Populate(command.Flags);
			Populate(command.Arguments);
		}
		private void Populate(IFlagInfo flag)
		{
			if (flag is IParameterFlagInfo parameter)
				Populate(parameter.Parameter, flag);
		}
		private void Populate(IArgumentInfo argument)
		{
			if (argument is IParameterArgumentInfo parameter)
				Populate(parameter.Parameter, argument);
		}
		#endregion

		#region Helpers
		private void Populate(ParameterInfo parameter, object target)
		{
			if (parameter.Name is not null)
				TryAdd(parameter.Name, target);
		}
		private void Populate(IEnumerable<IFlagInfo> flags)
		{
			foreach (IFlagInfo flag in flags)
				Populate(flag);
		}
		private void Populate(IEnumerable<IArgumentInfo> arguments)
		{
			foreach (IArgumentInfo argument in arguments)
				Populate(argument);
		}
		#endregion
	}
	private class Context(ICommandEngine engine, Lookup lookup)
	{
		#region Properties
		public ICommandEngine Engine { get; } = engine;
		public IEngineSettings Settings { get; } = engine.Settings;
		public Lookup Lookup { get; } = lookup;
		#endregion

		#region Constructors
		public Context(ICommandEngine engine) : this(engine, new(engine.RootGroup)) { }
		#endregion
	}
	private class GroupContext(Context context, ICommandGroupInfo group) : Context(context.Engine, context.Lookup)
	{
		#region Fields
		public ICommandGroupInfo Group { get; } = group;
		#endregion
	}
	private class CommandContext(Context context, ICommandInfo command) : Context(context.Engine, context.Lookup)
	{
		#region Fields
		public ICommandGroupInfo? Group { get; } = command.Group;
		public ICommandInfo Command { get; } = command;
		public ParameterLookup Parameters { get; } = new(command);
		#endregion
	}
	#endregion

	#region Constants
	private const string ColumnHeaderStyle = "bold";
	private const string HeaderStyle = "bold italic";
	private const string GroupStyle = "blue";
	private const string CommandStyle = "yellow";
	private const string ArgumentStyle = "cyan";
	private const string FlagStyle = "fuchsia";
	private const string RequiredStyle = "bold";
	private const string OptionalStyle = "italic";
	#endregion

	#region Methods
	/// <inheritdoc/>
	public void Print(ICommandEngine engine)
	{
		Context context = new(engine);
		List<IRenderable> items = [];

		TryGetProjectInfo(context, items);
		TryGetGroups(context, engine.RootGroup.Groups, items);
		TryGetFlags(context, engine.RootGroup.SharedFlags, items);
		TryGetCommands(context, engine.RootGroup.Commands, items);

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

		GroupContext context = new(new(engine), group);
		List<IRenderable> items = [];

		TryGetDocumentation(context, group.Documentation, items);
		TryGetGroups(context, group.Groups, items);
		TryGetFlags(context, group.SharedFlags, items);
		TryGetCommands(context, group.Commands, items);

		PrintItems(items);
	}

	/// <inheritdoc/>
	public void Print(ICommandEngine engine, ICommandInfo command)
	{
		CommandContext context = new(new(engine), command);
		List<IRenderable> items = [];

		TryGetDocumentation(context, command.Documentation, items);
		TryGetExampleCommand(context, items);
		TryGetFlags(context, command.Flags, items);
		TryGetArguments(context, command.Arguments, items);

		PrintItems(items);
	}

	private static void TryGetProjectInfo(Context context, List<IRenderable> items)
	{
		if (context.Settings.Name is null && context.Settings.Description is null && context.Settings.Version is null)
			return;

		string? version = context.Settings.Version;
		if (version is not null && version.Length > 0 && char.IsDigit(version[0]))
			version = "v" + version;

		string header = (context.Settings.Name, version) switch
		{
			(string name, string) => $"{name.EscapeMarkup()} - {version.EscapeMarkup()}",
			(null, string) => $"program - {version.EscapeMarkup()}",
			(string name, null) => name.EscapeMarkup(),

			_ => "program",
		};

		string descriptionText = context.Settings.Description?.EscapeMarkup() ?? "No description.";
		Markup description = new(descriptionText.PadRight(header.Length + 2));

		IRenderable container = GetContainer(header, description);

		items.Add(container);
	}
	private static void TryGetDocumentation(Context context, IDocumentationInfo? info, List<IRenderable> items)
	{
		if (info is null)
			return;

		TryGetDocumentation(context, "Summary", info.Summary, items);
		TryGetDocumentation(context, "Remarks", info.Remarks, items);
		TryGetDocumentation(context, "Example", info.Example, items);
		TryGetDocumentation(context, "Returns", info.Returns, items);
	}
	private static void TryGetDocumentation(Context context, string header, IDocumentationNode? node, List<IRenderable> items)
	{
		if (node is null)
			return;

		IRenderable content = GetDocumentation(context, node);
		IRenderable container = GetContainer(header, content);

		items.Add(container);
	}

	private static void TryGetExampleCommand(CommandContext context, List<IRenderable> items)
	{
		IFlagInfo[] required = [.. context.Command.Flags.Where(f => f.ValueInfo.IsRequired)];
		IFlagInfo[] chained = context.Settings.MergeLongAndShortFlags ? [] : [.. required.Where(f => f.Kind is FlagKind.Toggle or FlagKind.Repeat && f.ShortName is not null)];
		IFlagInfo[] full = [.. required.Except(chained)];
		IArgumentInfo[] arguments = [.. context.Command.Arguments.Where(arg => arg.ValueInfo.IsRequired)];

		List<string> parts = [];

		ICommandGroupInfo? parent = context.Command.Group;
		while (parent is not null && parent.Name is not null)
		{
			parts.Insert(0, $"[{GroupStyle}]{parent.Name}[/]");
			parent = parent.Parent;
		}

		if (context.Command.Name is not null)
			parts.Add($"[{CommandStyle}]{context.Command.Name.EscapeMarkup()}[/]");

		string shortPrefix = $"[{FlagStyle}]{context.Settings.ShortFlagPrefix.EscapeMarkup()}[/]";
		string longPrefix = $"[{FlagStyle}]{context.Settings.LongFlagPrefix.EscapeMarkup()}[/]";
		string separator = $"{context.Settings.FlagValueSeparators.First()}";

		if (chained.Length > 0)
		{
			string current = shortPrefix;
			foreach (IFlagInfo flag in chained)
			{
				string name = $"{flag.ShortName}".EscapeMarkup();
				current += $"[{FlagStyle}]{name}[/]";
			}

			parts.Add(current);
		}

		foreach (IFlagInfo flag in full)
		{
			string current = flag.LongName is not null ? longPrefix : shortPrefix;
			string? name = flag.LongName is not null ? flag.LongName : $"{flag.ShortName}";
			Debug.Assert(name is not null);

			current += $"[{FlagStyle}]{name.EscapeMarkup()}[/]";

			if (flag.Kind is FlagKind.Regular)
			{
				string value = flag.LongName is not null ? flag.LongName.EscapeMarkup() : "value";
				current += separator;
				current += $"<[{FlagStyle}]{value}[/]>";
			}

			parts.Add(current);
		}

		foreach (IArgumentInfo argument in arguments)
			parts.Add($"<[{ArgumentStyle}]{argument.Name.EscapeMarkup()}[/]>");

		string text = string.Join(' ', parts);
		Markup markup = new(text);

		IRenderable container = GetContainer("Minimal example", markup);
		items.Add(container);
	}
	private static void TryGetGroups(Context context, IReadOnlyDictionary<string, ICommandGroupInfo> groups, List<IRenderable> items)
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
			GroupContext groupContext = new(context, group.Value);

			Markup name = GetGroupName(groupContext, group.Value);
			Markup summary = GetDocumentation(groupContext, group.Value.Documentation?.Summary);

			table.AddRow(name, summary);
		}
	}
	private static void TryGetCommands(Context context, IReadOnlyDictionary<string, ICommandInfo> commands, List<IRenderable> items)
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
			CommandContext commandContext = new(context, command.Value);

			Markup name = GetCommandName(context, command.Value);
			Markup summary = GetDocumentation(commandContext, command.Value.Documentation?.Summary);

			table.AddRow(name, summary);
		}
	}
	private static void TryGetFlags(Context context, IReadOnlyCollection<IFlagInfo> flags, List<IRenderable> items)
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
			IRenderable shortName = GetShortFlag(context, flag);
			IRenderable longName = GetLongFlag(context, flag);
			IRenderable isRequired = GetIsRequired(flag.ValueInfo.IsRequired);
			IRenderable defaultValueLabel = GetDefaultLabel(flag.DefaultValueInfo);
			IRenderable summary = GetDocumentation(context, flag.Documentation?.Summary);

			table.AddRow(shortName, longName, isRequired, defaultValueLabel, summary);
		}
	}
	private static void TryGetArguments(Context context, IReadOnlyList<IArgumentInfo> arguments, List<IRenderable> items)
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
			IRenderable name = GetArgumentName(context, argument);
			IRenderable isRequired = GetIsRequired(argument.ValueInfo.IsRequired);
			IRenderable defaultValueLabel = GetDefaultLabel(argument.DefaultValueInfo);
			IRenderable summary = GetDocumentation(context, argument.Documentation?.Summary);

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
		AnsiConsole.WriteLine();
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
		IRenderable markup = new Markup($"[{ColumnHeaderStyle}]{escaped}[/]");
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
		return new(escaped);
	}
	private static Markup GetArgumentName(Context context, IArgumentInfo? argument)
	{
		if (argument is null)
			return new("");

		StringBuilder builder = new();
		AddRawArgument(context, builder, argument);

		return new(builder.ToString());
	}
	private static Markup GetCommandName(Context context, ICommandInfo? command)
	{
		if (command is null)
			return new("");

		StringBuilder builder = new();
		AddRawCommand(context, builder, command);

		return new(builder.ToString());
	}
	private static Markup GetGroupName(Context context, ICommandGroupInfo? group)
	{
		if (group is null)
			return new("");

		StringBuilder builder = new();
		AddRawGroup(context, builder, group);

		return new(builder.ToString());
	}
	private static Markup GetIsRequired(bool isRequired)
	{
		if (isRequired)
			return new($"[{RequiredStyle}]required[/]");

		return new($"[{OptionalStyle}]optional[/]");
	}
	private static Markup GetShortFlag(Context context, IFlagInfo flag)
	{
		StringBuilder builder = new();
		AddRawShortFlag(context, builder, flag);

		return new(builder.ToString());
	}
	private static Markup GetLongFlag(Context context, IFlagInfo flag)
	{
		StringBuilder builder = new();
		AddRawLongFlag(context, builder, flag);

		return new(builder.ToString());
	}
	private static Markup GetDocumentation(Context context, IDocumentationNode? node)
	{
		if (node is null)
			return new("");

		StringBuilder builder = new();
		AddMarkupContent(context, builder, node);

		string content = builder.ToString();
		return new(content);
	}
	#endregion

	#region Raw markup helpers
	private static void AddMarkupContent(Context context, StringBuilder builder, IEnumerable<IDocumentationNode> nodes)
	{
		foreach (IDocumentationNode node in nodes)
			AddMarkupContent(context, builder, node);
	}
	private static void AddMarkupContent(Context context, StringBuilder builder, IDocumentationNode? node)
	{
		if (node is null)
			return;

		if (node is ITextDocumentationNode rootText)
		{
			string escaped = rootText.Text.EscapeMarkup();
			builder.Append(escaped);
		}
		else if (node is ILineBreakTagDocumentationNode)
			builder.AppendLine();
		else if (node is IBoldTagDocumentationNode bold)
		{
			builder.Append("[bold]");
			AddMarkupContent(context, builder, bold.Children);
			builder.Append("[/]");
		}
		else if (node is IItalicTagDocumentationNode italic)
		{
			builder.Append("[italic]");
			AddMarkupContent(context, builder, italic.Children);
			builder.Append("[/]");
		}
		else if (node is ITagDocumentationNode tag)
		{
			if (tag.Link is not null)
			{
				if (tag.Children.Count is 0)
				{
					builder
						.Append("[link]")
						.Append(tag.Link.ToString().EscapeMarkup())
						.Append("[/]");
				}
				else
				{
					builder
						.Append("[link=")
						.Append(tag.Link.ToString().EscapeMarkup())
						.Append(']');

					AddMarkupContent(context, builder, tag.Children);
					builder.Append("[/]");
				}
			}
			else if (tag.NameReference is not null)
			{
				AddRawReference(context, builder, tag.NameReference);
				Debug.Assert(tag.Children.Count is 0);
			}
			else if (tag.CodeReference is not null)
			{
				AddRawReference(context, builder, tag.CodeReference);
				Debug.Assert(tag.Children.Count is 0);
			}
			else
				AddMarkupContent(context, builder, tag.Children);
		}
		else if (node is IDocumentationNodeCollection collection)
			AddMarkupContent(context, builder, collection.Children);
	}
	private static void AddRawReference(Context context, StringBuilder builder, string id)
	{
		const string error = "[red]#error[/]";

		if (FindTarget(context, id, out object? target) is false)
		{
			builder.Append(error);
			return;
		}

		if (target is ICommandGroupInfo group && group.Name is not null)
		{
			AddRawParentChain(context, builder, group.Parent);
			AddRawGroup(context, builder, group);

			return;
		}

		if (target is ICommandInfo command && command.Name is not null)
		{
			AddRawParentChain(context, builder, command.Group);
			AddRawCommand(context, builder, command);

			return;
		}

		if (target is IFlagInfo flag)
		{
			AddRawFlag(context, builder, flag);
			return;
		}

		if (target is IArgumentInfo argument)
		{
			AddRawArgument(context, builder, argument);
			return;
		}

		builder.Append(error);
	}
	private static void AddRawGroup(Context context, StringBuilder builder, ICommandGroupInfo group)
	{
		if (group.Name is null)
			return;

		builder
			.Append($"[{GroupStyle}]")
			.Append(group.Name.EscapeMarkup())
			.Append("[/]");
	}
	private static void AddRawCommand(Context context, StringBuilder builder, ICommandInfo command)
	{
		if (command.Name is null)
			return;

		builder
			.Append($"[{CommandStyle}]")
			.Append(command.Name.EscapeMarkup())
			.Append("[/]");
	}
	private static void AddRawFlag(Context context, StringBuilder builder, IFlagInfo flag)
	{
		if (flag.LongName is not null)
			AddRawLongFlag(context, builder, flag);
		else
			AddRawShortFlag(context, builder, flag);
	}
	private static void AddRawLongFlag(Context context, StringBuilder builder, IFlagInfo flag)
	{
		builder
			.Append($"[{FlagStyle}]")
			.Append(context.Settings.LongFlagPrefix)
			.Append(flag.LongName.EscapeMarkup())
			.Append("[/]");
	}
	private static void AddRawShortFlag(Context context, StringBuilder builder, IFlagInfo flag)
	{
		builder
			.Append($"[{FlagStyle}]")
			.Append(context.Settings.ShortFlagPrefix)
			.Append(flag.ShortName.ToString().EscapeMarkup())
			.Append("[/]");
	}
	private static void AddRawArgument(Context context, StringBuilder builder, IArgumentInfo argument)
	{
		builder
			.Append($"[{ArgumentStyle}]")
			.Append(argument.Name.EscapeMarkup())
			.Append("[/]");
	}
	private static void AddRawParentChain(Context context, StringBuilder builder, ICommandGroupInfo? parent)
	{
		List<ICommandGroupInfo> parents = [];

		while (parent?.Name is not null)
		{
			parents.Insert(0, parent);
			parent = parent.Parent;
		}

		foreach (ICommandGroupInfo group in parents)
		{
			AddRawGroup(context, builder, group);
			builder.Append(' ');
		}
	}
	#endregion

	#region Helpers
	private static bool FindTarget(Context context, string key, [NotNullWhen(true)] out object? target)
	{
		if (context is CommandContext commandContext)
		{
			if (commandContext.Parameters.TryGetValue(key, out target))
				return true;
		}

		if (context.Lookup.TryGetValue(key, out target))
			return true;

		return false;
	}
	#endregion
}
