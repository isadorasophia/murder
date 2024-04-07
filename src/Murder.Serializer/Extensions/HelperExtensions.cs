using Microsoft.CodeAnalysis;

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
        if (fullyQualifiedTypeName.Contains("?") || type is not INamedTypeSymbol { IsGenericType: true } namedTypeSymbol)
        {
            return fullyQualifiedTypeName;
        }

        var genericTypes = string.Join(
            ", ",
            namedTypeSymbol.TypeArguments.Select(x => $"global::{x.FullyQualifiedName()}")
        );

        return $"{fullyQualifiedTypeName}<{genericTypes}>";
    }
}