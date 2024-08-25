using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Editor.Attributes;
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
                if (ImGui.BeginCombo($"##AnimationID", component.CurrentAnimation))
                {
                    foreach (string value in ase.Animations.Keys.Order())
                    {
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            continue;
                        }

                        fileChanged = DrawAnimationOption(target, fileChanged, component, value, value);
                    }

                    fileChanged = DrawAnimationOption(target, fileChanged, component, "-empty-", string.Empty);
                    ImGui.EndCombo();
                }
            }

            ImGui.EndTable();
        }

        return fileChanged;
    }

    private static bool DrawAnimationOption(object target, bool fileChanged, SpriteComponent component, string label, string value)
    {
        if (ImGui.MenuItem(label))
        {
            ImmutableArray<string> nextAnimations;
            if (component.NextAnimations.Length == 0)
            {
                nextAnimations = [value];
            }
            else
            {
                nextAnimations = component.NextAnimations.SetItem(0, value);
            }

            target.GetType().GetProperty("NextAnimations")!.SetValue(target, nextAnimations);
            fileChanged = true;
        }

        return fileChanged;
    }
}