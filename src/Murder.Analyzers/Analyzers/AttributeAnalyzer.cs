using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Murder.Analyzers.Extensions;
using System.Collections.Immutable;

namespace Murder.Analyzers.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AttributeAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor ImporterSettingsAttribute = new(
        title: nameof(ResourceAnalyzer) + "." + nameof(ImporterSettingsAttribute),
        messageFormat: Diagnostics.Resources.ImporterSettingsAttribute.Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Implementations of ResourceImporter need to be annotated with ImporterSettingsAttribute."
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(ImporterSettingsAttribute);

    public override void Initialize(AnalysisContext context)
    {
        var syntaxKind = ImmutableArray.Create(SyntaxKind.ClassDeclaration);

        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(Analyze, syntaxKind);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        // Bail if ResourceImporter is not resolvable.
        var resourceImporter = context.Compilation.GetTypeByMetadataName(TypeMetadataNames.ResourceImporter);
        if (resourceImporter is null)
        {
            return;
        }

        // Bail if the node we are checking is not a type declaration.
        if (context.ContainingSymbol is not INamedTypeSymbol typeSymbol || context.Node is not TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return;
        }

        // Abstract types don't need to be annotated.
        if (typeSymbol.IsAbstract)
        {
            return;
        }

        // Bail if the current type declaration is not a ResourceImporter.
        if (!typeSymbol.IsSubtypeOf(resourceImporter))
        {
            return;
        }

        // Bail if the attribute we care about is not resolvable.
        var importerSettingsAttribute = context.Compilation.GetTypeByMetadataName(TypeMetadataNames.ImporterSettingsAttribute);
        if (importerSettingsAttribute is null)
        {
            return;
        }

        var hasAttribute = typeSymbol.HasAttribute(importerSettingsAttribute);
        if (!hasAttribute)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ImporterSettingsAttribute,
                    typeDeclarationSyntax.Identifier.GetLocation()
                )
            );
        }
    }
}