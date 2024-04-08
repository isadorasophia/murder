using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Murder.Serializer.Extensions;
using Murder.Serializer.Metadata;
using System.Collections.Immutable;
using System.Reflection;

namespace Murder.Generator.Metadata;

public readonly struct MetadataType
{
    public ITypeSymbol Type { get; init; } = default!;

    public string QualifiedName { get; init; } = string.Empty;

    /// <summary>
    /// We deserialize pretty much everything as a polymorphicc type.
    /// The only exception is fields, which already have its specified type.
    /// </summary>
    public bool IsPolymorphic { get; init; } = true;

    public MetadataType() { }
}

public readonly struct DictionaryKeyTypes
{
    public INamedTypeSymbol Key { get; init; }
    public INamedTypeSymbol Value { get; init; }
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

    public readonly HashSet<MetadataType> SerializableTypes = new(MetadataComparer.Default);
    public readonly HashSet<MetadataType> PolymorphicTypes = new(MetadataComparer.Default);

    public readonly HashSet<DictionaryKeyTypes> ComplexDictionaries = new(DictionaryKeyTypesComparer.Default);

    private readonly HashSet<ITypeSymbol> _polymorphicTypesToLookForImplementation = new(SymbolEqualityComparer.Default);

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
            if (!IsSerializableType(symbols, component))
            {
                continue;
            }

            MetadataType m = new() { Type = component, QualifiedName = component.FullyQualifiedName() };
            TrackMetadata(symbols, m);
        }

        var messages = FetchMessages(symbols, allValueTypesToBeCompiled);
        foreach (var message in messages)
        {
            if (!IsSerializableType(symbols, message))
            {
                continue;
            }

            MetadataType m = new() { Type = message, QualifiedName = message.FullyQualifiedName() };
            TrackMetadata(symbols, m);
        }

        var stateMachines = FetchStateMachines(symbols, potentialClasses);
        foreach (var stateMachine in stateMachines)
        {
            if (!IsSerializableType(symbols, stateMachine))
            {
                continue;
            }

            MetadataType m = new() 
            { 
                Type = stateMachine, 
                QualifiedName = $"Bang.StateMachines.StateMachineComponent<{stateMachine.FullyQualifiedName()}>" 
            };

            TrackMetadata(symbols, m);
        }

        var interactions = FetchInteractions(symbols, allValueTypesToBeCompiled);
        foreach (var interaction in interactions)
        {
            if (!IsSerializableType(symbols, interaction))
            {
                continue;
            }

            MetadataType m = new()
            {
                Type = interaction,
                QualifiedName = $"Bang.Interactions.InteractiveComponent<{interaction.FullyQualifiedName()}>"
            };

            TrackMetadata(symbols, m);
        }

        var assets = FetchGameAssets(symbols, potentialClasses);
        foreach (var asset in assets)
        {
            MetadataType m = new() { Type = asset, QualifiedName = asset.FullyQualifiedName() };
            TrackMetadata(symbols, m);
        }

        return true;
    }

    private INamedTypeSymbol? FetchParentContext(MurderTypeSymbols symbols)
    {
        return _referencedAssemblyTypeFetcher
            .GetAllCompiledClassesWithSubtypes()
            .Where(t => t.ImplementsInterface(symbols.SerializerContextInterface))
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

    private IEnumerable<INamedTypeSymbol> FetchInteractions(
        MurderTypeSymbols symbols,
        ImmutableArray<INamedTypeSymbol> allValueTypesToBeCompiled) =>
        allValueTypesToBeCompiled
        .Where(t => !t.IsGenericType && t.ImplementsInterface(symbols.InteractionInterface))
        .OrderBy(i => i.Name);

    private IEnumerable<INamedTypeSymbol> FetchStateMachines(
        MurderTypeSymbols symbols,
        ImmutableArray<ClassDeclarationSyntax> potentialStateMachines) =>
        potentialStateMachines
            .Select(GetTypeSymbol)
            .Where(t => !t.IsAbstract && t.IsSubtypeOf(symbols.StateMachineClass))
            .OrderBy(x => x.Name);

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

    private void TrackMetadata(MurderTypeSymbols symbols, MetadataType metadataType, IAssemblySymbol? parentAssembly = null)
    {
        if (!SerializableTypes.Add(metadataType))
        {
            // already tracked, bye.
            return;
        }

        ITypeSymbol t = metadataType.Type;
        if (t.ContainingAssembly is null)
        {
            return;
        }

        if (parentAssembly is null || t.ContainingAssembly.Equals(parentAssembly, SymbolEqualityComparer.Default))
        {
            // Either this is root or matches the assembly of the parent, so we are okay checking it out.
            // Manually track private fields, because System.Text.Json won't do it for us.
            LookForPrivateCandidateFields(symbols, metadataType.Type);
        }
    }

    private void LookForPrivateCandidateFields(MurderTypeSymbols murderSymbols, ITypeSymbol symbol)
    {
        foreach (ISymbol member in symbol.GetMembers())
        {
            if (member.Kind != SymbolKind.Field && member.Kind != SymbolKind.Property)
            {
                continue;
            }

            bool isSerializable = IsSerializableMember(murderSymbols, member);
            if (!isSerializable)
            {
                // not interesting to us.
                continue;
            }

            ITypeSymbol? memberType = member is IFieldSymbol field ? field.Type : member is IPropertySymbol property ? property.Type : null;
            if (memberType is null)
            {
                continue;
            }

            if (member.DeclaredAccessibility != Accessibility.Public)
            {
                MetadataType m = new() { Type = memberType, QualifiedName = memberType.FullyQualifiedName() };
                TrackMetadata(murderSymbols, m, parentAssembly: symbol.ContainingAssembly);
            }

            if (IsPolymorphismCandidate(murderSymbols, memberType))
            {
                _polymorphicTypesToLookForImplementation.Add(memberType);
            }

            if (memberType is INamedTypeSymbol memberNamedType && memberNamedType.IsGenericType)
            {
                foreach (INamedTypeSymbol a in memberNamedType.TypeArguments)
                {
                    if (IsPolymorphismCandidate(murderSymbols, a))
                    {
                        _polymorphicTypesToLookForImplementation.Add(a);
                    }
                }
            }
        }
    }

    private bool IsPolymorphismCandidate(MurderTypeSymbols murderSymbols, ITypeSymbol s)
    {
        if (!s.IsAbstract)
        {
            return false;
        } 
        
        if (s.ContainingNamespace.Name.StartsWith("System"))
        {
            return false;
        }

        if (s.Equals(murderSymbols.ComponentInterface, SymbolEqualityComparer.Default))
        {
            return false;
        }

        if (s.Equals(murderSymbols.MessageInterface, SymbolEqualityComparer.Default))
        {
            return false;
        }

        if (s.Equals(murderSymbols.StateMachineClass, SymbolEqualityComparer.Default))
        {
            return false;
        }

        if (s.Equals(murderSymbols.StateMachineComponentInterface, SymbolEqualityComparer.Default))
        {
            return false;
        }

        if (s.Equals(murderSymbols.InteractionInterface, SymbolEqualityComparer.Default))
        {
            return false;
        }

        if (s.Equals(murderSymbols.InteractiveComponentInterface, SymbolEqualityComparer.Default))
        {
            return false;
        }

        if (s.Equals(murderSymbols.GameAssetClass, SymbolEqualityComparer.Default))
        {
            return false;
        }

        return true;
    }

    private static bool IsSerializableType(MurderTypeSymbols murderSymbols, INamedTypeSymbol type)
    {
        foreach (AttributeData attribute in type.GetAttributes())
        {
            if (attribute.AttributeClass is not INamedTypeSymbol s)
            {
                continue;
            }

            if (s.Equals(murderSymbols.PersistOnSaveAttribute, SymbolEqualityComparer.Default))
            {
                return true;
            }

            if (s.Equals(murderSymbols.DoNotPersistOnSaveAttribute, SymbolEqualityComparer.Default))
            {
                return false;
            }

            if (s.Equals(murderSymbols.DoNotPersistEntityOnSaveAttribute, SymbolEqualityComparer.Default))
            {
                return false;
            }

            if (s.Equals(murderSymbols.RuntimeOnlyAttribute, SymbolEqualityComparer.Default))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns whether a member (property, field) is serialiazable.
    /// </summary>
    private static bool IsSerializableMember(MurderTypeSymbols murderSymbols, ISymbol member)
    {
        if (member is IPropertySymbol property && property.SetMethod is null)
        {
            // we very explicitly ignore { get; } properties.
            return false;
        }

        foreach (AttributeData attribute in member.GetAttributes())
        {
            if (attribute.AttributeClass is not INamedTypeSymbol s)
            {
                continue;
            }

            if (s.Equals(murderSymbols.IgnoreFieldAttribute, SymbolEqualityComparer.Default))
            {
                return false;
            }

            if (s.Equals(murderSymbols.SerializeFieldAttribute, SymbolEqualityComparer.Default))
            {
                return true;
            }
        }

        if (member.DeclaredAccessibility != Accessibility.Public)
        {
            return false;
        }

        return true;
    }
}