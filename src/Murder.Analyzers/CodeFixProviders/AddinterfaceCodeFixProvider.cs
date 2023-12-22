using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Murder.Analyzers.Extensions;
using System.Collections.Immutable;
using System.Composition;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Murder.Analyzers.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddInterfaceCodeFixProvider)), Shared]
public sealed class AddInterfaceCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(
            Diagnostics.Attributes.RuntimeOnlyAttributeOnNonComponent.Id
        );

    public override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document
            .GetSyntaxRootAsync(context.CancellationToken)
            .ConfigureAwait(false);

        if (root is null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            if (root.FindNode(diagnosticSpan) is not AttributeSyntax attributeSyntax)
            {
                return;
            }

            if (attributeSyntax.GetTypeAnnotatedByAttribute() is not TypeDeclarationSyntax typeDeclarationSyntax)
            {
                return;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixes.AddInterface.Title("IComponent"),
                    createChangedDocument: c => AddInterface(context.Document, typeDeclarationSyntax, c),
                    equivalenceKey: nameof(CodeFixes.AddInterface)),
                diagnostic
            );
        }
    }

    private async Task<Document> AddInterface(
        Document document,
        TypeDeclarationSyntax typeDeclarationSyntax,
        CancellationToken cancellationToken)
    {
        var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (oldRoot is null)
        {
            return document;
        }

        var typeSyntax = ParseTypeName("IComponent");
        var baseTypeSyntax = SimpleBaseType(typeSyntax);
        var newBaseListSyntax =
            typeDeclarationSyntax.BaseList?.AddTypes(baseTypeSyntax) ??
            BaseList(SeparatedList<BaseTypeSyntax>(new[] { baseTypeSyntax }));
        var newTypeDeclarationSyntax = typeDeclarationSyntax.WithBaseList(newBaseListSyntax);
        var rootWithAttribute = oldRoot.ReplaceNode(typeDeclarationSyntax, newTypeDeclarationSyntax);

        var bangComponentsIsImported = rootWithAttribute
            .ChildNodes()
            .OfType<UsingDirectiveSyntax>()
            .Any(u => u.Name.ToString() == "Bang.Components");

        if (bangComponentsIsImported)
        {
            return document.WithSyntaxRoot(rootWithAttribute);
        }

        if (rootWithAttribute is not CompilationUnitSyntax compilationUnit)
        {
            return document.WithSyntaxRoot(rootWithAttribute);
        }

        var bangToken = IdentifierName("Bang");
        var systemsToken = IdentifierName("Components");
        var name = QualifiedName(bangToken, systemsToken);
        var rootWithImportedNamespace = compilationUnit.AddUsings(UsingDirective(name));

        return document.WithSyntaxRoot(rootWithImportedNamespace);
    }
}