using ImGuiNET;
using Murder.Assets;
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

        private readonly Vector4 _hoverColor;
        private readonly Vector4 _selectedColor;
        private readonly Vector4 _normalColor;
        
        private ExplorerWindow? _selectedExplorerWindow;
        private readonly ImmutableArray<ExplorerWindow> _explorerPages;
        
        private ImmutableArray<ExplorerWindow> CreateExplorerPages() => 
        [
            new("assets", "Assets", "\uf520", DrawAssetsWindow),
            new("explorer", "Atlas", "\uf03e", DrawAtlasWindow),
            new("save-data", "Save Data", "\uf0c7", DrawSavesWindow),
        ];

        private void DrawExplorerIcons()
        {
            ImGui.Spacing();
            ImGui.Spacing();

            foreach (ExplorerWindow tab in _explorerPages)
            {
                bool isSelected = _selectedExplorerWindow?.Id == tab.Id;
                Vector4 baseColor = isSelected ? Game.Profile.Theme.Accent : Game.Profile.Theme.BgFaded;

                ImGui.PushStyleColor(ImGuiCol.Button, baseColor);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, baseColor);

                ImGui.PushFont(Fonts.LargeIcons);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Game.Profile.Theme.HighAccent);

                if (ImGui.Button($"{tab.Icon}", new Vector2(-4, 36)))
                {
                    _selectedExplorerWindow = isSelected ? null : tab;

                    lock (_foldersLock)
                    {
                        _folders = null;
                    }
                }

                ImGui.PopStyleColor();
                ImGui.PopFont();

                ImGui.PopStyleColor(2);

                ImGuiHelpers.HelpTooltip(tab.Title);
            }
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