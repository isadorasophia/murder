using Microsoft.CodeAnalysis;
using Murder.Serializer.Extensions;
using System.Collections.Immutable;

namespace Murder.Serializer.Metadata;

public sealed class ReferencedAssemblyTypeFetcher
{
    private readonly Compilation _compilation;

    public string? ParentAssembly => _parentAssembly;
    private readonly string? _parentAssembly = null;

    public ReferencedAssemblyTypeFetcher(Compilation compilation, string? parentAssembly)
    {
        _compilation = compilation;
        _parentAssembly = parentAssembly;
    }

    public INamedTypeSymbol? FindFirstClassImplementationInParentAssemblyOf(INamedTypeSymbol @interface)
    {
        IAssemblySymbol? s = _compilation.SourceModule.ReferencedAssemblySymbols
            .FirstOrDefault(s => s.Name == _parentAssembly);

        if (s is null)
        {
            return null;
        }

        bool Matches(INamedTypeSymbol t) => !t.IsValueType && t.ImplementsInterface(@interface);

        foreach (var namespaceSymbol in s.GlobalNamespace.GetNamespaceMembers())
        {
            if (namespaceSymbol.Name != "Murder")
            {
                continue;
            }

            foreach (var nestedNamespacecSymbol in namespaceSymbol.GetNamespaceMembers())
            {
                if (nestedNamespacecSymbol.Name != "Serialization")
                {
                    continue;
                }

                foreach (INamedTypeSymbol type in nestedNamespacecSymbol.GetTypeMembers())
                {
                    if (Matches(type))
                    {
                        return type;
                    }
                }
            }
        }

        return null;
    }

    internal List<INamedTypeSymbol> FindAllDeclaredSerializableAttributeTypes(
        MurderTypeSymbols knownSymbols, 
        INamedTypeSymbol contextType)
    {
        List<INamedTypeSymbol> serializableTypesFromParent = [];
        foreach (AttributeData attribute in contextType.GetAttributes())
        {
            if (attribute.AttributeClass is not INamedTypeSymbol t || 
                !t.Equals(knownSymbols.JsonSerializableAttribute, SymbolEqualityComparer.Default))
            {
                continue;
            }

            foreach (TypedConstant arg in attribute.ConstructorArguments)
            {
                object? v = arg.Value;

                if (v is INamedTypeSymbol s)
                {
                    serializableTypesFromParent.Add(s);
                }
            }
        }

        return serializableTypesFromParent;
    }
}