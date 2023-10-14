using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Particles;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(Emitter))]
    internal class EmitterField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;

            Emitter emitter = (Emitter)fieldValue!;

            using TableMultipleColumns table = new($"emitter", flags: ImGuiTableFlags.SizingFixedFit, -1, 400);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            ImGuiHelpers.ColorIcon('\uf496', Game.Profile.Theme.Faded);

            ImGuiHelpers.HelpTooltip("Maximum total of particles that this emitter may have at once.");

            ImGui.TableNextColumn();
            ImGui.PushItemWidth(-1);

            if (DrawValue(ref emitter, nameof(Emitter.MaxParticlesPool)))
            {
                modified = true;
            }

            ImGui.PopItemWidth();
            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            ImGui.Text($"{nameof(Emitter.Shape)}:");
            DrawTooltip(typeof(Emitter), nameof(Emitter.Shape));

            ImGui.TableNextColumn();
            ImGui.PushItemWidth(-1);

            EmitterShape shape = emitter.Shape;
            if (DrawValue(ref shape, nameof(EmitterShape.Kind)))
            {
                emitter = emitter.WithShape(shape);
                modified = true;
            }

            ImGui.PopItemWidth();

            string? targetField = default;
            switch (shape.Kind)
            {
                case EmitterShapeKind.Rectangle:
                    targetField = nameof(EmitterShape.Rectangle);
                    break;

                case EmitterShapeKind.Line:
                    targetField = nameof(EmitterShape.Line);
                    break;

                case EmitterShapeKind.Circle:
                case EmitterShapeKind.CircleOutline:
                    targetField = nameof(EmitterShape.Circle);
                    break;
            }

            if (targetField is not null && DrawValue(ref shape, targetField))
            {
                emitter = emitter.WithShape(shape);
                modified = true;
            }

            if (modified)
            {
                fieldValue = emitter;
            }

            modified |= CustomComponent.DrawAllMembers(
                fieldValue,
                exceptForMembers: new HashSet<string>() { nameof(Emitter.MaxParticlesPool), nameof(Emitter.Shape) });

            return (modified, fieldValue);
        }

        private static void DrawTooltip(Type t, string field)
        {
            if (ReflectionHelper.FindTooltip(t, field) is string tooltip)
            {
                ImGuiHelpers.HelpTooltip(tooltip);
            }
        }
    }
}