using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;

namespace Murder.Editor.Systems
{
    [OnlyShowOnDebugView]
    [Filter(ContextAccessorFilter.AnyOf, typeof(TargetInteractionComponent), typeof(GuidToIdTargetInteractionComponent))]
    public class DebugShowInteractionsSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            EditorHook? editorHook = context.World.TryGetUnique<EditorComponent>()?.EditorHook;
            if (editorHook is null || !editorHook.DrawTargetInteractions)
            {
                return default;
            }
            
            foreach (Entity e in context.Entities)
            {
                int? targetId = default;
                
                if (e.HasTargetInteraction())
                {
                    targetId = e.GetTargetInteraction().Target;
                }

                if (e.HasGuidToIdTargetInteraction())
                {
                    Guid targetGuid = e.GetGuidToIdTargetInteraction().Target;
                    targetId = editorHook?.GetEntityIdForGuid?.Invoke(targetGuid);
                }

                if (targetId == null)
                {
                    continue;
                }

                Entity? target = context.World.TryGetEntity(targetId.Value);
                if (target == null || !target.HasTransform())
                {
                    continue;
                }
                
                RenderServices.DrawLine(render.DebugFxSpriteBatch, e.GetTransform().Point, target.GetTransform().Point, Color.White);
            }

            return default;
        }
    }
}
