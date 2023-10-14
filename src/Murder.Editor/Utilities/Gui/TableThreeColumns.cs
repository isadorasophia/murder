using ImGuiNET;

namespace Murder.Editor.ImGuiExtended
{
    public class TableMultipleColumns : IDisposable
    {
        public bool Opened => _opened;
        private readonly bool _opened;

        /// <summary>
        /// Create a new table with specified width. The measurements will be scaled to the dpi.
        /// </summary>
        public TableMultipleColumns(string label, ImGuiTableFlags flags = ImGuiTableFlags.BordersOuter, params int[] widths)
        {
            bool dynamicWidth = widths.Any(d => d < 0);
            if (ImGui.BeginTable(label, widths.Length, flags,
                outer_size: dynamicWidth ? System.Numerics.Vector2.Zero : new(widths.Sum(), 0)))
            {
                for (int i = 0; i < widths.Length; i++)
                {
                    var w = widths[i];
                    ImGui.TableSetupColumn($"c_{w}", w == -1 ? ImGuiTableColumnFlags.WidthStretch : ImGuiTableColumnFlags.WidthFixed, w, 0);
                }

                _opened = true;
            }
        }

        /// <summary>
        /// Create a new table with specified width and column flag. The measurements will be scaled to the dpi.
        /// </summary>
        public TableMultipleColumns(string label, ImGuiTableFlags flags = ImGuiTableFlags.BordersOuter, params (ImGuiTableColumnFlags Flags, int Width)[] widths)
        {
            bool dynamicWidth = widths.Any(d => d.Width < 0);

            if (ImGui.BeginTable(label, widths.Length, flags,
                outer_size: dynamicWidth ? System.Numerics.Vector2.Zero : new(widths.Select(t => t.Width).Sum(), 0)))
            {
                foreach ((ImGuiTableColumnFlags columnFlags, int w) in widths)
                {
                    ImGui.TableSetupColumn($"c_{w}", columnFlags, w, 0);
                }

                _opened = true;
            }
        }

        /// <summary>
        /// Create a new table with specified width. The measurements will be scaled to the dpi.
        /// </summary>
        public TableMultipleColumns(string label, ImGuiTableFlags flags = ImGuiTableFlags.BordersOuter, params (int Width, ImGuiTableColumnFlags Flags)[] columns)
        {
            bool dynamicWidth = columns.Any(d => d.Width < 0);

            if (ImGui.BeginTable(label, columns.Length, flags,
                outer_size: dynamicWidth ? System.Numerics.Vector2.Zero : new(columns.Sum(d => d.Width), 0)))
            {
                foreach (var (width, columnFlags) in columns)
                {
                    ImGui.TableSetupColumn($"c_{width}", columnFlags, width, 0);
                }

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