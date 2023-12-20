using ImGuiNET;
using Murder.Core;
using Murder.Editor.ImGuiExtended;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Editor
{
    public partial class EditorScene : Scene
    {
        private const float ExplorerIconsColumnWidth = 48f;
        private const float ExplorerDefaultWidth = 300f;
        private static readonly Vector4 HoverColor = new(1.0f, 0.47f, 0.77f, 1.0f);
        private static readonly Vector4 SelectedColor = new(0.74f, 0.57f, 0.97f, 1.0f);
        private static readonly Vector4 NormalColor = new(0.27f, 0.28f, 0.35f, 1.0f);
        
        private ExplorerWindow? _selectedExplorerWindow;
        private readonly ImmutableArray<ExplorerWindow> _explorerPages;

        public EditorScene()
        {
            ImmutableArray<ExplorerWindow>.Builder builder = ImmutableArray.CreateBuilder<ExplorerWindow>();
            builder.Add(new("assets", "Assets", "\uf70e", DrawAssetsWindow));
            builder.Add(new("explorer", "Atlas", "\uf0ac", DrawAtlasWindow));
            builder.Add(new("save-data", "Save Data", "\uf0c7", DrawSavesWindow));

            _explorerPages = builder.ToImmutable();
            _selectedExplorerWindow = _explorerPages.First();
        }

        private void DrawExplorerIcons()
        {
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.PushFont(Fonts.LargeIcons);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, HoverColor);
            foreach (ExplorerWindow tab in _explorerPages)
            {
                bool isSelected = _selectedExplorerWindow?.Id == tab.Id;
                Vector4 baseColor = isSelected ? SelectedColor : NormalColor;

                ImGui.PushStyleColor(ImGuiCol.Button, baseColor);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, baseColor);
                
                if (ImGui.Button($"{tab.Icon}", new Vector2(-6, 36)))
                {
                    _selectedExplorerWindow = isSelected ? null : tab;
                }
                ImGui.PopStyleColor(2);
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();
        }

        private void DrawExplorerIfNeeded()
        {
            ImGui.TableNextColumn();
            if (_selectedExplorerWindow is not null)
            {
                Vector2 size = ImGui.GetContentRegionAvail() - new Vector2(0, ImGui.GetStyle().FramePadding.Y);
                if (ImGui.BeginChild("explorer_child", size))
                {
                    ImGui.Spacing();
                    ImGui.Spacing();
                    ImGui.PushFont(Fonts.TitleFont);
                    ImGui.Text(_selectedExplorerWindow.Title);
                    ImGui.PopFont();

                    ImGui.Spacing();
                    ImGui.Spacing();
                    ImGui.Separator();
                    ImGui.Spacing();

                    _selectedExplorerWindow.RenderCallback();
                }

                ImGui.EndChild();
            }
        }
        
        private sealed record ExplorerWindow(
            string Id,
            string Title,
            string Icon,
            Action RenderCallback
        );
    }
}