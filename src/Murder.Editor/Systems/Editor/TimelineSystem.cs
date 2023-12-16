using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
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

        if (context.World.GetEntitiesWith(typeof(SpriteComponent)).FirstOrDefault() is not Entity entity)
        {
            return;
        }

        SpriteComponent sprite = entity.GetSprite();

        hook.TimeSinceAnimationStarted = Game.Now - sprite.AnimationStartedTime;
    }
}
