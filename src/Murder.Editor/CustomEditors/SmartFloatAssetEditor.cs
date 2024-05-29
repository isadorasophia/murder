using ImGuiNET;
using Murder.Assets;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(SmartFloatAsset))]
    internal class SmartFloatAssetEditor : CustomEditor
    {
        public override object Target => _floatAsset!;
        private SmartFloatAsset? _floatAsset;

        public override void OpenEditor(ImGuiRenderer imGuiRenderer, RenderContext renderContext, object target, bool overwrite)
        {
            _floatAsset = (SmartFloatAsset)target;

        }

        public override void DrawEditor()
        {
            if (_floatAsset is not SmartFloatAsset target)
                return;

            if (ImGui.BeginTable("values_table", 4, ImGuiTableFlags.Resizable | ImGuiTableFlags.Borders))
            {
                ImGui.TableSetupColumn("delete", ImGuiTableColumnFlags.WidthFixed, 20, 0);
                ImGui.TableSetupColumn("id", ImGuiTableColumnFlags.WidthFixed, 20, 0);
                ImGui.TableSetupColumn("title", ImGuiTableColumnFlags.WidthStretch, -1, 1);
                ImGui.TableSetupColumn("value", ImGuiTableColumnFlags.WidthFixed, 200, 2);

                for (int i = 0; i < target.Values.Length; i++)
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    if (ImGuiHelpers.DeleteButton($"delete{i}"))
                    {
                        target.Titles = target.Titles.RemoveAt(i);
                        target.Values = target.Values.RemoveAt(i);
                        target.FileChanged = true;
                        break;
                    }

                    ImGui.TableNextColumn();
                    ImGui.Text(i.ToString());

                    ImGui.TableNextColumn();
                    string title = target.Titles[i];
                    ImGui.SetNextItemWidth(-1);
                    if (ImGui.InputText($"###{i}_title", ref title, 200))
                    {
                        target.Titles = target.Titles.SetItem(i, title);
                        target.FileChanged = true;
                    }

                    ImGui.TableNextColumn();
                    float value = target.Values[i];
                    ImGui.SetNextItemWidth(-1);
                    if (ImGui.InputFloat($"###{i}_float", ref value))
                    {
                        target.Values = target.Values.SetItem(i, value);
                        target.FileChanged = true;
                    }
                }

                ImGui.EndTable();
            }

            if (ImGui.Button("Add Entry"))
            {
                target.Titles = target.Titles.Add("New Entry");
                target.Values = target.Values.Add(0);
                target.FileChanged = true;
            }

        }

    }
}