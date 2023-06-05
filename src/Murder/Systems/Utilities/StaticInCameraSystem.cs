using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using Murder.Services;

namespace Murder.Systems;

[Filter(ContextAccessorFilter.None)]
internal class StaticInCameraSystem : IMonoPreRenderSystem
{
    private List<((Entity entity, SpriteComponent sprite, Vector2 renderPosition) entity, Rectangle boundingBox)> _sprites = new();
    private Rectangle _lastBounds = Rectangle.Empty;

    public void BeforeDraw(Context context)
    {
        UpdateQuadTree(context.World);
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

    private void UpdateQuadTree(World world)
    {
        var camera = ((MonoWorld)world).Camera;

        if (_lastBounds != camera.SafeBounds)
        {
            _lastBounds = camera.SafeBounds;
        }
        else
        {
            return;
        }
        
        Quadtree qt = world.GetUnique<QuadtreeComponent>().Quadtree;
        _sprites.Clear();
        qt.StaticRender.Retrieve(camera.SafeBounds, ref _sprites);
        foreach (var node in _sprites)
        {
            if (camera.Bounds.Touches(node.boundingBox) || node.entity.sprite.TargetSpriteBatch == TargetSpriteBatches.Ui)
            {
                node.entity.entity.SetInCamera(node.entity.renderPosition);
            }
            else
            {
                node.entity.entity.RemoveInCamera();
            }
        }
    }

}
