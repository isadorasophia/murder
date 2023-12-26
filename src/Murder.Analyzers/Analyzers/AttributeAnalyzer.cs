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
        id: Diagnostics.Attributes.ImporterSettingsAttribute.Id,
        title: nameof(AttributeAnalyzer) + "." + nameof(ImporterSettingsAttribute),
        messageFormat: Diagnostics.Attributes.ImporterSettingsAttribute.Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Implementations of ResourceImporter need to be annotated with ImporterSettingsAttribute."
    );

    public static readonly DiagnosticDescriptor RuntimeOnlyAttributeOnNonComponent = new(
        id: Diagnostics.Attributes.RuntimeOnlyAttributeOnNonComponent.Id,
        title: nameof(AttributeAnalyzer) + "." + nameof(RuntimeOnlyAttributeOnNonComponent),
        messageFormat: Diagnostics.Attributes.RuntimeOnlyAttributeOnNonComponent.Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "RuntimeOnly attribute must annotate only IComponents."
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(ImporterSettingsAttribute, RuntimeOnlyAttributeOnNonComponent);

    public override void Initialize(AnalysisContext context)
    {
        var syntaxKind = ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.Attribute);

        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(Analyze, syntaxKind);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        AnalyzeResourceImporter(context);
        AnalyzeRuntimeOnlyAttribute(context);
    }
    
    private static void AnalyzeResourceImporter(SyntaxNodeAnalysisContext context)
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

    private static void AnalyzeRuntimeOnlyAttribute(SyntaxNodeAnalysisContext context)
    {
        // Bail if the node we are checking is not an argument
        if (context.Node is not AttributeSyntax attributeSyntax)
        {
            return;
        }

        // Bail if we can't find the unique attribute.
        var uniqueAttribute = context.Compilation.GetTypeByMetadataName(TypeMetadataNames.RuntimeOnlyAttribute);
        if (uniqueAttribute is null)
        {
            return;
        }

        // Bail if we can't find the IComponent interface
        var componentInterface = context.Compilation.GetTypeByMetadataName(TypeMetadataNames.ComponentInterface);
        if (componentInterface is null)
        {
            return;
        }

        var annotatedTypeNode = attributeSyntax.GetTypeAnnotatedByAttribute();
        if (annotatedTypeNode is null)
        {
            return;
        }

        var annotatedType = context.SemanticModel.GetDeclaredSymbol(annotatedTypeNode);
        if (annotatedType is not ITypeSymbol annotatedTypeSymbol)
        {
            return;
        }

        var attributeData = annotatedType
            .GetAttributes()
            .SingleOrDefault(a => a.ApplicationSyntaxReference!.GetSyntax() == attributeSyntax);
        var attributeClass = attributeData?.AttributeClass;
        if (attributeClass is null)
        {
            return;
        }

        // Return if this is not an UniqueAttribute.
        if (!uniqueAttribute.Equals(attributeClass, SymbolEqualityComparer.IncludeNullability))
        {
            return;
        }

        if (!annotatedTypeSymbol.ImplementsInterface(componentInterface))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    RuntimeOnlyAttributeOnNonComponent,
                    attributeSyntax.GetLocation()
                )
            );
        }
    }
}