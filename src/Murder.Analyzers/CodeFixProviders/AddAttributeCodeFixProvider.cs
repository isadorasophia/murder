using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Murder.Analyzers.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddAttributeCodeFixProvider)), Shared]
public sealed class AddAttributeCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(Diagnostics.Attributes.ImporterSettingsAttribute.Id);

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
            if (root.FindNode(diagnosticSpan) is TypeDeclarationSyntax typeDeclarationSyntax)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: CodeFixes.AddAttribute.Title("ImporterSettings"),
                        createChangedDocument: c => AddAttribute(context.Document, typeDeclarationSyntax, c),
                        equivalenceKey: nameof(CodeFixes.AddAttribute)),
                    diagnostic
                );
            }
        }
    }

    private async Task<Document> AddAttribute(
        Document document,
        TypeDeclarationSyntax typeDeclarationSyntax,
        CancellationToken cancellationToken)
    {
        var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (oldRoot is null)
        {
            return document;
        }

        var nameSyntax = IdentifierName("ImporterSettings");
        var filterAttributeArgument = AttributeArgument(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName("FilterType"),
                Token(SyntaxKind.DotToken),
                IdentifierName("OnlyTheseFolders")
            )
        );

        var emptyStringArrayAttributeArgument = AttributeArgument(
            ArrayCreationExpression(
                ArrayType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                InitializerExpression(
                    SyntaxKind.ArrayInitializerExpression,
                    SeparatedList<ExpressionSyntax>(
                    new[]
                    {
                        LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            Token(SyntaxTriviaList.Empty, SyntaxKind.StringLiteralToken, $"\"\"", "", SyntaxTriviaList.Empty)
                        )
                    })
                )
            ).AddTypeRankSpecifiers(
                ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()))
            )
        );

        var dotStringArrayAttributeArgument = AttributeArgument(
            ArrayCreationExpression(
                ArrayType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                InitializerExpression(
                    SyntaxKind.ArrayInitializerExpression,
                    SeparatedList<ExpressionSyntax>(
                        new[]
                        {
                            LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                Token(
                                    SyntaxTriviaList.Empty,
                                    SyntaxKind.StringLiteralToken,
                                    $"\".\"",
                                    "",
                                    SyntaxTriviaList.Empty
                                )
                            )
                        })
                )
            ).AddTypeRankSpecifiers(
                ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()))
            )
        );

        var attributeArguments = AttributeArgumentList(
            SeparatedList(
                new[]
                {
                    filterAttributeArgument,
                    emptyStringArrayAttributeArgument,
                    dotStringArrayAttributeArgument
                }
            )
        );
        var attribute = Attribute(nameSyntax).WithArgumentList(attributeArguments);

        var separatedSyntaxList = SeparatedList(new[] { attribute });
        var newAttributeList = AttributeList(separatedSyntaxList);
        var attributeLists =
            new SyntaxList<AttributeListSyntax>(typeDeclarationSyntax.AttributeLists.Prepend(newAttributeList));
        var newTypeDeclarationSyntax = typeDeclarationSyntax.WithAttributeLists(attributeLists);
        var rootWithAttribute = oldRoot.ReplaceNode(typeDeclarationSyntax, newTypeDeclarationSyntax);

        var correctNamespaceIsImported = rootWithAttribute
            .ChildNodes()
            .OfType<UsingDirectiveSyntax>()
            .Any(u => u.Name.ToString() == "Murder.Editor.Importers");


        if (correctNamespaceIsImported || rootWithAttribute is not CompilationUnitSyntax compilationUnit)
        {
            return document.WithSyntaxRoot(rootWithAttribute);
        }

        var murderToken = IdentifierName("Murder");
        var editorToken = IdentifierName("Editor");
        var importersToken = IdentifierName("Importers");

        var name = QualifiedName(QualifiedName(murderToken, editorToken), importersToken);
        var rootWithImportedNamespace = compilationUnit.AddUsings(UsingDirective(name));

        return document.WithSyntaxRoot(rootWithImportedNamespace);
    }
}