using ImGuiNET;

namespace Murder.Editor.ImGuiExtended
{
    internal class DragDrop
    {
        private static int _draggedId = 0;

        public static void DragDropSource(string payload_id, string payloadName, int id)
        {
            if (ImGui.BeginDragDropSource())
            {
                ImGui.Text($"Moving {payloadName}...");

                ImGui.SetDragDropPayload(payload_id, IntPtr.Zero, 0);
                _draggedId = id;

                ImGui.EndDragDropSource();
            }
        }

        public static bool DragDropTarget(string payload_id, out int draggedId)
        {
            if (ImGui.BeginDragDropTarget())
            {
                ImGuiPayloadPtr payload = ImGui.AcceptDragDropPayload(payload_id);

                bool hasDropped;
                unsafe
                {
                    hasDropped = payload.NativePtr != null;
                }

                if (hasDropped)
                {
                    draggedId = _draggedId;
                    return true;
                }

                ImGui.EndDragDropTarget();
            }
            
            draggedId = 0;
            return false;
        }
    }
}
