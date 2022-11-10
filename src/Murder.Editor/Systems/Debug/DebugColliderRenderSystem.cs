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

namespace Murder.Editor.Systems
{
    [WorldEditor]
    [OnlyShowOnDebugView]
    [Filter(kind: ContextAccessorKind.Read, typeof(ColliderComponent), typeof(PositionComponent))]
    public class DebugColliderRenderSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            if (context.World.TryGetUnique<EditorComponent>() is EditorComponent editor)
            {
                if (!editor.EditorHook.DrawCollisions)
                    return default;
            }
            else
            {
                return default;
            }

            foreach (Entity e in context.Entities)
            {
                ColliderComponent collider = e.GetCollider();
                IMurderTransformComponent position = e.GetGlobalTransform();

                Color color = collider.DebugColor;
                ImmutableArray<IShape> newShapes = ImmutableArray.Create<IShape>();

                bool showHandles = e.HasComponent<ShowColliderHandlesComponent>();

                for (int shapeIndex = 0; shapeIndex < collider.Shapes.Length; shapeIndex++)
                {
                    IShape shape = collider.Shapes[shapeIndex];

                    switch (shape)
                    {
                        case PolygonShape polyShape:
                            var poly = polyShape.Polygon;
                            if (poly.Vertices.IsDefaultOrEmpty)
                                continue;

                            for (int i = 0; i < poly.Vertices.Length - 1; i++)
                            {
                                Point pointA = poly.Vertices[i];
                                Point pointB = poly.Vertices[i + 1];
                                RenderServices.DrawLine(render.DebugSpriteBatch, pointA + position.Vector2, pointB + position.Vector2, color);
                            }

                            RenderServices.DrawLine(render.DebugSpriteBatch, poly.Vertices[poly.Vertices.Length - 1] + position.Vector2,
                                poly.Vertices[0] + position.Vector2, color);

                            break;

                        case LineShape line:
                            RenderServices.DrawLine(render.DebugSpriteBatch, line.Start + position.GetGlobal().Vector2, line.End + position.Vector2, color);
                            break;
                        case BoxShape box:
                            RenderServices.DrawRectangleOutline(render.DebugSpriteBatch, box.Rectangle.AddPosition(position.ToVector2()), color);

                            if (showHandles)
                            {
                                var halfBox = box.Size / 2f;
                                if (EditorServices.DrawHandle($"offset_{e.EntityId}_{shapeIndex}", render,
                                    editor.EditorHook.CursorWorldPosition, position.Vector2 + box.Offset + halfBox, color, out Vector2 newPosition))
                                {
                                    newShapes = collider.Shapes.SetItem(shapeIndex, new BoxShape(box.Origin, (newPosition - position.Vector2).Point - halfBox, box.Width, box.Height));
                                }

                                if (EditorServices.DrawHandle($"offset_{e.EntityId}_{shapeIndex}_TL", render,
                                    editor.EditorHook.CursorWorldPosition, position + box.Offset, color, out var newTopLeft))
                                {
                                    newShapes = collider.Shapes.SetItem(shapeIndex, box.ResizeTopLeft(newTopLeft - position.Vector2));
                                }

                                if (EditorServices.DrawHandle($"offset_{e.EntityId}_{shapeIndex}_BR", render,
                                    editor.EditorHook.CursorWorldPosition, position.Vector2 + box.Offset + box.Size, color, out var newBottomRight))
                                {
                                    newShapes = collider.Shapes.SetItem(shapeIndex, box.ResizeBottomRight(newBottomRight - position.Vector2));
                                }
                            }
                            break;
                        case CircleShape circle:
                            RenderServices.DrawCircle(render.DebugSpriteBatch, circle.Offset + position.GetGlobal().Vector2, circle.Radius, 24, color);
                            break;

                        case LazyShape lazy:
                            RenderServices.DrawCircle(render.DebugSpriteBatch, lazy.Offset + position.GetGlobal().Vector2, lazy.Radius, 6, color);
                            break;
                    }
                }

                if (!newShapes.IsDefaultOrEmpty)
                {
                    e.SetCollider(new ColliderComponent(newShapes, collider.Solid, collider.DebugColor));
                }
            }

            return default;
        }
    }
}
