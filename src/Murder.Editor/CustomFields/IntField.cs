using ImGuiNET;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Security.Cryptography;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(int))]
    internal class IntField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            int number = Convert.ToInt32(fieldValue);

            if (AttributeExtensions.IsDefined(member, typeof(CollisionLayerAttribute)))
            {
                return DrawCollisionLayerSelector(member, ref modified, ref number);
            }
            
            if (AttributeExtensions.IsDefined(member, typeof(SpriteBatchReferenceAttribute)))
            {
                return DrawSpriteBatchSelector(member, ref modified, ref number);
            }

            modified = ImGui.InputInt("", ref number, 1);

            return (modified, number);
        }

        private static (bool modified, object? result) DrawSpriteBatchSelector(EditorMember member, ref bool modified, ref int number)
        {
            ImmutableArray<(string Name, int Id)> list = AssetsFilter.SpriteBatches;
            string[] prettyNames = AssetsFilter.SpriteBatchesNames;

            int index;
            for (index = list.Length - 1; index >= 0; index--)
            {
                if (list[index].Id == number)
                    break;
            }

            if (ImGui.Combo($"##{member.Name}-sb", ref index, prettyNames, prettyNames.Length))
            {
                modified = true;
            }

            return (modified, list[index].Id);
        }
        private static (bool modified, object? result) DrawCollisionLayerSelector(EditorMember member, ref bool modified, ref int number)
        {
            using TableMultipleColumns table = new($"##{member.Name}-{member.Member.Name}-col-table",
                                flags: ImGuiTableFlags.SizingFixedFit,
                                -1, -1, -1);
            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            ImmutableArray<(string Name, int Id)> list = AssetsFilter.CollisionLayers;
            string[] prettyNames = AssetsFilter.CollisionLayersNames;

            for (int i = 0; i < list.Length; i++)
            {
                bool isChecked = (list[i].Id & number) != 0;

                if (ImGui.Checkbox($"##{member.Name}-{i}-col-layer", ref isChecked))
                {
                    if (isChecked)
                    {
                        number |= list[i].Id;
                    }
                    else
                    {
                        number &= ~list[i].Id;
                    }

                    modified = true;
                }

                ImGui.SameLine();
                ImGui.Text(prettyNames[i]);

                ImGui.TableNextColumn();

                if ((i + 1) % 3 == 0)
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                }
            }

            return (modified, number);
        }
    }
}