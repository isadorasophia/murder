using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Cutscenes;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Services;
using Murder.Editor.Utilities;
using Murder.Helpers;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Editor.Systems
{
    [WorldEditor(startActive: true)]
    [OnlyShowOnDebugView]
    [Filter(kind: ContextAccessorKind.Read, typeof(ColliderComponent), typeof(ITransformComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(CutsceneAnchorsComponent), typeof(SoundParameterComponent))] // Skip cutscene and sounds.
    public class DebugColliderRenderSystem : IMurderRenderSystem
    {
        private bool _wasClicking = false;

        public void Draw(RenderContext render, Context context)
        {
            DrawImpl(render, context, allowEditingByDefault: false, ref _wasClicking);
        }

        /// <param name="allowEditingByDefault">Whether we only allow editing if the collider component is open.</param>
        public static void DrawImpl(RenderContext render, Context context, bool allowEditingByDefault, ref bool wasClicking)
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

                bool showHandles = allowEditingByDefault ? true : e.HasComponent<ShowColliderHandlesComponent>();
                bool usingCursor = false;

                bool isSolid = collider.Layer.HasFlag(CollisionLayersBase.SOLID);
                for (int shapeIndex = 0; shapeIndex < collider.Shapes.Length; shapeIndex++)
                {
                    IShape shape = collider.Shapes[shapeIndex];
                    IShape? newShape;

                    // Don't draw out of bounds!
                    if (!shape.GetBoundingBox().AddPosition(globalPosition.Point).Touches(render.Camera.SafeBounds))
                        continue;

                    // Draw bounding box
                    // RenderServices.DrawRectangleOutline(render.DebugBatch, shape.GetBoundingBox().AddPosition(globalPosition.Vector2), Color.WarmGray);

                    if (editor.EditorHook.KeepOriginalColliderShapes)
                    {
                        if (DrawOriginalHandles(
                            shape,
                            $"offset_{e.EntityId}_{shapeIndex}",
                            globalPosition,
                            render,
                            editor.EditorHook,
                            showHandles,
                            color,
                            /* flip: isSolid, */
                            e.TryGetFacing() is FacingComponent facing && facing.Direction.Flipped(), out var newShapeResult))
                        {
                            newShape = newShapeResult;
                            usingCursor = true;
                        }
                        else
                        {
                            newShape = null;
                        }
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
                        wasClicking = true;
                        editor.EditorHook.UsingCursor = true;
                    }
                    else if (wasClicking)
                    {
                        if (usingCursor)
                        {
                            editor.EditorHook.UsingCursor = true;
                        }
                        else
                        {
                            wasClicking = false;
                        }
                    }
                    else if (allowEditingByDefault)
                    {
                        editor.EditorHook.UsingCursor = false;
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
            bool _ /* flip */)
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
                poly.Draw(render.DebugBatch, globalPosition.Vector2, false, color);
            }

            return null;
        }

        private static bool DrawOriginalHandles(
            IShape shape,
            string id,
            IMurderTransformComponent globalPosition,
            RenderContext render,
            EditorHook hook,
            bool showHandles,
            Color color,
            bool solid,
            out IShape newShape)
        {
            newShape = shape;

            Vector2 cursorPosition = hook.CursorWorldPosition;

            Batch2D batch = render.DebugBatch;
            color = solid ? color : color * 0.5f;
            switch (shape)
            {
                case PolygonShape polyShape:
                    var poly = polyShape.Polygon;
                    if (poly.Vertices.IsDefaultOrEmpty)
                        return false;

                    if (showHandles)
                    {
                        if (EditorServices.PolyHandle(id, render, globalPosition.Vector2, cursorPosition, poly, color, out var newPolygonResult))
                        {
                            newShape = new PolygonShape(newPolygonResult);
                            return true;
                        }
                    }
                    else
                    {
                        RenderServices.DrawPoints(batch, globalPosition.Point, poly.Vertices.AsSpan(), color, 1);
                    }
                    break;

                case LineShape line:
                    RenderServices.DrawLine(batch, line.Start + globalPosition.Vector2, line.End + globalPosition.Vector2, color);
                    break;
                case BoxShape box:
                    RenderServices.DrawRectangleOutline(batch, box.Rectangle.AddPosition(globalPosition.ToVector2()), color);

                    if (showHandles)
                    {
                        if (EditorServices.BoxHandle(id, render,
                            cursorPosition, box.Rectangle + globalPosition.Point, color, out IntRectangle newRectangle))
                        {
                            newShape = new BoxShape(box.Origin, (newRectangle.TopLeft - globalPosition.Vector2).Point(), newRectangle.Width, newRectangle.Height);
                            return true;
                        }
                    }
                    break;
                case CircleShape circle:
                    RenderServices.DrawCircle(batch, circle.Offset + globalPosition.Vector2, circle.Radius, 24, color);
                    break;

                case LazyShape lazy:
                    RenderServices.DrawCircle(batch, lazy.Offset + globalPosition.Vector2, lazy.Radius, 6, color);
                    break;
            }

            return false;
        }
    }
}
