using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;

namespace Murder.Editor.Systems
{
    [OnlyShowOnDebugView]
    [Filter(ContextAccessorFilter.AnyOf, typeof(IdTargetComponent), typeof(GuidToIdTargetComponent))]
    public class DebugShowInteractionsSystem : IMonoRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            EditorHook? editorHook = context.World.TryGetUnique<EditorComponent>()?.EditorHook;
            if (editorHook is null || !editorHook.DrawTargetInteractions)
            {
                return;
            }
            
            foreach (Entity e in context.Entities)
            {
                ShowTargetId(render, context.World, editorHook, e);
                ShowTargetIdCollection(render, context.World, editorHook, e);
            }
        }

        private void ShowTargetId(RenderContext render, World world, EditorHook hook, Entity e)
        {
            Point from = e.GetTransform().Point;

            int? targetId = default;
            if (e.HasIdTarget())
            {
                targetId = e.GetIdTarget().Target;
            }

            if (e.HasGuidToIdTarget())
            {
                Guid targetGuid = e.GetGuidToIdTarget().Target;
                targetId = hook.GetEntityIdForGuid?.Invoke(targetGuid);
            }

            if (targetId == null)
            {
                return;
            }

            ShowLine(render, world, from, targetId.Value);
        }

        private void ShowTargetIdCollection(RenderContext render, World world, EditorHook hook, Entity e)
        {
            Point from = e.GetTransform().Point;
            
            IEnumerable<int>? targets = default;
            if (e.HasIdTargetCollection())
            {
                targets = e.GetIdTargetCollection().Targets.Values;
            }

            if (e.HasGuidToIdTargetCollection())
            {
                List<int> listId = new();
                
                foreach ((_, Guid g) in e.GetGuidToIdTargetCollection().Targets)
                {
                    if (hook.GetEntityIdForGuid?.Invoke(g) is int targetId)
                    {
                        listId.Add(targetId);
                    }
                }

                targets = listId;
            }

            if (targets == null || targets.Count() == 0)
            {
                return;
            }

            foreach (int target in targets)
            {
                ShowLine(render, world, from, target);
            }
        }

        private void ShowLine(RenderContext render, World world, Point from, int targetTo)
        {
            Entity? target = world.TryGetEntity(targetTo);
            if (target == null || !target.HasTransform())
            {
                return;
            }

            RenderServices.DrawLine(render.DebugFxSpriteBatch, from, target.GetTransform().Point, Color.White);
        }
    }
}
