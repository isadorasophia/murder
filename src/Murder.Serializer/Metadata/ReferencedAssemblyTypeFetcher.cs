using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Murder.Serializer.Metadata;

public sealed class ReferencedAssemblyTypeFetcher
{
    private readonly Compilation _compilation;
    private ImmutableArray<INamedTypeSymbol>? _cacheOfAllTypesInReferenceProjects;

    private readonly string? _parentAssembly = null;

    public ReferencedAssemblyTypeFetcher(Compilation compilation, string? parentAssembly)
    {
        _compilation = compilation;
        _parentAssembly = parentAssembly;
    }

    public ImmutableArray<INamedTypeSymbol> AllTypesInReferencedAssembly()
    {
        if (_cacheOfAllTypesInReferenceProjects is not null)
        {
            return _cacheOfAllTypesInReferenceProjects.Value;
        }

        IAssemblySymbol? s = _compilation.SourceModule.ReferencedAssemblySymbols
            .FirstOrDefault(s => s.Name == _parentAssembly);

        if (s is null)
        {
            return ImmutableArray<INamedTypeSymbol>.Empty;
        }

        _cacheOfAllTypesInReferenceProjects = s.GlobalNamespace.GetNamespaceMembers()
            .SelectMany(GetAllTypesInNamespace).ToImmutableArray();

        return _cacheOfAllTypesInReferenceProjects.Value;
    }

    // Recursive method to get all types in a namespace, including nested types.
    private static IEnumerable<INamedTypeSymbol> GetAllTypesInNamespace(INamespaceSymbol namespaceSymbol)
    {
        foreach (INamedTypeSymbol type in namespaceSymbol.GetTypeMembers())
        {
            yield return type;
        }

        IEnumerable<INamedTypeSymbol> nestedTypes =
            from nestedNamespace in namespaceSymbol.GetNamespaceMembers()
            from nestedType in GetAllTypesInNamespace(nestedNamespace)
            select nestedType;

        foreach (INamedTypeSymbol nestedType in nestedTypes)
        {
            yield return nestedType;
        }
    }
}