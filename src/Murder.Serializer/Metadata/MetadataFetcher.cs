using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Murder.Serializer.Extensions;
using Murder.Serializer.Metadata;
using System.Collections.Immutable;

namespace Murder.Generator.Metadata;

public record struct DictionaryKeyTypes
{
    public INamedTypeSymbol Key;
    public INamedTypeSymbol Value;
}

public sealed class MetadataFetcher
{
    private readonly Compilation _compilation;
    private readonly ReferencedAssemblyTypeFetcher _referencedAssemblyTypeFetcher;

    public MetadataFetcher(Compilation compilation)
    {
        _compilation = compilation;
        _referencedAssemblyTypeFetcher = new(compilation);
    }

    public readonly HashSet<INamedTypeSymbol> SerializableTypes = new(SymbolEqualityComparer.Default);
    public readonly HashSet<INamedTypeSymbol> PolymorphicTypes = new(SymbolEqualityComparer.Default);

    public readonly HashSet<DictionaryKeyTypes> ComplexDictionaries = new();

    /// <summary>
    /// If this inherits from another Murder project, we will need to join the symbols.
    /// </summary>
    public INamedTypeSymbol? ParentContext { get; private set; } = null;

    internal bool Populate(
        MurderTypeSymbols symbols,
        ImmutableArray<TypeDeclarationSyntax> potentialStructs,
        ImmutableArray<ClassDeclarationSyntax> potentialClasses)
    {
        ParentContext = FetchParentContext(symbols);

        // Gets all potential components/messages from the assembly this generator is processing.
        var builder = ImmutableArray.CreateBuilder<INamedTypeSymbol>();
        foreach (TypeDeclarationSyntax t in potentialStructs)
        {
            if (ValueTypeFromTypeDeclarationSyntax(t) is not INamedTypeSymbol symbol)
            {
                continue;
            }

            builder.Add(symbol);
        }

        ImmutableArray<INamedTypeSymbol> allValueTypesToBeCompiled = builder.ToImmutable();

        var components = FetchComponents(symbols, allValueTypesToBeCompiled);
        foreach (var component in components)
        {
            SerializableTypes.Add(component);
        }

        var messages = FetchMessages(symbols, allValueTypesToBeCompiled);
        foreach (var message in messages)
        {
            SerializableTypes.Add(message);
        }

        var stateMachines = FetchStateMachines(symbols, potentialClasses);
        foreach (var stateMachine in stateMachines)
        {
            SerializableTypes.Add(stateMachine);
        }

        var interactions = FetchInteractions(symbols, allValueTypesToBeCompiled);
        foreach (var interaction in interactions)
        {
            SerializableTypes.Add(interaction);
        }

        var assets = FetchGameAssets(symbols, potentialClasses);
        foreach (var asset in assets)
        {
            SerializableTypes.Add(asset);
        }

        return true;
    }

    private INamedTypeSymbol? FetchParentContext(MurderTypeSymbols symbols)
    {
        return _referencedAssemblyTypeFetcher
            .GetAllCompiledClassesWithSubtypes()
            .Where(t => t.ImplementsInterface(symbols.SerializerInterface))
            .OrderBy(HelperExtensions.NumberOfParentClasses)
            .LastOrDefault();
    }

    private IEnumerable<INamedTypeSymbol> FetchComponents(
        MurderTypeSymbols symbols,
        ImmutableArray<INamedTypeSymbol> allValueTypesToBeCompiled) => 
        allValueTypesToBeCompiled
            .Where(t => !t.IsGenericType && t.ImplementsInterface(symbols.ComponentInterface))
            .OrderBy(c => c.Name);

    private IEnumerable<INamedTypeSymbol> FetchMessages(
        MurderTypeSymbols symbols,
        ImmutableArray<INamedTypeSymbol> allValueTypesToBeCompiled) =>
        allValueTypesToBeCompiled
            .Where(t => !t.IsGenericType && t.ImplementsInterface(symbols.MessageInterface))
            .OrderBy(x => x.Name);

    private IEnumerable<INamedTypeSymbol> FetchStateMachines(
        MurderTypeSymbols symbols,
        ImmutableArray<ClassDeclarationSyntax> potentialStateMachines) => 
        potentialStateMachines
            .Select(GetTypeSymbol)
            .Where(t => !t.IsAbstract && t.IsSubtypeOf(symbols.StateMachineClass))
            .OrderBy(x => x.Name);

    private static IEnumerable<INamedTypeSymbol> FetchInteractions(
        MurderTypeSymbols symbols,
        ImmutableArray<INamedTypeSymbol> allValueTypesToBeCompiled) =>
        allValueTypesToBeCompiled
        .Where(t => !t.IsGenericType && t.ImplementsInterface(symbols.InteractionInterface))
        .OrderBy(i => i.Name);

    private IEnumerable<INamedTypeSymbol> FetchGameAssets(
        MurderTypeSymbols symbols,
        ImmutableArray<ClassDeclarationSyntax> potentialClasses) =>
        potentialClasses
            .Select(GetTypeSymbol)
            .Where(t => !t.IsAbstract && t.IsSubtypeOf(symbols.GameAssetClass))
            .OrderBy(x => x.Name);

    private INamedTypeSymbol? ValueTypeFromTypeDeclarationSyntax(
        TypeDeclarationSyntax typeDeclarationSyntax)
    {
        var semanticModel = _compilation.GetSemanticModel(typeDeclarationSyntax.SyntaxTree);
        if (semanticModel.GetDeclaredSymbol(typeDeclarationSyntax) is not INamedTypeSymbol potentialComponentTypeSymbol)
        {
            return null;
        }

        // Record *classes* cannot be components or messages.
        if (typeDeclarationSyntax is RecordDeclarationSyntax && !potentialComponentTypeSymbol.IsValueType)
        {
            return null;
        }

        return potentialComponentTypeSymbol;
    }

    private INamedTypeSymbol GetTypeSymbol(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var semanticModel = _compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
        return (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(classDeclarationSyntax)!;
    }
}