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

	/// <inheritdoc/>
	public required IReadOnlyCollection<string> FlagValueSeparators { get; init; }
	#endregion

	#region Functions
	/// <summary>Validates the given <paramref name="settings"/> and creates a copy of them.</summary>
	/// <param name="settings">The settings to validate and copy.</param>
	/// <returns>The validated copy of the given <paramref name="settings"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if the given settings are invalid.</exception>
	public static IEngineSettings From(IEngineSettings settings)
	{
		List<string> errors = [];

		if (settings.LongFlagPrefix == settings.ShortFlagPrefix && (settings.MergeLongAndShortFlags is false))
			errors.Add($"The {nameof(LongFlagPrefix)} and {nameof(ShortFlagPrefix)} settings had the same value, but the {settings.MergeLongAndShortFlags} setting was false.");

		if (settings.IncludeHelpCommand && string.IsNullOrWhiteSpace(settings.HelpCommandName))
			errors.Add($"{nameof(IncludeHelpCommand)} setting was set to true, but the {nameof(HelpCommandName)} setting ({settings.HelpCommandName}) was invalid.");

		if (settings.IncludeHelpFlag && string.IsNullOrWhiteSpace(settings.LongHelpFlagName) && settings.ShortHelpFlagName is null)
			errors.Add($"The {nameof(IncludeHelpFlag)} settings was set set to true, but both the {nameof(LongHelpFlagName)} ({settings.LongHelpFlagName}) and {nameof(ShortHelpFlagName)} ({settings.ShortHelpFlagName}) settings had invalid values.");

		if (settings.FlagValueSeparators.Count is 0)
			errors.Add($"The {nameof(FlagValueSeparators)} must have at least one separator.");

		if (errors.Count > 0)
			Throw.New.ArgumentException(nameof(settings), string.Join(Environment.NewLine, errors));

		return new EngineSettings()
		{
			AllowFlagShadowing = settings.AllowFlagShadowing,
			LongFlagPrefix = settings.LongFlagPrefix,
			ShortFlagPrefix = settings.ShortFlagPrefix,
			MergeLongAndShortFlags = settings.MergeLongAndShortFlags,
			IncludeHelpFlag = settings.IncludeHelpFlag,
			LongHelpFlagName = settings.LongHelpFlagName,
			ShortHelpFlagName = settings.ShortHelpFlagName,
			IncludeHelpCommand = settings.IncludeHelpCommand,
			HelpCommandName = settings.HelpCommandName,
			FlagValueSeparators = [.. settings.FlagValueSeparators.OrderByDescending(s => s.Length)],
		};
	}
	#endregion
}
