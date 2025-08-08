using System.Xml;
using OwlDomain.Documentation;
using OwlDomain.Documentation.Document;
using OwlDomain.Documentation.Document.Nodes;

namespace OwlDomain.CommandLine.Documentation;

/// <summary>
/// 	Represents a provider for documentation information.
/// </summary>
public sealed class DocumentationProvider : IDocumentationProvider
{
	#region Fields
	private readonly DocumentationParser _parser = new();
	private readonly DocumentationFileFinder _fileFinder = new();
	private readonly DocumentationIdGenerator _idGenerator = new();
	private readonly Dictionary<Assembly, IAssemblyDocumentation?> _assemblies = [];
	#endregion

	#region Methods
	/// <inheritdoc/>
	public IDocumentationInfo? GetInfo(Type type)
	{
		Assembly? assembly = type.Assembly;

		if (assembly is null || TryEnsureDocumentation(assembly, out IAssemblyDocumentation? documentation) is false)
			return null;

		string id = _idGenerator.Get(type);
		if (documentation.Members.TryGetValue(id, out IMemberDocumentation? memberDoc) is false)
			return null;

		return Convert(memberDoc.RootNode);
	}

	/// <inheritdoc/>
	public IDocumentationInfo? GetInfo(PropertyInfo property)
	{
		Type? type = property.ReflectedType ?? property.DeclaringType;
		Assembly? assembly = type?.Assembly;

		if (assembly is null || TryEnsureDocumentation(assembly, out IAssemblyDocumentation? documentation) is false)
			return null;

		string id = _idGenerator.Get(property);
		if (documentation.Members.TryGetValue(id, out IMemberDocumentation? memberDoc) is false)
			return null;

		return Convert(memberDoc.RootNode);
	}

	/// <inheritdoc/>
	public IDocumentationInfo? GetInfo(ParameterInfo parameter)
	{
		MethodInfo? method = parameter.Member as MethodInfo;
		Type? type = method?.ReflectedType ?? method?.DeclaringType;
		Assembly? assembly = type?.Assembly;

		if (method is null || assembly is null || TryEnsureDocumentation(assembly, out IAssemblyDocumentation? documentation) is false)
			return null;

		string id = _idGenerator.Get(method);
		if (documentation.Members.TryGetValue(id, out IMemberDocumentation? memberDoc) is false)
			return null;

		IDocumentationNode? node =
			(memberDoc.RootNode as IDocumentationNodeCollection)
			?.Children
			.SingleOrDefault(c => c is IParameterTagDocumentationNode param && param.NameReference == parameter.Name);

		if (node is null)
			return null;

		return Convert(node);
	}

	/// <inheritdoc/>
	public IDocumentationInfo? GetInfo(MethodInfo method)
	{
		Type? type = method.ReflectedType ?? method.DeclaringType;
		Assembly? assembly = type?.Assembly;

		if (assembly is null || TryEnsureDocumentation(assembly, out IAssemblyDocumentation? documentation) is false)
			return null;

		string id = _idGenerator.Get(method);
		if (documentation.Members.TryGetValue(id, out IMemberDocumentation? memberDoc) is false)
			return null;

		return Convert(memberDoc.RootNode);
	}
	#endregion

	#region Helpers
	private bool TryEnsureDocumentation(Assembly assembly, [NotNullWhen(true)] out IAssemblyDocumentation? documentation)
	{
		if (_assemblies.TryGetValue(assembly, out documentation))
			return documentation is not null;

		if (_fileFinder.TryFind(assembly, out string? path) is false)
		{
			_assemblies.Add(assembly, null);
			return false;
		}

		XmlDocument xml = new();
		xml.Load(path);

		documentation = _parser.Parse(xml, assembly);
		_assemblies.Add(assembly, documentation);
		return true;
	}
	private static IDocumentationInfo? Convert(IDocumentationNode? node)
	{
		if (node is null)
			return null;

		if (node is IParameterTagDocumentationNode param)
		{
			if (param.Children.Count is 0)
				return null;

			IDocumentationNode summary = new DocumentationNodeCollection(param.Children);
			return new DocumentationInfo(summary, null);
		}

		if (node is IDocumentationNodeCollection collection)
		{
			IDocumentationNode? summary = collection.Children.FirstOrDefault(c => c is ISummaryTagDocumentationNode);
			if (summary is null)
				return null;

			IDocumentationNode? remarks = collection.Children.FirstOrDefault(c => c is IRemarksTagDocumentationNode);

			return new DocumentationInfo(summary, remarks);
		}

		return null;
	}
	#endregion
}
