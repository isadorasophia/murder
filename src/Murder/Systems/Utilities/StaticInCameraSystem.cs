using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using System.Numerics;

namespace Murder.Systems;

[Filter(typeof(DisableSceneTransitionEffectsComponent))]
internal class StaticInCameraSystem : IMonoPreRenderSystem
{
    private readonly List<NodeInfo<(Entity entity, SpriteComponent sprite, Vector2 renderPosition)>> _sprites = new();
    private Rectangle _lastBounds = Rectangle.Empty;

    public void BeforeDraw(Context context)
    {
        UpdateQuadTree(context);
    }

#if false // Used for debugging.
    public void Draw(RenderContext render, Context context)
    {
        return;

        var camera = ((MonoWorld)context.World).Camera;
        Quadtree qt = context.World.GetUnique<QuadtreeComponent>().Quadtree;
        _sprites.Clear();
        qt.StaticRender.Retrieve(camera.SafeBounds, ref _sprites);
        foreach (var node in _sprites)
        {
            if (camera.Bounds.Touches(node.boundingBox) || node.entity.sprite.TargetSpriteBatch == TargetSpriteBatches.Ui)
            {
                RenderServices.DrawRectangleOutline(render.DebugSpriteBatch, node.boundingBox, Color.White);
            }
        }
    }
#endif

    private void UpdateQuadTree(Context context)
    {
        Camera2D camera = ((MonoWorld)context.World).Camera;

        Rectangle bounds = camera.Bounds;
        Rectangle safeBounds = camera.SafeBounds;

        if (context.HasAnyEntity && 
            context.Entity.GetDisableSceneTransitionEffects().OverrideCameraPosition is Vector2 position)
        {
            bounds = bounds.SetPosition(position);
            safeBounds = safeBounds.SetPosition(position);
        }

        if (_lastBounds != safeBounds)
        {
            _lastBounds = safeBounds;
        }
        else
        {
            return;
        }
        
        Quadtree qt = Quadtree.GetOrCreateUnique(context.World);
        _sprites.Clear();
        qt.StaticRender.Retrieve(safeBounds, _sprites);
        foreach (var node in _sprites)
        {
            if (bounds.Touches(node.BoundingBox) || node.EntityInfo.sprite.TargetSpriteBatch == Batches2D.UiBatchId)
            {
                node.EntityInfo.entity.SetInCamera(node.EntityInfo.renderPosition);
            }
            else
            {
                node.EntityInfo.entity.RemoveInCamera();
            }
        }
    }

}
