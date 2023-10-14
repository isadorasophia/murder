using ImGuiNET;

namespace Murder.Editor.ImGuiExtended
{
    public class TableTwoColumns : IDisposable
    {
        private readonly bool _opened;

        public TableTwoColumns(string label, ImGuiTableFlags flags = ImGuiTableFlags.SizingFixedSame | ImGuiTableFlags.BordersOuter)
        {
            if (ImGui.BeginTable(label, 2, flags))
            {
                ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthFixed, -1, 0);
                ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthStretch, -1, 1);

                _opened = true;
            }
        }

        public void Dispose()
        {
            if (_opened)
            {
                ImGui.EndTable();
            }
        }
    }
}