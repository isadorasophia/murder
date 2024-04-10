using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Murder.Generator.Metadata;

namespace Murder.Serializer.Extensions;

public static class HelperExtensions
{
    /// <summary>
    /// Checks if the given <paramref name="type"/> implements the interface <paramref name="interfaceToCheck"/>.
    /// </summary>
    /// <param name="type">Type declaration symbol.</param>
    /// <param name="interfaceToCheck">Interface to be checked.</param>
    /// <returns></returns>
    public static bool ImplementsInterface(
        this ITypeSymbol type,
        ISymbol? interfaceToCheck
    ) => type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, interfaceToCheck));

    public static bool IsSubtypeOf(
        this ITypeSymbol type,
        ISymbol subtypeToCheck)
    {
        ITypeSymbol? nextTypeToVerify = type;
        do
        {
            var subtype = nextTypeToVerify?.BaseType;
            if (subtype is not null && SymbolEqualityComparer.Default.Equals(subtype, subtypeToCheck))
            {
                return true;
            }

            nextTypeToVerify = subtype;

        } while (nextTypeToVerify is not null);

        return false;
    }

    public static int NumberOfParentClasses(INamedTypeSymbol type)
        => type.BaseType is null ? 0 : 1 + NumberOfParentClasses(type.BaseType);

    private static readonly SymbolDisplayFormat _fullyQualifiedDisplayFormat =
        new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

    public static string FullyQualifiedName(this ITypeSymbol type)
    {
        string fullyQualifiedTypeName = type.ToDisplayString(_fullyQualifiedDisplayFormat);

        // Roslyn graces us with Nullable types as `T?` instead of `Nullable<T>`, so we make an exception here.

        if (type is not INamedTypeSymbol namedTypeSymbol)
        {
            return fullyQualifiedTypeName;
        }

        if (fullyQualifiedTypeName[^1] == '?')
        {
            return $"{namedTypeSymbol.TypeArguments[0].FullyQualifiedName()}?";
        }

        if (type.IsTupleType || !namedTypeSymbol.IsGenericType)
        {
            return fullyQualifiedTypeName;
        }

        var genericTypes = string.Join(
            ", ",
            namedTypeSymbol.TypeArguments.Select(x => $"{x.FullyQualifiedName()}")
        );

        return $"{fullyQualifiedTypeName}<{genericTypes}>";
    }
}

public sealed class MetadataComparer : IEqualityComparer<MetadataType>
{
    public readonly static MetadataComparer Default = new();

    public bool Equals(MetadataType x, MetadataType y) => SymbolEqualityComparer.Default.Equals(x.Type, y.Type);

    public int GetHashCode(MetadataType obj) => SymbolEqualityComparer.Default.GetHashCode(obj.Type);
}

public sealed class DictionaryKeyTypesComparer : IEqualityComparer<ComplexDictionaryArguments>
{
    public readonly static DictionaryKeyTypesComparer Default = new();

    public bool Equals(ComplexDictionaryArguments x, ComplexDictionaryArguments y)
    {
        return SymbolEqualityComparer.Default.Equals(x.Key, y.Key) &&
            SymbolEqualityComparer.Default.Equals(x.Value, y.Value);
    }

    public int GetHashCode(ComplexDictionaryArguments obj)
    {
        return SymbolEqualityComparer.Default.GetHashCode(obj.Key) ^ SymbolEqualityComparer.Default.GetHashCode(obj.Value);
    }
}