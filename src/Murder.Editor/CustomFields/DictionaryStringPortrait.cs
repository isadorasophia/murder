using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Core;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableDictionary<string, Portrait>))]
    internal class DictionaryStringPortrait : DictionaryField<string, Portrait>
    {
        private string _new = string.Empty;

        protected override bool Add(IList<string> candidates, [NotNullWhen(true)] out (string Key, Portrait Value)? element)
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
                    element = (_new, new Portrait());

                    ImGui.EndPopup();
                    return true;
                }

                ImGui.EndPopup();
            }

            element = (default!, default!);
            return false;
        }

        protected override List<string> GetCandidateKeys(EditorMember member, IDictionary<string, Portrait> fieldValue) =>
            new() { default! };

        protected override bool CanModifyKeys() => true;

        public override bool DrawElementValue(EditorMember member, Portrait value, out Portrait modifiedValue)
        {
            bool modified = false;

            using TableMultipleColumns table = new($"action_{value.Aseprite}_{value.AnimationId}", flags: ImGuiTableFlags.SizingFixedFit, -1, -1);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            if (Game.Data.TryGetAsset<AsepriteAsset>(value.Aseprite) is AsepriteAsset ase)
            {
                EditorAssetHelpers.DrawPreview(ase, maxSize: 256, value.AnimationId);
                ImGui.TableNextColumn();
            }

            ImGui.PushItemWidth(300);

            modifiedValue = value;
            if (DrawValue(ref modifiedValue, nameof(Portrait.Aseprite)))
            {
                modified = true;
            }

            // Draw combo box for the animation id
            string animation = value.AnimationId;
            if (EditorAssetHelpers.DrawComboBoxFor(value.Aseprite, ref animation))
            {
                modifiedValue = value.WithAnimationId(animation);
                modified = true;
            }

            ImGui.PopItemWidth();

            return modified;
        }
    }
}
