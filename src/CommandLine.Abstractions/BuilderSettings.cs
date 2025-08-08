namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents the customisable <see cref="ICommandEngine"/> settings.
/// </summary>
public sealed class BuilderSettings : IEngineSettings
{
	#region Properties
	/// <inheritdoc/>
	public bool AllowFlagShadowing { get; set; } = false;

	/// <inheritdoc/>
	public string LongFlagPrefix { get; set; } = "--";

	/// <inheritdoc/>
	public string ShortFlagPrefix { get; set; } = "-";

	/// <inheritdoc/>
	public bool MergeLongAndShortFlags { get; set; } = false;

	/// <inheritdoc/>
	public bool IncludeHelpFlag { get; set; } = true;

	/// <inheritdoc/>
	public string? LongHelpFlagName { get; set; } = "help";

	/// <inheritdoc/>
	public char? ShortHelpFlagName { get; set; } = 'h';

	/// <inheritdoc/>
	public bool IncludeHelpCommand { get; set; } = true;

	/// <inheritdoc/>
	public string HelpCommandName { get; set; } = "help";

	/// <inheritdoc/>
	public HashSet<string> FlagValueSeparators { get; } = [":", "=", " "];

	IReadOnlyCollection<string> IEngineSettings.FlagValueSeparators => FlagValueSeparators;
	#endregion

	#region Methods
	/// <summary>Sets the <see cref="MergeLongAndShortFlags"/> setting to <see langword="true"/>.</summary>
	/// <param name="prefix">The prefix to use for both long and short flags.</param>
	/// <returns>The used builder instance.</returns>
	public BuilderSettings WithMergedFlagsPrefix(string prefix)
	{
		MergeLongAndShortFlags = true;
		LongFlagPrefix = prefix;
		ShortFlagPrefix = prefix;

		return this;
	}

	/// <summary>Sets the <see cref="MergeLongAndShortFlags"/> setting to <see langword="false"/>.</summary>
	/// <param name="longFlagPrefix">The prefix to use for long style flags.</param>
	/// <param name="shortFlagPrefix">The prefix to use for short style flags.</param>
	/// <returns>The used builder instance.</returns>
	/// <exception cref="ArgumentException">
	/// 	Thrown if either the given <paramref name="longFlagPrefix"/> or the
	/// 	<paramref name="shortFlagPrefix"/> are invalid, or they are equivalent.
	/// </exception>
	public BuilderSettings WithSeparateFlagPrefixes(string longFlagPrefix, string shortFlagPrefix)
	{
		longFlagPrefix.ThrowIfNullOrEmptyOrWhitespace(nameof(longFlagPrefix));
		shortFlagPrefix.ThrowIfNullOrEmptyOrWhitespace(nameof(shortFlagPrefix));

		if (longFlagPrefix == shortFlagPrefix)
			Throw.New.ArgumentException(nameof(shortFlagPrefix), "The short flag prefix should be different from the long flag prefix.");

		MergeLongAndShortFlags = false;
		LongFlagPrefix = longFlagPrefix;
		ShortFlagPrefix = shortFlagPrefix;

		return this;
	}

	/// <summary>Sets the <see cref="IncludeHelpFlag"/> setting to <see langword="true"/>.</summary>
	/// <param name="longName">The long name for the help flag.</param>
	/// <param name="shortName">The short name for the help flag.</param>
	/// <returns>The used settings instance.</returns>
	/// <exception cref="ArgumentException">Thrown if both the given <paramref name="longName"/> and <paramref name="shortName"/> have invalid values.</exception>
	public BuilderSettings WithHelpFlag(string? longName, char? shortName)
	{
		if (string.IsNullOrWhiteSpace(longName) && shortName is null)
			Throw.New.ArgumentException(nameof(longName), $"When including the help flag, either the long name or the short name must be provided.");

		IncludeHelpFlag = true;
		LongHelpFlagName = longName;
		ShortHelpFlagName = shortName;

		return this;
	}

	/// <summary>Sets the <see cref="IncludeHelpFlag"/> setting to <see langword="false"/>.</summary>
	/// <returns>The used settings instance.</returns>
	public BuilderSettings WithoutHelpFlag()
	{
		IncludeHelpFlag = false;
		return this;
	}

	/// <summary>Sets the <see cref="IncludeHelpCommand"/> setting to <see langword="true"/>.</summary>
	/// <param name="commandName">The name of the help command.</param>
	/// <returns>The used builder instance.</returns>
	/// <exception cref="ArgumentException">Thrown if the given <paramref name="commandName"/> is invalid.</exception>
	public BuilderSettings WithHelpCommand(string commandName)
	{
		commandName.ThrowIfNullOrEmptyOrWhitespace(nameof(commandName));

		IncludeHelpCommand = true;
		HelpCommandName = commandName;

		return this;
	}


	/// <summary>Sets the <see cref="IncludeHelpCommand"/> setting to <see langword="false"/>.</summary>
	/// <returns>The used settings instance.</returns>
	public BuilderSettings WithoutHelpCommand()
	{
		IncludeHelpCommand = false;
		return this;
	}

	/// <summary>Sets the <see cref="FlagValueSeparators"/> setting.</summary>
	/// <param name="separators">The separators which can be used to separate the flag name from the flag value.</param>
	/// <returns>The used builder instance.</returns>
	/// <remarks>The white-space character <c> </c> has the special meaning of allowing any white-space characters to be used as a separator.</remarks>
	public BuilderSettings WithFlagValueSeparators(params ReadOnlySpan<string> separators)
	{
		FlagValueSeparators.Clear();

		foreach (string sep in separators)
			FlagValueSeparators.Add(sep);

		return this;
	}
	#endregion
}
