using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Editor.CustomEditors
{
    [CustomComponentOf(typeof(TileGridComponent))]
    internal class TileGridComponentEditor : CustomComponent
    {
        protected override bool DrawAllMembersWithTable(ref object target)
        {
            bool fileChanged = false;
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Button, Game.Profile.Theme.Faded);
            if (ImGui.Button("Open Tileset tab"))
            {
                if (Architect.Instance.ActiveScene is EditorScene editor)
                {
                    if (editor.EditorShown is WorldAssetEditor worldAssetEditor)
                    {
                        worldAssetEditor.SwitchToTilesetsTab();
                    }
                }
            }
            ImGui.PopStyleColor();

            var tiles = (TileGridComponent)target;
            DrawInfo("X: ", tiles.Rectangle.X);
            DrawInfo("Y: ", tiles.Rectangle.Y);
            DrawInfo("Width: ", tiles.Rectangle.Width);
            DrawInfo("Height: ", tiles.Rectangle.Height);

            return fileChanged;
        }

        private static void DrawInfo(string name, float value)
        {
            ImGui.Text(name);
            ImGui.SameLine();
            ImGui.TextColored(Game.Profile.Theme.Faded, value.ToString());
        }
    }
}
