using Microsoft.CodeAnalysis;

namespace Murder.Generator.Metadata;

internal class MurderTypeSymbols
{
    public INamedTypeSymbol ComponentInterface { get; }
    public INamedTypeSymbol MessageInterface { get; }
    public INamedTypeSymbol StateMachineClass { get; }
    public INamedTypeSymbol InteractionInterface { get; }
    public INamedTypeSymbol GameAssetClass { get; }

    private MurderTypeSymbols(
        INamedTypeSymbol componentInterface,
        INamedTypeSymbol messageInterface,
        INamedTypeSymbol stateMachineClass,
        INamedTypeSymbol interactionInterface,
        INamedTypeSymbol gameAssetClass)
    {
        ComponentInterface = componentInterface;
        MessageInterface = messageInterface;
        StateMachineClass = stateMachineClass;
        InteractionInterface = interactionInterface;
        GameAssetClass = gameAssetClass;
    }

    public static MurderTypeSymbols? FromCompilation(Compilation compilation)
    {
        // Bail if IComponent is not resolvable.
        var componentInterface = compilation.GetTypeByMetadataName("Bang.Components.IComponent");
        if (componentInterface is null) return null;

        // Bail if IMessage is not resolvable.
        var messageInterface = compilation.GetTypeByMetadataName("Bang.Components.IMessage");
        if (messageInterface is null) return null;

        // Bail if StateMachine is not resolvable.
        var stateMachineClass = compilation.GetTypeByMetadataName("Bang.StateMachines.StateMachine");
        if (stateMachineClass is null) return null;

        // Bail if IInteraction is not resolvable.
        var interactionInterface = compilation.GetTypeByMetadataName("Bang.Interactions.IInteraction");
        if (interactionInterface is null) return null;

        // Bail if ComponentsLookup is not resolvable.
        var gameAssetClass = compilation.GetTypeByMetadataName("Murder.Assets.GameAsset");
        if (gameAssetClass is null) return null;

        return new MurderTypeSymbols(
            componentInterface,
            messageInterface,
            stateMachineClass,
            interactionInterface,
            gameAssetClass
        );
    }
}
