using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Microsoft.Xna.Framework.Input;
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
        private readonly static int _hash = typeof(DebugColliderRenderSystem).GetHashCode();
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
            bool usingCursor = false;

            foreach (Entity e in context.Entities)
            {
                ColliderComponent collider = e.GetCollider();
                IMurderTransformComponent globalPosition = e.GetGlobalTransform();

                Color color = collider.DebugColor * .6f;
                ImmutableArray<IShape> newShapes = [];

                bool showHandles = allowEditingByDefault ? true : e.HasComponent<ShowColliderHandlesComponent>();

                bool isSolid = collider.Layer.HasFlag(CollisionLayersBase.SOLID);
                for (int shapeIndex = 0; shapeIndex < collider.Shapes.Length; shapeIndex++)
                {
                    IShape shape = collider.Shapes[shapeIndex];
                    IShape? newShape;

                    // Don't draw out of bounds!
                    if (!shape.GetBoundingBox().AddPosition(globalPosition.Point).Touches(render.Camera.SafeBounds))
                        continue;

                    // Draw bounding box
                    if (DrawOriginalHandles(
                        shape,
                        $"offset_{e.EntityId}_{shapeIndex}",
                        globalPosition,
                        render,
                        editor.EditorHook,
                        showHandles,
                        color * (isSolid ? 1f : 0.5f),
                        e.TryGetFacing() is FacingComponent facing && facing.Direction.Flipped(), out var newShapeResult))
                    {
                        newShape = newShapeResult;
                        usingCursor = true;
                    }
                    else
                    {
                        newShape = null;
                    }

                    if (newShape is not null)
                    {
                        newShapes = collider.Shapes.SetItem(shapeIndex, newShape);
                        wasClicking = true;
                    }
                    else if (wasClicking && !usingCursor)
                    {
                        wasClicking = false;   
                    }
                    else if (allowEditingByDefault)
                    {
                        usingCursor = false;
                    }
                }

                if (!newShapes.IsDefaultOrEmpty)
                {
                    e.SetCollider(new ColliderComponent(newShapes, collider.Layer, collider.DebugColor));
                }
            }

            if (usingCursor)
            {
                editor.EditorHook.CursorIsBusy.Add(_hash);
            }
            else
            {
                editor.EditorHook.CursorIsBusy.Remove(_hash);
            }
        }

        private static IShape? DrawPolyHandles(
            IShape shape,
            string id,
            IMurderTransformComponent globalPosition,
            RenderContext render,
            Vector2? cursorPosition,
            bool showHandles,
            Color color,
            bool _ /* flip */)
        {
            var poly = shape.GetPolygon().Polygon;
            if (poly.Vertices.IsDefaultOrEmpty)
                return null;
            if (showHandles)
            {
                if (cursorPosition != null && EditorServices.DrawPolygonHandles(poly, render, globalPosition.Vector2, cursorPosition.Value, id, color, out var newPoly))
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

            if (hook.CursorWorldPosition is not Point cursorPosition)
                return false;


            bool isReadonly = hook.UsingGui;

            Batch2D batch = render.DebugBatch;
            color = color * 0.85f;
            switch (shape)
            {
                case PolygonShape polyShape:
                    var poly = polyShape.Polygon;
                    if (poly.Vertices.IsDefaultOrEmpty)
                        return false;

                    if (showHandles)
                    {
                        if (EditorServices.PolyHandle(id, render, globalPosition.Vector2, cursorPosition, poly, Color.White, color, out var newPolygonResult))
                        {

                            if (!isReadonly)
                            {
                                newShape = new PolygonShape(newPolygonResult);
                                return true;
                            }
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
                            if (!isReadonly)
                            {
                                newShape = new BoxShape(box.Origin, (newRectangle.TopLeft - globalPosition.Vector2).Point(), newRectangle.Width, newRectangle.Height);
                                bool hasChanges = !newShape.Equals(shape);
                                return true;
                            }
                        }
                    }
                    break;
                case CircleShape circle:
                    RenderServices.DrawCircleOutline(batch, circle.Offset + globalPosition.Vector2, circle.Radius, 24, color);
                    break;

                case LazyShape lazy:
                    RenderServices.DrawCircleOutline(batch, lazy.Offset + globalPosition.Vector2, lazy.Radius, 6, color);
                    break;
            }

            return false;
        }
    }
}