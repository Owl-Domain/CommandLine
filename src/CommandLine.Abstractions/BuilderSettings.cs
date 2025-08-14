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

	/// <inheritdoc/>
	public string ListPrefix { get; set; } = "[";

	/// <inheritdoc/>
	public string ListSuffix { get; set; } = "]";

	/// <inheritdoc/>
	public string ListValueSeparator { get; set; } = ",";

	/// <inheritdoc/>
	public TimeSpan ParsingTimeout { get; set; } = TimeSpan.Zero;

	/// <inheritdoc/>
	public TimeSpan ValidationTimeout { get; set; } = TimeSpan.Zero;

	/// <inheritdoc/>
	public TimeSpan ExecutionTimeout { get; set; } = TimeSpan.Zero;

	/// <inheritdoc/>
	public string FlagArgumentSeparator { get; set; } = "--";
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

	/// <summary>Sets the <see cref="ListPrefix"/> and <see cref="ListSuffix"/> settings.</summary>
	/// <param name="prefix">The prefix used to start a list.</param>
	/// <param name="suffix">The suffix used to end a list.</param>
	/// <returns>The used builder instance.</returns>
	public BuilderSettings WithListSymbols(string prefix, string suffix)
	{
		ListPrefix = prefix;
		ListSuffix = suffix;
		return this;
	}

	/// <summary>Sets the <see cref="ListValueSeparator"/> setting.</summary>
	/// <param name="separator">The separator that will be used to separate list values.</param>
	/// <returns>The used builder instance.</returns>
	public BuilderSettings WithListSeparator(string separator)
	{
		ListValueSeparator = separator;
		return this;
	}

	/// <summary>Sets the <see cref="ParsingTimeout"/> setting.</summary>
	/// <param name="duration">The amount of time that command parsing is allowed to take.</param>
	/// <returns>The used builder instance.</returns>
	/// <remarks>This setting is merely a suggestion and it's up to the individual parsers to respect it.</remarks>
	public BuilderSettings WithParsingTimeout(TimeSpan duration)
	{
		duration.ThrowIfLessThan(TimeSpan.Zero, nameof(duration));

		ParsingTimeout = duration;
		return this;
	}

	/// <summary>Sets the <see cref="ValidationTimeout"/> setting.</summary>
	/// <param name="duration">The amount of time that command validation is allowed to take.</param>
	/// <returns>The used builder instance.</returns>
	/// <remarks>This setting is merely a suggestion and it's up to the individual validators to respect it.</remarks>
	public BuilderSettings WithValidationTimeout(TimeSpan duration)
	{
		duration.ThrowIfLessThan(TimeSpan.Zero, nameof(duration));

		ValidationTimeout = duration;
		return this;
	}

	/// <summary>Sets the <see cref="ExecutionTimeout"/> setting.</summary>
	/// <param name="duration">The amount of time that command execution is allowed to take.</param>
	/// <returns>The used builder instance.</returns>
	/// <remarks>Once the actual command is being executed, this value is merely a suggestion that the command itself needs to respect.</remarks>
	public BuilderSettings WithExecutionTimeout(TimeSpan duration)
	{
		duration.ThrowIfLessThan(TimeSpan.Zero, nameof(duration));

		ExecutionTimeout = duration;
		return this;
	}

	/// <summary>Sets the <see cref="FlagArgumentSeparator"/> setting.</summary>
	/// <param name="separator">The separator to use when marking the end of flags, where everything aftwards will be parsed as arguments.</param>
	/// <returns>The used builder instance.</returns>
	/// <exception cref="ArgumentException">Thrown if the given <paramref name="separator"/> is empty or only consists of white-space characters.</exception>
	public BuilderSettings WithFlagArgumentSeparator(string separator)
	{
		separator.ThrowIfEmptyOrWhitespace(nameof(separator));

		FlagArgumentSeparator = separator;
		return this;
	}

	/// <summary>Configures the settings to enable Windows styled syntax.</summary>
	/// <param name="onlyOnWindows">Whether these settings should only be set when running on windows.</param>
	/// <returns>The used builder instance.</returns>
	/// <remarks>
	/// 	This setting means:
	/// 	<list type="bullet">
	/// 		<item>The <see cref="MergeLongAndShortFlags"/> setting is set to <see langword="true"/>.</item>
	/// 		<item>Both <see cref="ShortFlagPrefix"/> and <see cref="LongFlagPrefix"/> is set to <c>/</c>.</item>
	/// 		<item>The short help flag name is set to <c>?</c>.</item>
	/// 	</list>
	/// </remarks>
	public BuilderSettings WithWindowsStyleSyntax(bool onlyOnWindows = true)
	{
		if (onlyOnWindows)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) is false)
				return this;
		}

		WithMergedFlagsPrefix("/");
		ShortHelpFlagName = '?';

		return this;
	}
	#endregion
}
