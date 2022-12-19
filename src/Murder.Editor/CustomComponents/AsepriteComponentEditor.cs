using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Editor.Attributes;
using static Murder.Editor.Data.Graphics.Aseprite;

namespace Murder.Editor.CustomComponents
{
    [CustomComponentOf(typeof(AsepriteComponent))]
    internal class AsepriteComponentEditor : CustomComponent
    {
        protected override bool DrawAllMembersWithTable(ref object target)
        {
            bool fileChanged = false;

            if (ImGui.BeginTable($"field_{target.GetType().Name}", 2,
                ImGuiTableFlags.SizingFixedSame | ImGuiTableFlags.BordersOuter))
            {
                ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthFixed, -1, 0);
                ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthStretch, -1, 1);
                fileChanged |= DrawAllMembers(target);

                var component = (AsepriteComponent)target;
                if (Game.Data.TryGetAsset<AsepriteAsset>(component.AnimationGuid) is AsepriteAsset ase)
                {
                    if (ImGui.BeginCombo($"##AnimationID", component.AnimationId))
                    {
                        foreach (var value in ase.Animations.Keys)
                        {
                            if (string.IsNullOrWhiteSpace(value))
                            {
                                continue;
                            }

                            if (ImGui.MenuItem(value))
                            {
                                var nextAnimations = component.NextAnimations.Insert(0, value);
                                target.GetType().GetField("NextAnimations")!.SetValue(target, nextAnimations);
                                fileChanged = true;
                            }
                        }
                        ImGui.EndCombo();
                    }
                }

                ImGui.EndTable();
            }

            return fileChanged;
        }
    }
}
