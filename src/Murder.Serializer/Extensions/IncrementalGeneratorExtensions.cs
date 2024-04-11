using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace Murder.Serializer;

internal static class IncrementalGeneratorExtensions
{
    public static IncrementalValuesProvider<TypeDeclarationSyntax> PotentialStructs(
        this IncrementalGeneratorInitializationContext context) => 
        context.SyntaxProvider.CreateSyntaxProvider(
            (node, _) => node.IsStructOrRecord(),
            (c, _) => (TypeDeclarationSyntax)c.Node);

    public static IncrementalValuesProvider<ClassDeclarationSyntax> PotentialClasses(
        this IncrementalGeneratorInitializationContext context) => 
        context.SyntaxProvider.CreateSyntaxProvider(
            (node, _) => node.IsClass(),
            (c, _) => (ClassDeclarationSyntax)c.Node);

    public static bool IsClass(this SyntaxNode node)
        => node is ClassDeclarationSyntax;

    // Returns true for structs that implement an interface and records with base types.
    // We only check if a record is a value type later on in the chain because we need a TypeSymbol.
    public static bool IsStructOrRecord(this SyntaxNode node)
        => node is
            RecordDeclarationSyntax or StructDeclarationSyntax;
}
