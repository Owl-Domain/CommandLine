namespace OwlDomain.CommandLine.Groups;

/// <summary>
/// 	Represents information about a command group that is associated with a class.
/// </summary>
public interface IClassCommandGroupInfo : ICommandGroupInfo
{
	#region Properties
	/// <summary>The class that the command group is associated with.</summary>
	Type Class { get; }
	#endregion
}

/// <summary>
/// 	Represents information about a command group that is associated with a class.
/// </summary>
/// <typeparam name="T">The class that the command group is associated with.</typeparam>
public interface IClassCommandGroupInfo<out T> : IClassCommandGroupInfo
	where T : class
{
	#region Properties
	Type IClassCommandGroupInfo.Class => typeof(T);
	#endregion
}
