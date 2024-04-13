using Microsoft.CodeAnalysis;

namespace Murder.Serializer.Metadata;

internal readonly struct MurderTypeSymbols
{
    public INamedTypeSymbol ComponentInterface { get; init; }
    public INamedTypeSymbol MessageInterface { get; init; }
    public INamedTypeSymbol StateMachineClass { get; init; }
    public INamedTypeSymbol StateMachineComponentInterface { get; init; }
    public INamedTypeSymbol InteractionInterface { get; init; }
    public INamedTypeSymbol InteractiveComponentInterface { get; init; }
    public INamedTypeSymbol GameAssetClass { get; init; }
    public INamedTypeSymbol SerializerContextInterface { get; init; }
    public INamedTypeSymbol ComplexDictionaryClass { get; init; }

    // ** Attributes ** //

    public INamedTypeSymbol SerializeFieldAttribute { get; init; }
    public INamedTypeSymbol ShowInEditorFieldAttribute { get; init; }
    public INamedTypeSymbol IgnoreFieldAttribute { get; init; }
    public INamedTypeSymbol JsonSerializableAttribute { get; init; }
    public INamedTypeSymbol RuntimeOnlyAttribute { get; init; }
    public INamedTypeSymbol PersistOnSaveAttribute { get; init; }
    public INamedTypeSymbol DoNotPersistOnSaveAttribute { get; init; }
    public INamedTypeSymbol DoNotPersistEntityOnSaveAttribute { get; init; }

    public static MurderTypeSymbols? FromCompilation(Compilation compilation)
    {
        INamedTypeSymbol? componentInterface = compilation.GetTypeByMetadataName("Bang.Components.IComponent");
        if (componentInterface is null) return null;

        INamedTypeSymbol? messageInterface = compilation.GetTypeByMetadataName("Bang.Components.IMessage");
        if (messageInterface is null) return null;

        INamedTypeSymbol? stateMachineClass = compilation.GetTypeByMetadataName("Bang.StateMachines.StateMachine");
        if (stateMachineClass is null) return null;

        INamedTypeSymbol? stateMachineComponentInterface = compilation.GetTypeByMetadataName("Bang.StateMachines.IStateMachineComponent");
        if (stateMachineComponentInterface is null) return null;

        INamedTypeSymbol? interactionInterface = compilation.GetTypeByMetadataName("Bang.Interactions.IInteraction");
        if (interactionInterface is null) return null;

        INamedTypeSymbol? interactiveComponentInterface = compilation.GetTypeByMetadataName("Bang.Interactions.IInteractiveComponent");
        if (interactiveComponentInterface is null) return null;

        INamedTypeSymbol? gameAssetClass = compilation.GetTypeByMetadataName("Murder.Assets.GameAsset");
        if (gameAssetClass is null) return null;

        INamedTypeSymbol? murderSerializer = compilation.GetTypeByMetadataName("Murder.Serialization.IMurderSerializer");
        if (murderSerializer is null) return null;

        INamedTypeSymbol? complexDictionaryClass = compilation.GetTypeByMetadataName("Murder.Serialization.ComplexDictionary`2");
        if (complexDictionaryClass is null) return null;

        // attributes

        INamedTypeSymbol? serializeFieldAttribute = compilation.GetTypeByMetadataName("Bang.SerializeAttribute");
        if (serializeFieldAttribute is null) return null;

        INamedTypeSymbol? showInEditorAttribute = compilation.GetTypeByMetadataName("Murder.Attributes.ShowInEditorAttribute");
        if (showInEditorAttribute is null) return null;

        INamedTypeSymbol? ignoreFieldAttribute = compilation.GetTypeByMetadataName("System.Text.Json.Serialization.JsonIgnoreAttribute");
        if (ignoreFieldAttribute is null) return null;

        INamedTypeSymbol? jsonSerializableAttribute = compilation.GetTypeByMetadataName("System.Text.Json.Serialization.JsonSerializableAttribute");
        if (jsonSerializableAttribute is null) return null;

        INamedTypeSymbol? runtimeOnlyAttribute = compilation.GetTypeByMetadataName("Murder.Utilities.Attributes.RuntimeOnlyAttribute");
        if (runtimeOnlyAttribute is null) return null;

        INamedTypeSymbol? persistOnSaveAttribute = compilation.GetTypeByMetadataName("Murder.Attributes.PersistOnSaveAttribute");
        if (persistOnSaveAttribute is null) return null;

        INamedTypeSymbol? doNotPersistOnSaveAttribute = compilation.GetTypeByMetadataName("Murder.Attributes.DoNotPersistOnSaveAttribute");
        if (doNotPersistOnSaveAttribute is null) return null;

        INamedTypeSymbol? doNotPersistEntityOnSaveAttribute = compilation.GetTypeByMetadataName("Murder.Attributes.DoNotPersistEntityOnSaveAttribute");
        if (doNotPersistEntityOnSaveAttribute is null) return null;

        return new MurderTypeSymbols()
        {
            ComponentInterface = componentInterface,
            MessageInterface = messageInterface,
            StateMachineClass = stateMachineClass,
            StateMachineComponentInterface = stateMachineComponentInterface,
            InteractionInterface = interactionInterface,
            InteractiveComponentInterface = interactiveComponentInterface,
            GameAssetClass = gameAssetClass,
            SerializerContextInterface = murderSerializer,
            ComplexDictionaryClass = complexDictionaryClass,
            SerializeFieldAttribute = serializeFieldAttribute,
            ShowInEditorFieldAttribute = showInEditorAttribute,
            IgnoreFieldAttribute = ignoreFieldAttribute,
            JsonSerializableAttribute = jsonSerializableAttribute,
            RuntimeOnlyAttribute = runtimeOnlyAttribute,
            PersistOnSaveAttribute = persistOnSaveAttribute,
            DoNotPersistOnSaveAttribute = doNotPersistOnSaveAttribute,
            DoNotPersistEntityOnSaveAttribute = doNotPersistEntityOnSaveAttribute
        };
    }
}
