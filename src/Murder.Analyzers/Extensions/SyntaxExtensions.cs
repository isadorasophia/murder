using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Murder.Analyzers.Extensions;

public static class SyntaxExtensions
{
    // First call to .Parent gets the AttributeList.
    // Second call to .Parent get the type annotated with the attribute we're looking for.
    public static SyntaxNode? GetTypeAnnotatedByAttribute(this AttributeSyntax? attributeSyntax)
        => attributeSyntax?.Parent?.Parent;
}