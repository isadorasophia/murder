using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Core;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<PortraitInfo>))]
    internal class ImmutableArrayPortraitField : ImmutableArrayField<PortraitInfo>
    {
        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out PortraitInfo element)
        {
            if (ImGui.Button("New Portrait"))
            {
                element = new();
                return true;
            }

            element = default;
            return false;
        }

        protected override bool DrawElement(ref PortraitInfo element, EditorMember member, int index)
        {
            bool modified = false;

            string id = $"speaker_{index}";
            using TableMultipleColumns table = new(id, flags: ImGuiTableFlags.SizingFixedFit, 0, 0);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            if (Game.Data.TryGetAsset<SpriteAsset>(element.Portrait.Sprite) is SpriteAsset ase)
            {
                EditorAssetHelpers.DrawPreview(ase, maxSize: 256, element.Portrait.AnimationId);
                ImGui.TableNextColumn();
            }

            ImGui.PushItemWidth(300);

            string name = element.Name;
            if (DrawValue(ref element, nameof(PortraitInfo.Name)))
            {
                modified = true;
            }

            Portrait portrait = element.Portrait;
            if (DrawValue(ref portrait, nameof(Portrait.Sprite)))
            {
                element = element with { Portrait = element.Portrait.WithSprite(portrait.Sprite) };
                modified = true;
            }

            // Draw combo box for the animation id
            string animation = element.Portrait.AnimationId;
            if (EditorAssetHelpers.DrawComboBoxFor(element.Portrait.Sprite, ref animation))
            {
                element = element with { Portrait = element.Portrait.WithAnimationId(animation) };
                modified = true;
            }
            ImGui.PopItemWidth();

            if (portrait.Sprite != Guid.Empty)
            {
                int flags = (int)element.Properties;
                if (ImGuiHelpers.DrawEnumFieldAsFlags(id, typeof(PortraitProperties), ref flags))
                {
                    element = element with { Properties = (PortraitProperties)flags };
                    modified = true;
                }
            }

            return modified;
        }
    }
}