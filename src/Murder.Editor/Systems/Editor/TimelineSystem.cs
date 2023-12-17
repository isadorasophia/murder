using Bang.Contexts;
using Bang.Systems;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Editor.Utilities.Attributes;

namespace Murder.Editor.Systems.Editor;

[SpriteEditor]
[Filter(typeof(EditorComponent))]
internal class TimelineSystem : IUpdateSystem
{
    public void Update(Context context)
    {
        if (!context.HasAnyEntity || 
            context.Entity.TryGetComponent<EditorComponent>() is not EditorComponent component ||
            component.EditorHook is not TimelineEditorHook hook)
        {
            return;
        }

        if (hook.IsPaused)
        {
            return;
        }

        hook.Time += Game.DeltaTime;
    }
}
