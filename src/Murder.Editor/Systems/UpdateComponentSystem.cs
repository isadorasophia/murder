using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Attributes;
using Murder.Diagnostics;
using Murder.Editor.Components;
using Murder.Editor.Messages;
using Murder.Editor.Utilities;

namespace Murder.Editor.Systems;

[EditorSystem]
[Messager(typeof(AssetUpdatedMessage))]
public class UpdateComponentSystem : IMessagerSystem
{
    public void OnMessage(World world, Entity entity, IMessage message)
    {
        if (world.TryGetUnique<EditorComponent>() is not EditorComponent editor)
        {
            return;
        }

        EditorHook hook = editor.EditorHook;
        AssetUpdatedMessage assetUpdated = (AssetUpdatedMessage)message;

        Type type = assetUpdated.UpdatedComponent;

        _ = entity.TryGetComponent(type, out IComponent? c);
        hook.OnComponentModified?.Invoke(entity.EntityId, type, c);
    }
}