using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;
using System.Collections.Immutable;
using System.Reflection.Emit;

namespace Murder.Editor.CustomComponents;

[CustomComponentOf(typeof(SpriteComponent))]
internal class SpriteComponentEditor : CustomComponent
{
    protected override bool DrawAllMembersWithTable(ref object target)
    {
        bool fileChanged = false;

        if (ImGui.BeginTable($"field_{target.GetType().Name}", 2,
            ImGuiTableFlags.SizingFixedSame | ImGuiTableFlags.BordersOuter | ImGuiTableFlags.BordersInnerH))
        {
            ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthFixed, -1, 0);
            ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthStretch, -1, 1);
            fileChanged |= DrawAllMembers(target);

            ImGui.TableNextColumn();
            ImGui.Text("Current Animation:");

            ImGui.TableNextColumn();


            var component = (SpriteComponent)target;
            if (Game.Data.TryGetAsset<SpriteAsset>(component.AnimationGuid) is SpriteAsset ase)
            {
                ImGui.SetNextItemWidth(-1);

                if (ImGuiHelpers.FilteredCombo($"##AnimationID", component.CurrentAnimation, ase.Animations.Keys.Order(), (name, index) => name, out string selectedAnimation))
                {
                    SelectAnimation(target, component, selectedAnimation);
                    fileChanged = true;
                }
            }

            ImGui.EndTable();
        }

        return fileChanged;
    }

    private static void SelectAnimation(object target, SpriteComponent component, string selectedAnimation)
    {
        ImmutableArray<string> nextAnimations;
        if (component.NextAnimations.Length == 0)
        {
            nextAnimations = [selectedAnimation];
        }
        else
        {
            nextAnimations = component.NextAnimations.SetItem(0, selectedAnimation);
        }

        target.GetType().GetProperty("NextAnimations")!.SetValue(target, nextAnimations);
    }
}