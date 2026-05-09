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
    private HashSet<int> _previousEntities = new HashSet<int>();
    private HashSet<int> _currentEntities = new HashSet<int>();
    private readonly List<NodeInfo<(Entity entity, SpriteComponent sprite, Vector2 renderPosition)>> _sprites = new();

    public void BeforeDraw(Context context)
    {
        UpdateQuadTree(context);
    }

#if false // Used for debugging.
    public void Draw(RenderContext render, Context context)
    {
        return;

        var camera = ((MonoWorld)context.World).Camera;
        Quadtree qt = context.World.GetUniqueQuadtree().Quadtree;
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

        Rectangle safeBounds = camera.SafeBounds;

        if (context.HasAnyEntity &&
            context.Entity.GetDisableSceneTransitionEffects().ForceCameraPosition is Vector2 position)
        {
            safeBounds = safeBounds.SetPosition(position);
        }

        Quadtree qt = Quadtree.GetOrCreateUnique(context.World);
        _sprites.Clear();
        qt.StaticRender.Retrieve(safeBounds, _sprites);

        _currentEntities.Clear();
        foreach (var node in _sprites)
        {
            if (safeBounds.Touches(node.BoundingBox) || node.EntityInfo.sprite.TargetSpriteBatch == Batches2D.UiBatchId)
            {
                node.EntityInfo.entity.SetInCamera(node.EntityInfo.renderPosition);
                _currentEntities.Add(node.Id);
            }
        }

        foreach (int id in _previousEntities)
        {
            if (!_currentEntities.Contains(id))
            {
                context.World.TryGetEntity(id)?.RemoveInCamera();
            }
        }

        // Copy _currentEntities to _previousEntities.
        (_previousEntities, _currentEntities) = (_currentEntities, _previousEntities);
    }

}