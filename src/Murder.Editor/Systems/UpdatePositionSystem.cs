using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Editor.Components;
using Murder.Editor.Messages;
using Murder.Editor.Utilities;

namespace Murder.Editor.Systems;

[Messager(typeof(AssetUpdatedMessage))]
public class UpdatePositionSystem : IMessagerSystem
{
    public void OnMessage(World world, Entity entity, IMessage message)
    {
        if (world.TryGetUnique<EditorComponent>() is not EditorComponent editor)
        {
            return;
        }
        EditorHook hook = editor.EditorHook;

        Type type = ((AssetUpdatedMessage)message).UpdatedComponent;

        if (type == typeof(PositionComponent))
        {
            hook.OnComponentModified?.Invoke(entity.EntityId, entity.GetTransform());
        }

        
    }
}