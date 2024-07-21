using Bang;
using ImGuiNET;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Editor.Components;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using Murder.Prefabs;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<IShape>))]
    internal class ShapeArrayField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            ImmutableArray<IShape> shapes = (ImmutableArray<IShape>)fieldValue!;
            if (shapes.IsDefault) shapes = ImmutableArray<IShape>.Empty;

            ImGui.PushID($"Add ${member.Member.ReflectedType}");

            if (Add(out IShape? element))
            {
                shapes = shapes.Add(element);

                modified = true;
            }

            ImGui.PopID();

            if (modified || shapes.Length == 0)
            {
                return (modified, shapes);
            }

            // Index for slicing into convex shapes
            int sliceIndex = -1;

            for (int index = 0; index < shapes.Length; index++)
            {
                ImGui.PushID($"{member.Member.ReflectedType}_{index}");
                element = shapes[index];

                ImGui.BeginGroup();
                ImGuiHelpers.IconButton('', $"{member.Name}_alter", Game.Data.GameProfile.Theme.Accent);

                if (ImGui.IsItemHovered())
                {
                    ImGui.OpenPopup($"{member.Member.ReflectedType}_{index}_extras");
                    ImGui.SetNextWindowPos(ImGui.GetItemRectMin() + new System.Numerics.Vector2(-12, -2));
                }

                if (ImGui.BeginPopup($"{member.Member.ReflectedType}_{index}_extras"))
                {
                    ImGui.BeginGroup();
                    if (ImGuiHelpers.IconButton('', $"{member.Name}_remove", Game.Data.GameProfile.Theme.HighAccent))
                    {
                        shapes = shapes.RemoveAt(index);
                        modified = true;
                    }
                    ImGuiHelpers.HelpTooltip("Remove");

                    if (ImGuiHelpers.IconButton('', $"{member.Name}_duplicate", Game.Data.GameProfile.Theme.HighAccent))
                    {
                        shapes = shapes.Insert(index, shapes[index]);
                        modified = true;
                    }
                    ImGuiHelpers.HelpTooltip("Duplicate");

                    if (ImGuiHelpers.IconButton('', $"{member.Name}_move_up", Game.Data.GameProfile.Theme.HighAccent))
                    {
                        if (index > 0)
                        {
                            shapes = shapes.RemoveAt(index).Insert(index - 1, shapes[index]);
                            modified = true;
                        }
                    }
                    ImGuiHelpers.HelpTooltip("Move up");

                    if (ImGuiHelpers.IconButton('', $"{member.Name}_move_down", Game.Data.GameProfile.Theme.HighAccent))
                    {
                        if (index < shapes.Length - 1)
                        {
                            shapes = shapes.RemoveAt(index).Insert(index + 1, shapes[index]);
                            modified = true;
                        }
                    }
                    ImGuiHelpers.HelpTooltip("Move down");

                    ImGui.EndGroup();
                    if (!ImGui.IsWindowAppearing() && ImGui.IsWindowFocused() && !ImGui.IsMouseHoveringRect(ImGui.GetWindowPos(), ImGui.GetWindowPos() + ImGui.GetWindowSize()))
                        ImGui.CloseCurrentPopup();
                    ImGui.EndPopup();
                }

                ImGui.SameLine();

                (bool elementModified, bool sliceIt) = DrawElement(ref element);
                if (elementModified)
                {
                    shapes = shapes.SetItem(index, element!);
                    modified = true;
                }

                if (sliceIt)
                {
                    sliceIndex = index;
                }

                ImGui.EndGroup();
                ImGui.PopID();
            }

            if (sliceIndex >= 0)
            {
                if (shapes[sliceIndex] is PolygonShape polygon)
                {
                    shapes = shapes.RemoveAt(sliceIndex);

                    var convexes = Polygon.EarClippingTriangulation(polygon.Polygon);
                    var toAdd = new PolygonShape[convexes.Count];
                    for (int i = 0; i < convexes.Count; i++)
                    {
                        toAdd[i] = new PolygonShape(convexes[i]);
                    }

                    shapes = shapes.AddRange(toAdd);
                }
                modified = true;
            }

            return (modified, shapes);
        }

        private bool Add([NotNullWhen(true)] out IShape? element)
        {
            element = null;

            if (SearchBox.SearchShapes() is Type shapeType)
            {
                if (Activator.CreateInstance(shapeType) is IShape shape)
                {
                    element = shape;
                    return true;
                }
            }

            return false;
        }

        private (bool modified, bool sliceIt) DrawElement(ref IShape? element)
        {
            if (element is PolygonShape polygonShape)
            {
                var polygon = polygonShape.Polygon;

                if (polygon.IsConvex())
                {
                    ImGui.Text($"Polygon has {polygon.Vertices.Length} points, and it's convex");
                }
                else
                {
                    ImGui.TextColored(Game.Profile.Theme.Warning, $"POLYGON IS NOT CONVEX");
                    if (ImGui.Button("Slice it!"))
                    {
                        return (false, sliceIt: true);
                    }
                    ImGui.Text($"Polygon has {polygon.Vertices.Length} points.");
                }

                return (false, false);
            }
            else
            {
                return (CustomComponent.ShowEditorOf(ref element), false);
            }
        }
    }
}