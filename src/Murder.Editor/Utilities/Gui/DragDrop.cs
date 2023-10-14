using ImGuiNET;

namespace Murder.Editor.ImGuiExtended
{
    internal class DragDrop<T> where T : new()
    {
        private static T _draggedId = new();

        public static void DragDropSource(string payload_id, string displayName, T id)
        {
            if (ImGui.BeginDragDropSource())
            {
                ImGui.Text($"Moving {displayName}...");

                ImGui.SetDragDropPayload(payload_id, IntPtr.Zero, 0);
                _draggedId = id;

                ImGui.EndDragDropSource();
            }
        }

        public static bool DragDropTarget(string payload_id, out T draggedId)
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

            draggedId = new();
            return false;
        }
    }
}