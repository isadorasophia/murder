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
    [CustomFieldOf(typeof(ImmutableDictionary<string, PortraitInfo>))]
    internal class DictionaryStringPortrait : DictionaryField<string, PortraitInfo>
    {
        private string _new = string.Empty;

        protected override bool Add(IList<string> candidates, [NotNullWhen(true)] out (string Key, PortraitInfo Value)? element)
        {
            if (ImGui.Button("New Portrait"))
            {
                _new = "Portrait-Name";
                ImGui.OpenPopup("Add Item##dictionary");
            }

            if (ImGui.BeginPopup("Add Item##dictionary"))
            {
                ImGui.InputText("##AddAnim_new_name", ref _new, 128);

                if (ImGui.Button("Create"))
                {
                    ImGui.CloseCurrentPopup();
                    element = (_new, new PortraitInfo());

                    ImGui.EndPopup();
                    return true;
                }

                ImGui.EndPopup();
            }

            element = (default!, default!);
            return false;
        }

        protected override List<string> GetCandidateKeys(EditorMember member, IDictionary<string, PortraitInfo> fieldValue) =>
            new() { default! };

        protected override bool CanModifyKeys() => true;

        public override bool DrawElementValue(EditorMember member, PortraitInfo value, out PortraitInfo modifiedValue)
        {
            bool modified = false;

            string id = $"action_{value.Portrait.Sprite}_{value.Portrait.AnimationId}";
            using TableMultipleColumns table = new(id, flags: ImGuiTableFlags.SizingFixedFit, 0, 0);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            if (Game.Data.TryGetAsset<SpriteAsset>(value.Portrait.Sprite) is SpriteAsset ase)
            {
                EditorAssetHelpers.DrawPreview(ase, maxSize: 256, value.Portrait.AnimationId);
                ImGui.TableNextColumn();
            }

            ImGui.PushItemWidth(300);

            modifiedValue = value;

            Portrait portrait = modifiedValue.Portrait;
            if (DrawValue(ref portrait, nameof(Portrait.Sprite)))
            {
                modifiedValue = value with { Portrait = value.Portrait.WithSprite(portrait.Sprite) };
                modified = true;
            }

            // Draw combo box for the animation id
            string animation = value.Portrait.AnimationId;
            if (EditorAssetHelpers.DrawComboBoxFor(value.Portrait.Sprite, ref animation))
            {
                modifiedValue = value with { Portrait = value.Portrait.WithAnimationId(animation) };
                modified = true;
            }
            ImGui.PopItemWidth();

            if (portrait.Sprite != Guid.Empty)
            {
                int flags = (int)value.Properties;
                if (ImGuiHelpers.DrawEnumFieldAsFlags(id, typeof(PortraitProperties), ref flags))
                {
                    modifiedValue = value with { Properties = (PortraitProperties)flags };
                    modified = true;
                }
            }

            return modified;
        }
    }
}