using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using ImGuiNET;
using Murder.Components;
using Murder.Components.Cutscenes;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Core.Physics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Messages;
using Murder.Editor.Services;
using Murder.Editor.Utilities;
using Murder.Helpers;
using Murder.Services;
using Murder.Utilities;
using SkiaSharp;
using System.Collections.Immutable;
using System.Numerics;
using System.Xml.Linq;

namespace Murder.Editor.Systems
{
    [WorldEditor(startActive: true)]
    [OnlyShowOnDebugView]
    [Filter(kind: ContextAccessorKind.Read, typeof(ColliderComponent), typeof(ITransformComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(CutsceneAnchorsComponent), typeof(SoundParameterComponent))] // Skip cutscene and sounds.
    public class DebugColliderRenderSystem : IMurderRenderSystem, IGuiSystem
    {
        private static int? _wasEditing = null;
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

            if (Game.Input.Down(MurderInputButtons.Space))
            {
                return;
            }

            bool usingCursor = false;
            EditorHook hook = editor.EditorHook;
            
            foreach (Entity e in context.Entities)
            {
                if (!e.HasCollider() || !e.HasTransform())
                {
                    continue;
                }

                ColliderComponent collider = e.GetCollider();
                IMurderTransformComponent globalPosition = e.GetGlobalTransform();

                Color color = collider.DebugColor * .6f;
                ImmutableArray<IShape> newShapes = [];

                bool showHandles = allowEditingByDefault ? true :
                    (!hook.HideEditIds.Contains(e.EntityId)) &&
                    (hook.EditorMode == EditorHook.EditorModes.EditMode && (!hook.CanSwitchModes || hook.IsEntitySelectedOrParent(e))) &&
                    (hook.CursorIsBusy.Count==1 && hook.CursorIsBusy.Contains(typeof(DebugColliderRenderSystem)) || !hook.CursorIsBusy.Any());

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
                    _wasEditing = e.EntityId;
                }
                else if (_wasEditing == e.EntityId)
                {
                    e.SendMessage(new AssetUpdatedMessage(typeof(ColliderComponent)));
                }
            }

            if (usingCursor)
            {
                editor.EditorHook.CursorIsBusy.Add(typeof(DebugColliderRenderSystem));
            }
            else
            {
                editor.EditorHook.CursorIsBusy.Remove(typeof(DebugColliderRenderSystem));
            }
        }


        private static bool DrawOriginalHandles(
            IShape shape,
            string id,
            IMurderTransformComponent globalPosition,
            RenderContext render,
            EditorHook hook,
            bool showHandles,
            Color color,
            bool _,
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
                            cursorPosition, box.Rectangle + globalPosition.Point, color, out IntRectangle newRectangle, false))
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

        public void DrawGui(RenderContext render, Context context)
        {
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
            if (hook.EditorMode != EditorHook.EditorModes.EditMode)
            {
                return;
            }

            if (hook.AllSelectedEntities.FirstOrDefault().Value is not Entity SelectedEntity)
            {
                return;
            }

            if (ImGui.BeginPopupContextItem("GameplayContextMenu", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.NoReopen))
            {
                ImGui.SeparatorText("Colliders");
                if (DrawCreateShapeMenu(SelectedEntity, hook))
                {
                    ImGui.CloseCurrentPopup();
                }
                if (SelectedEntity.Children.Any())
                {
                    ImGui.Separator();
                    ImGui.TextColored(Game.Profile.Theme.Accent, "Children");
                    foreach (var child in SelectedEntity.FetchChildrenWithNames)
                    {
                        if (context.World.TryGetEntity(child.Key) is Entity childEntity)
                        {
                            if (childEntity.HasCollider())
                            {
                                ImGui.TextColored(Game.Profile.Theme.Faded, child.Value ?? $"Child {child.Key}");
                                ImGui.SameLine();
                                if (DrawCreateShapeMenu(childEntity, hook))
                                {
                                    ImGui.CloseCurrentPopup();
                                }
                            }
                        }
                    }
                }



                ImGui.EndPopup();
            }
        }

        private static Type DrawShapeButtons()
        {
            Type? result = null;
            if (ImGui.Button("Box"))
            {
                result = typeof(BoxShape);
            }
            ImGui.SameLine();
            if (ImGui.Button("Polygon"))
            {
                result = typeof(PolygonShape);
            }
            ImGui.SameLine();
            if (ImGui.Button("Circle"))
            {
                result = typeof(CircleShape);
            }
            //ImGui.SameLine();
            //if (ImGui.Button("Line"))
            //{
            //    result = typeof(LineShape);
            //}
            ImGui.SameLine();
            if (ImGui.Button("Lazy"))
            {
                result = typeof(LazyShape);
            }

            return result!;
        }

        private static IShape? CreateShapeFromType(Type type, Point offset)
        {
            if (type == typeof(BoxShape))
            {
                return new BoxShape(Vector2.Zero, offset, 16, 16);
            }
            else if (type == typeof(PolygonShape))
            {
                return new PolygonShape(Polygon.DIAMOND.AddPosition(offset));
            }
            else if (type == typeof(CircleShape))
            {
                return new CircleShape(16, offset);
            }
            else if (type == typeof(LineShape))
            {
                return new LineShape(offset, offset + new Point(32, 32));
            }
            else if (type == typeof(LazyShape))
            {
                return new LazyShape(16, offset);
            }
            return null;
        }

        private static bool DrawCreateShapeMenu(Entity entity, EditorHook hook)
        {
            if (entity.TryGetCollider() is ColliderComponent collider)
            {
                if (DrawShapeButtons() is Type shapeType)
                {
                    Vector2 position = entity.GetGlobalTransform().Vector2;
                    Point offset = (hook.LastCursorWorldPosition - position).Point();

                    if (CreateShapeFromType(shapeType, offset) is IShape newShape)
                    {
                        entity.SetCollider(collider with { Shapes = collider.Shapes.Add(newShape) });

                        entity.SendMessage(new AssetUpdatedMessage(typeof(ColliderComponent)));
                        return true;
                    }
                }

            }
            return false;
        }
    }
}