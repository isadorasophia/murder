using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Murder.Serializer.Metadata;
using Murder.Serializer.Extensions;
using System.Collections.Immutable;
using Murder.Serializer.Templating;

namespace Murder.Serializer;

[Generator]
public sealed class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var structs = context.PotentialStructs().Collect();
        var classes = context.PotentialClasses().Collect();

        var compilation = structs
            .Combine(classes)
            .Combine(context.CompilationProvider);

        context.RegisterSourceOutput(
            compilation,
            EmitSource
        );
    }

    private void EmitSource(SourceProductionContext context, ((ImmutableArray<TypeDeclarationSyntax> Left, ImmutableArray<ClassDeclarationSyntax> Right) Left, Compilation Right) input)
    {
        Compilation compilation = input.Right;

        ImmutableArray<TypeDeclarationSyntax> potentialStructs = input.Left.Left;
        ImmutableArray<ClassDeclarationSyntax> potentialClasses = input.Left.Right;

#if DEBUG
        // Uncomment this if you need to use a debugger.
        // if (!System.Diagnostics.Debugger.IsAttached)
        // {
        //     System.Diagnostics.Debugger.Launch();
        // }
#endif

        // Bail if any important type symbol is not resolvable.
        MurderTypeSymbols? bangTypeSymbols = MurderTypeSymbols.FromCompilation(compilation);
        if (bangTypeSymbols is null)
        {
            return;
        }

        ReferencedAssemblyTypeFetcher referencedAssemblyTypeFetcher = new(compilation);

        INamedTypeSymbol? parentSerialization = referencedAssemblyTypeFetcher
            .GetAllCompiledClassesWithSubtypes()
            .Where(t => t.ImplementsInterface(bangTypeSymbols.SerializerInterface))
            .OrderBy(HelperExtensions.NumberOfParentClasses)
            .LastOrDefault();

        string projectName = compilation.AssemblyName?.Replace(".", "") ?? "My";

        SourceWriter optionsSource = 
            Templates.GenerateJsonSerializerOptions(projectName);

        context.AddSource(optionsSource.Filename, optionsSource.ToSourceText());
    }
}
