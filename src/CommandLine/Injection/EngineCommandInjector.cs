namespace OwlDomain.CommandLine.Injection;

/// <summary>
/// 	Represents an injector for engine specific values.
/// </summary>
public sealed class EngineCommandInjector : BaseCommandInjector
{
	#region Methods
	/// <inheritdoc/>
	protected override bool CanInject(Type type)
	{
		bool canInject =
			type == typeof(ICommandExecutionContext) ||
			type == typeof(ICommandEngine) ||
			type == typeof(IEngineSettings) ||
			type == typeof(IRootValueParserSelector) ||

			type == typeof(ICommandParser) ||
			type == typeof(ICommandValidator) ||
			type == typeof(ICommandExecutor) ||
			type == typeof(IDocumentationPrinter) ||

			type == typeof(IVirtualCommands) ||
			type == typeof(IVirtualFlags) ||

			type == typeof(ICommandParserResult) ||
			type == typeof(ICommandValidatorResult) ||

			type == typeof(ICommandInfo) ||
			type == typeof(ICommandGroupInfo) ||

			type == typeof(DiagnosticBag);

		return canInject;
	}

	/// <inheritdoc/>
	protected override bool TryInject(ICommandExecutionContext context, Type type, out object? value)
	{
		value = GetValue(context, type);
		return value is not null;
	}

	private static object? GetValue(ICommandExecutionContext context, Type type)
	{
		if (type == typeof(ICommandExecutionContext)) return context;
		if (type == typeof(ICommandEngine)) return context.Engine;
		if (type == typeof(IEngineSettings)) return context.Engine.Settings;
		if (type == typeof(IRootValueParserSelector)) return context.Engine.ValueParserSelector;

		if (type == typeof(ICommandParser)) return context.Engine.Parser;
		if (type == typeof(ICommandValidator)) return context.Engine.Validator;
		if (type == typeof(ICommandExecutor)) return context.Engine.Executor;
		if (type == typeof(IDocumentationPrinter)) return context.Engine.DocumentationPrinter;

		if (type == typeof(IVirtualCommands)) return context.Engine.VirtualCommands;
		if (type == typeof(IVirtualFlags)) return context.Engine.VirtualFlags;

		if (type == typeof(ICommandParserResult)) return context.ValidatorResult.ParserResult;
		if (type == typeof(ICommandValidatorResult)) return context.ValidatorResult;

		if (type == typeof(ICommandInfo))
		{
			Debug.Assert(context.CommandTarget is not null, "There should always be a command target if the execution of an actual command is going to happen.");
			return context.CommandTarget;
		}
		if (type == typeof(ICommandGroupInfo)) return context.GroupTarget;

		if (type == typeof(DiagnosticBag)) return context.Diagnostics;

		return default;
	}
	#endregion
}
