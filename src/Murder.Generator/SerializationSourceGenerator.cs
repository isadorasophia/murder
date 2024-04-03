using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Murder.Generator.Metadata;
using System.Collections.Immutable;

namespace Murder.Generator;

[Generator]
public sealed class BangExtensionsGenerator : IIncrementalGenerator
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
        ImmutableArray<TypeDeclarationSyntax> potentialComponents = input.Left.Left;
        ImmutableArray<ClassDeclarationSyntax> potentialStateMachines = input.Left.Right;

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
    }
}
