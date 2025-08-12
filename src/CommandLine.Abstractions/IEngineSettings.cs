namespace OwlDomain.CommandLine;

/// <summary>
/// 	Represents the settings used for the <see cref="ICommandEngine"/>.
/// </summary>
public interface IEngineSettings
{
	#region Properties
	/// <summary>Whether flag shadowing is allowed.</summary>
	/// <remarks>
	/// 	This indicates whether the flags from outer command groups can be shadows
	/// 	by different flags from inner command groups when they have the same name.
	/// </remarks>
	bool AllowFlagShadowing { get; }

	/// <summary>The prefix to use for detecting long name flags.</summary>
	/// <remarks>
	/// 	If this is the same as <see cref="ShortFlagPrefix"/> then
	/// 	<see cref="MergeLongAndShortFlags"/> must be set to <see langword="true"/>.
	/// </remarks>
	string LongFlagPrefix { get; }

	/// <summary>The prefix to use for detecting short name flags.</summary>
	/// <remarks>
	/// 	If this is the same as <see cref="LongFlagPrefix"/> then
	/// 	<see cref="MergeLongAndShortFlags"/> must be set to <see langword="true"/>.
	/// </remarks>
	string ShortFlagPrefix { get; }

	/// <summary>Whether long and short flags should have the same prefix.</summary>
	bool MergeLongAndShortFlags { get; }

	/// <summary>Whether a virtual help flag should be included in every command.</summary>
	/// <remarks>
	/// 	If this is set to <see langword="true"/> then either <see cref="LongHelpFlagName"/>
	/// 	or <see cref="ShortHelpFlagName"/> (or both), must have a value.
	/// </remarks>
	bool IncludeHelpFlag { get; }

	/// <summary>The long name for the virtual help flag.</summary>
	/// <remarks><see cref="IncludeHelpFlag"/> must be set to <see langword="true"/> for this setting to matter.</remarks>
	string? LongHelpFlagName { get; }

	/// <summary>The short name for the virtual help flag.</summary>
	/// <remarks><see cref="IncludeHelpFlag"/> must be set to <see langword="true"/> for this setting to matter.</remarks>
	char? ShortHelpFlagName { get; }

	/// <summary>Whether the help command should be included in every command group.</summary>
	bool IncludeHelpCommand { get; }

	/// <summary>The name of the virtual help command.</summary>
	/// <remarks><see cref="IncludeHelpCommand"/> must be set to <see langword="true"/> for this setting to matter.</remarks>
	string HelpCommandName { get; }

	/// <summary>The separators that can be used to separator flag names from flag values.</summary>
	/// <remarks>The white-space character <c> </c> has the special meaning of allowing any white-space to be used as a separator.</remarks>
	IReadOnlyCollection<string> FlagValueSeparators { get; }

	/// <summary>The prefix used to start a list value.</summary>
	string ListPrefix { get; }

	/// <summary>The suffix used to end a list value.</summary>
	string ListSuffix { get; }

	/// <summary>The separator used to separate the values in the list.</summary>
	string ListValueSeparator { get; }
	#endregion
}
