using System.IO;
using System.Text.Json.Nodes;
using Spectre.Console;
using Spectre.Console.Json;

namespace OwlDomain.CommandLine.Output;

/// <summary>
/// 	Represents a command output printer.
/// </summary>
public sealed class OutputPrinter : IOutputPrinter
{
	#region Methods
	/// <inheritdoc/>
	public void Print(ICommandRunResult result)
	{
		if (result.WasCancelled)
			return;

		if (result.Successful)
		{
			PrintValue(result.ExecutorResult.Engine, result.ExecutorResult.Result);
			return;
		}

		PrintErrors(result.Diagnostics);
	}

	private static void PrintValue(ICommandEngine engine, object? result)
	{
		if (result is null)
			return;

		if (TryPrintSpecialValue(result))
			return;

		string? text = result switch
		{
			byte value => value.ToString(engine.Settings.NumberFormat),
			ushort value => value.ToString(engine.Settings.NumberFormat),
			uint value => value.ToString(engine.Settings.NumberFormat),
			ulong value => value.ToString(engine.Settings.NumberFormat),

			sbyte value => value.ToString(engine.Settings.NumberFormat),
			short value => value.ToString(engine.Settings.NumberFormat),
			int value => value.ToString(engine.Settings.NumberFormat),
			long value => value.ToString(engine.Settings.NumberFormat),

			Half value => value.ToString(engine.Settings.NumberFormat),
			float value => value.ToString(engine.Settings.NumberFormat),
			double value => value.ToString(engine.Settings.NumberFormat),
			decimal value => value.ToString(engine.Settings.NumberFormat),

			_ => result.ToString(),
		};

		Console.WriteLine(text);
	}
	private static bool TryPrintSpecialValue(object? value)
	{
		IAnsiConsole console = AnsiConsole.Console;

		if (value is Exception exception)
		{
			console.WriteException(exception);
			console.WriteLine();

			return true;
		}

		if (value is JsonNode node)
		{
			JsonText json = new(node.ToJsonString());

			console.Write(json);
			console.WriteLine();

			return true;
		}

		if (value is FileSystemInfo path)
		{
			TextPath markup = new(path.FullName);

			console.Write(markup);
			console.WriteLine();

			return true;
		}

		return false;
	}
	private static void PrintErrors(IDiagnosticBag diagnostics)
	{
		IAnsiConsole console = AnsiConsole.Console;

		Table table = new();

		table
			.SimpleHeavyBorder()
			.BorderColor(Color.Red);

		table.AddColumns(
			new TableColumn(new Markup("stage", "bold red")),
			new TableColumn(new Markup("location", "bold red")),
			new TableColumn(new Markup("message", "bold red")));

		foreach (IDiagnostic diagnostic in diagnostics)
		{
			string source = diagnostic.Source.ToString().ToLower();
			string location = diagnostic.Location.ToString();
			string message = diagnostic.Message;

			table.AddRow(source, location, message);
		}

		console.Write(table);
		console.WriteLine();
	}
	#endregion
}
