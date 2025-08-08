namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents the settings used for the <see cref="ICommandEngine"/>.
/// </summary>
public sealed class EngineSettings : IEngineSettings
{
	#region Properties
	/// <inheritdoc/>
	public required bool AllowFlagShadowing { get; init; }

	/// <inheritdoc/>
	public required string LongFlagPrefix { get; init; }

	/// <inheritdoc/>
	public required string ShortFlagPrefix { get; init; }

	/// <inheritdoc/>
	public required bool MergeLongAndShortFlags { get; init; }

	/// <inheritdoc/>
	public required bool IncludeHelpFlag { get; init; }

	/// <inheritdoc/>
	public required string? LongHelpFlagName { get; init; }

	/// <inheritdoc/>
	public required char? ShortHelpFlagName { get; init; }

	/// <inheritdoc/>
	public required bool IncludeHelpCommand { get; init; }

	/// <inheritdoc/>
	public required string HelpCommandName { get; init; }
	#endregion
}
