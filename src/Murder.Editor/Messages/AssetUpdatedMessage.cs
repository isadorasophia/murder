using Bang.Components;

namespace Murder.Editor.Messages;

public readonly struct AssetUpdatedMessage : IMessage
{
    public readonly Type UpdatedComponent;

    public AssetUpdatedMessage(Type updatedComponent)
    {
        UpdatedComponent = updatedComponent;
    }
}
