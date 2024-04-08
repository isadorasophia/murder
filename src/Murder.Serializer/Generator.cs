using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Murder.Serializer.Metadata;
using Murder.Serializer.Extensions;
using System.Collections.Immutable;
using Murder.Serializer.Templating;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Reflection;
using System.Diagnostics;
using Murder.Generator.Metadata;

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
        //if (!System.Diagnostics.Debugger.IsAttached)
        //{
        //    System.Diagnostics.Debugger.Launch();
        //}
#endif

        // Bail if any important type symbol is not resolvable.
        MurderTypeSymbols? symbols = MurderTypeSymbols.FromCompilation(compilation);
        if (symbols is null)
        {
            return;
        }

        MetadataFetcher metadata = new(compilation);
        metadata.Populate(symbols.Value, potentialStructs, potentialClasses);

        string projectName = compilation.AssemblyName?.Replace(".", "") ?? "My";

        INamedTypeSymbol? parentContext = metadata.ParentContext;

        SourceWriter jsonSerializerOptionsSource = 
            Templates.GenerateJsonSerializerOptions(metadata, projectName);

        SourceText sourceText = jsonSerializerOptionsSource.ToSourceText();
        context.AddSource(jsonSerializerOptionsSource.Filename, sourceText);

        RunIllegalSecondSourceGenerator(context, compilation, sourceText);
    }

    /// <summary>
    /// Now, this is where things get shady. You see, everyone will tell that running a second source generator is a bad idea,
    /// and you shouldn't do that.
    /// However, I really don't want to compromise my workflow by adding something manually or clumsy to the source.
    /// The compromise that I reached is this weird and unsupported code that, however, works. This will probably be around until
    /// https://github.com/dotnet/roslyn/issues/57239 is addressed.
    /// </summary>
    /// <param name="context">Source generation code context.</param>
    /// <param name="compilation">Compilation of the project.</param>
    /// <param name="sources">Source files which were just generated and should be scanned for code generation.</param>
    private static void RunIllegalSecondSourceGenerator(
        SourceProductionContext context, Compilation compilation, params SourceText[] sources)
    {
        ParseOptions options;
        if (compilation is CSharpCompilation csharpCompilation && csharpCompilation.SyntaxTrees.Length > 0)
        {
            options = csharpCompilation.SyntaxTrees[0].Options;
        }
        else
        {
            options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
        }

        // Add all sources to our compilation.
        foreach (SourceText source in sources)
        {
            SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(source, options);
            compilation = compilation.AddSyntaxTrees(syntaxTree);
        }

        Assembly? a = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains("System.Text.Json.SourceGeneration"));
        Type? textJsonForbiddenImporter = a?.GetType("System.Text.Json.SourceGeneration.JsonSourceGenerator");

        if (textJsonForbiddenImporter is null)
        {
            Debug.Fail("Unable to find System.Text.Json generator. Is it now loaded somehow?");
            return;
        }

        // See declaration of type at
        // https://github.com/dotnet/runtime/blob/c5bead63f8386f716b8ddd909c93086b3546efed/src/libraries/System.Text.Json/gen/JsonSourceGenerator.Roslyn4.0.cs
        ISourceGenerator jsonGenerator =
            ((IIncrementalGenerator)Activator.CreateInstance(textJsonForbiddenImporter)).AsSourceGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(jsonGenerator);
        driver = driver.RunGenerators(compilation);

        GeneratorDriverRunResult driverResult = driver.GetRunResult();
        foreach (GeneratorRunResult result in driverResult.Results)
        {
            foreach (GeneratedSourceResult source in result.GeneratedSources)
            {
                context.AddSource("__Custom" + source.HintName, source.SourceText);
            }
        }
    }
}
