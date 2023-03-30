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
                    IShape? newShape;
                    
                    // Don't draw out of bounds!
                    if (!shape.GetBoundingBox().AddPosition(globalPosition.Point).Touches(render.Camera.SafeBounds))
                        continue;

                    if (editor.EditorHook.KeepOriginalColliderShapes)
                    {
                        newShape = DrawOriginalHandles(
                            shape,
                            $"offset_{e.EntityId}_{shapeIndex}",
                            globalPosition,
                            render,
                            editor.EditorHook.CursorWorldPosition,
                            showHandles,
                            color,
                            e.TryGetFacing() is FacingComponent facing && facing.Direction.Flipped());
                    }
                    else
                    {
                        newShape = DrawPolyHandles(
                            shape,
                            $"offset_{e.EntityId}_{shapeIndex}",
                            globalPosition,
                            render,
                            editor.EditorHook.CursorWorldPosition,
                            showHandles,
                            color,
                            e.TryGetFacing() is FacingComponent facing && facing.Direction.Flipped());
                    }

                    if (newShape is not null)
                    {
                        newShapes = collider.Shapes.SetItem(shapeIndex, newShape);
                    }
                }

                if (!newShapes.IsDefaultOrEmpty)
                {
                    e.SetCollider(new ColliderComponent(newShapes, collider.Layer, collider.DebugColor));
                }
            }
        }

        private static IShape? DrawPolyHandles(
            IShape shape,
            string id,
            IMurderTransformComponent globalPosition,
            RenderContext render,
            Vector2 cursorPosition,
            bool showHandles,
            Color color,
            bool flip)
        {
            var poly = shape.GetPolygon().Polygon;
            if (poly.Vertices.IsDefaultOrEmpty)
                return null;
            if (showHandles)
            {
                if (EditorServices.DrawPolygonHandles(poly, render, globalPosition.Vector2, cursorPosition, id, color, out var newPoly))
                {
                    return new PolygonShape(newPoly);
                }
            }
            else
            {
                poly.Draw(render.DebugSpriteBatch, globalPosition.Vector2, false, color);
            }

            return null;
        }

        private IShape? DrawOriginalHandles(
            IShape shape,
            string id,
            IMurderTransformComponent globalPosition,
            RenderContext render,
            Vector2 cursorPosition,
            bool showHandles,
            Color color,
            bool flip)
        {
            IShape? newShape = null;
            switch (shape)
            {
                case PolygonShape polyShape:
                    var poly = polyShape.Polygon;
                    if (poly.Vertices.IsDefaultOrEmpty)
                        return null;

                    DrawPolyHandles(shape, id, globalPosition, render, cursorPosition, showHandles, color, flip);
                    break;

                case LineShape line:
                    RenderServices.DrawLine(render.DebugSpriteBatch, line.Start + globalPosition.Vector2, line.End + globalPosition.Vector2, color);
                    break;
                case BoxShape box:
                    RenderServices.DrawRectangleOutline(render.DebugSpriteBatch, box.Rectangle.AddPosition(globalPosition.ToVector2()), color);

                    if (showHandles)
                    {
                        var halfBox = box.Size / 2f;
                        if (EditorServices.DrawHandle(id, render,
                            cursorPosition, globalPosition.Vector2 + box.Offset + halfBox, color, out Vector2 newPosition))
                        {
                            newShape = new BoxShape(box.Origin, (newPosition - globalPosition.Vector2).Point - halfBox, box.Width, box.Height);
                        }

                        if (EditorServices.DrawHandle($"{id}_TL", render,
                            cursorPosition, globalPosition + box.Offset, color, out var newTopLeft))
                        {
                            newShape = box.ResizeTopLeft(newTopLeft - globalPosition.Vector2);
                        }

                        if (EditorServices.DrawHandle($"{id}_BR", render,
                            cursorPosition, globalPosition.Vector2 + box.Offset + box.Size, color, out var newBottomRight))
                        {
                            newShape = box.ResizeBottomRight(newBottomRight - globalPosition.Vector2);
                        }
                    }
                    break;
                case CircleShape circle:
                    RenderServices.DrawCircle(render.DebugSpriteBatch, circle.Offset + globalPosition.Vector2, circle.Radius, 24, color);
                    break;

                case LazyShape lazy:
                    RenderServices.DrawCircle(render.DebugSpriteBatch, lazy.Offset + globalPosition.Vector2, lazy.Radius, 6, color);
                    break;
            }

            return newShape;
        }
    }
}
