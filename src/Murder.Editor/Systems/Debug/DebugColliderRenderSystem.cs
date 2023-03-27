using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using System.Collections.Immutable;
using Murder.Editor.Attributes;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Editor.Components;
using Murder.Services;
using Murder.Core.Geometry;
using Murder.Utilities;
using Murder.Editor.Services;
using Bang.Components;
using Murder.Helpers;

namespace Murder.Editor.Systems
{
    [WorldEditor]
    [OnlyShowOnDebugView]
    [Filter(kind: ContextAccessorKind.Read, typeof(ColliderComponent), typeof(ITransformComponent))]
    public class DebugColliderRenderSystem : IMonoRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            if (context.World.TryGetUnique<EditorComponent>() is EditorComponent editor)
            {
                if (!editor.EditorHook.DrawCollisions)
                    return;
            }
            else
            {
                return;
            }

            foreach (Entity e in context.Entities)
            {
                ColliderComponent collider = e.GetCollider();
                IMurderTransformComponent globalPosition = e.GetGlobalTransform();

                Color color = collider.DebugColor;
                ImmutableArray<IShape> newShapes = ImmutableArray.Create<IShape>();

                bool showHandles = e.HasComponent<ShowColliderHandlesComponent>();

                for (int shapeIndex = 0; shapeIndex < collider.Shapes.Length; shapeIndex++)
                {
                    IShape shape = collider.Shapes[shapeIndex];

                    // Don't draw out of bounds!
                    if (!shape.GetBoundingBox().AddPosition(globalPosition.Point).Touches(render.Camera.SafeBounds))
                        continue;
                    
                    var poly = shape.GetPolygon().Polygon;
                    if (poly.Vertices.IsDefaultOrEmpty)
                        continue;
                    if (showHandles)
                    {
                        if (EditorServices.DrawPolygonHandles(poly, render, globalPosition.Vector2, editor.EditorHook.CursorWorldPosition, $"offset_{e.EntityId}_{shapeIndex}", color, out var newPoly))
                        {
                            newShapes = collider.Shapes.SetItem(shapeIndex, new PolygonShape(newPoly));
                        }
                    }
                    else
                    {
                        poly.Draw(render.DebugSpriteBatch, globalPosition.Vector2, false, color);
                    }
                }

                if (!newShapes.IsDefaultOrEmpty)
                {
                    e.SetCollider(new ColliderComponent(newShapes, collider.Layer, collider.DebugColor));
                }
            }
        }
    }
}
