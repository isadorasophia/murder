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
}