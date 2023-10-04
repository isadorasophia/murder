using Microsoft.CodeAnalysis;

namespace Murder.Analyzers.Extensions;

public static class TypeSymbolExtensions
{
    public static bool IsSubtypeOf(this ITypeSymbol type, ISymbol subtypeToCheck)
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

    internal static bool HasAttribute(this INamedTypeSymbol type, ISymbol? attributeToCheck)
        => type.GetAttributes().Any(attr =>
            attr.AttributeClass is not null && attr.AttributeClass.Equals(attributeToCheck,
                SymbolEqualityComparer.IncludeNullability));

}