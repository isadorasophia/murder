using Microsoft.CodeAnalysis;

namespace Murder.Serializer.Metadata;

internal readonly struct MurderTypeSymbols
{
    public INamedTypeSymbol ComponentInterface { get; }
    public INamedTypeSymbol MessageInterface { get; }
    public INamedTypeSymbol StateMachineClass { get; }
    public INamedTypeSymbol InteractionInterface { get; }
    public INamedTypeSymbol GameAssetClass { get; }
    public INamedTypeSymbol SerializerInterface { get; }
    public INamedTypeSymbol SerializableAttribute { get; }

    private MurderTypeSymbols(
        INamedTypeSymbol componentInterface,
        INamedTypeSymbol messageInterface,
        INamedTypeSymbol stateMachineClass,
        INamedTypeSymbol interactionInterface,
        INamedTypeSymbol gameAssetClass,
        INamedTypeSymbol serializerInterface,
        INamedTypeSymbol serializableAttribute)
    {
        ComponentInterface = componentInterface;
        MessageInterface = messageInterface;
        StateMachineClass = stateMachineClass;
        InteractionInterface = interactionInterface;
        GameAssetClass = gameAssetClass;
        SerializerInterface = serializerInterface;
        SerializableAttribute = serializableAttribute;
    }

    public static MurderTypeSymbols? FromCompilation(Compilation compilation)
    {
        // Bail if IComponent is not resolvable.
        INamedTypeSymbol? componentInterface = compilation.GetTypeByMetadataName("Bang.Components.IComponent");
        if (componentInterface is null) return null;

        // Bail if IMessage is not resolvable.
        INamedTypeSymbol? messageInterface = compilation.GetTypeByMetadataName("Bang.Components.IMessage");
        if (messageInterface is null) return null;

        // Bail if StateMachine is not resolvable.
        INamedTypeSymbol? stateMachineClass = compilation.GetTypeByMetadataName("Bang.StateMachines.StateMachine");
        if (stateMachineClass is null) return null;

        // Bail if IInteraction is not resolvable.
        INamedTypeSymbol? interactionInterface = compilation.GetTypeByMetadataName("Bang.Interactions.IInteraction");
        if (interactionInterface is null) return null;

        // Bail if GameAsset is not resolvable.
        INamedTypeSymbol? gameAssetClass = compilation.GetTypeByMetadataName("Murder.Assets.GameAsset");
        if (gameAssetClass is null) return null;

        // Bail if IMurderSerializer is not resolvable.
        INamedTypeSymbol? murderSerializer = compilation.
            GetTypeByMetadataName("Murder.Serialization.IMurderSerializer");
        if (murderSerializer is null) return null;

        // Bail if SerializableAttribute is not resolvable.
        INamedTypeSymbol? murderSerializeAttribute = compilation.GetTypeByMetadataName("Bang.SerializeAttribute");
        if (murderSerializeAttribute is null) return null;

        return new MurderTypeSymbols(
            componentInterface,
            messageInterface,
            stateMachineClass,
            interactionInterface,
            gameAssetClass,
            murderSerializer,
            murderSerializeAttribute
        );
    }
}
