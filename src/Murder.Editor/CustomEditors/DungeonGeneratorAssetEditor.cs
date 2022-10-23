using InstallWizard.Core;
using InstallWizard.Core.Graphics;
using InstallWizard.Data;
using InstallWizard.DebugUtilities;
using InstallWizard.Dungeons;
using InstallWizard.Util;
using Editor.CustomComponents;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Color = Microsoft.Xna.Framework.Color;

namespace Editor.CustomEditors
{
    [CustomEditorOf(typeof(DungeonGeneratorAsset))]
    internal class DungeonGeneratorAssetEditor : AssetEditor
    {
        private DungeonGeneratorAsset? _dungeon;

        private Texture2D? _preview;
        private IntPtr? _previewImagePointer;

        internal override void OpenEditor(ImGuiRenderer imGuiRenderer, object target)
        {
            DungeonGeneratorAsset dungeonGenerator = (DungeonGeneratorAsset)target;

            if (_dungeon != target)
            {
                // TODO: This will probably end horribly if two dungeons are opened at the same time.
                _preview = null;
                _previewImagePointer = null;

                _dungeon = dungeonGenerator;
                _asset = dungeonGenerator;
            }
        }

        internal override ValueTask DrawEditor()
        {
            GameLogger.Verify(_dungeon is not null);

            if (CustomComponent.ShowEditorOf(_dungeon))
            {
                // Do not initialize the dungeon again simply because we change a field,
                // wait for the user input on that.
                _dungeon.FileChanged = true;
            }

            if (ImGui.Button(" * RANDOM DUNGEON * "))
            {
                _dungeon.Seed = Random.Shared.Next();
                _dungeon.Generate();
                _preview = null;
            }
            ImGui.SameLine();

            if (ImGui.Button("Generate from seed"))
            {
                _dungeon.Generate();
                _preview = null;
            }

            ImGui.SameLine();

            if (_dungeon.IsGenerationDone)
            {
                ImGuiExtended.DisabledButton("Generation Done");
            }
            else if (ImGui.Button("Step Generation"))
            {
                _dungeon.Step();
                _preview = null;
            }

            ImGui.SameLine();

            if (ImGui.Button("Start Over"))
            {
                _dungeon.Reset();
                _preview = null;
            }

            DrawMap();
            
            return default;
        }

        public void DrawMap()
        {
            GameLogger.Verify(_dungeon is not null);

            var size = ImGui.GetContentRegionAvail() - new System.Numerics.Vector2(0, 5) * Architect.Instance.DPIScale/100f;

            if (size.X <= 0 || size.Y <= 0)
            {
                // Empty.
                return;
            }

            ImGui.InvisibleButton("map_canvas", size);

            var bottomRight = ImGui.GetItemRectMax();
            var topLeft = ImGui.GetItemRectMin();
            var canvasSize = bottomRight - topLeft;

            ImDrawListPtr drawList = ImGui.GetWindowDrawList();
            
            if (_dungeon.Height == 0 || _dungeon.Width == 0)
            {
                drawList.AddRect(topLeft, bottomRight, ImGuiExtended.MakeColor32(255,155,0,255), 6f);
                return;
            }

            if (_preview is null || size != _preview.Bounds.Size.ToSysVector2())
            {
                _preview = GeneratePreviewImage(size);
                if (_previewImagePointer is null)
                {
                    _previewImagePointer = Architect.Instance.ImGuiRenderer.BindTexture(_preview);
                }
                else
                {
                    Architect.Instance.ImGuiRenderer.BindTexture(_previewImagePointer.Value, _preview, true);
                }
            }

            drawList.AddRectFilled(topLeft, bottomRight, ImGuiExtended.MakeColor32(55,55,100,255), 6f);
            drawList.AddImage(_previewImagePointer!.Value, topLeft, bottomRight);
        }

        private Texture2D GeneratePreviewImage(Vector2 size)
        {
            GameLogger.Verify(_dungeon is not null);

            DungeonGrid dungeonGrid = _dungeon.Grid;

            Texture2D texture = new(Architect.GraphicsDevice, (int)size.X, (int)size.Y);
            Color[] data = new Color[(int)size.X * (int)size.Y];

            var smallest = (int)Math.Min(size.X/dungeonGrid.Width, size.Y/dungeonGrid.Height);
            var gridSize = new Point(smallest, smallest);
            
            for (int x = 0; x < dungeonGrid.Width; x++)
            {
                for (int y = 0; y < dungeonGrid.Height; y++)
                {
                    var grid = dungeonGrid.Grid[x + y * dungeonGrid.Width];
                    
                    var gridPos = new Vector2(x * gridSize.X, y * gridSize.Y);
                    
                    var color = Tile.GetPreviewColor(grid);

                    DrawRect(
                        x * gridSize.X, y * gridSize.Y,
                        gridSize.X, gridSize.Y,
                        ref data, (int)size.X, color);
                }
            }

            if (_dungeon.IsGenerationDone)
            {
                foreach (DungeonObject obj in _dungeon.DungeonObjects)
                {
                    int x = obj.Position.X;
                    int y = obj.Position.Y;

                    DrawRect(
                        x * gridSize.X, y * gridSize.Y,
                        gridSize.X, gridSize.Y,
                        ref data, (int)size.X, obj.GetPreviewColor());
                }
            }

            texture.SetData(data);
            
            return texture;
        }

        private void DrawRect(int x, int y, int w, int h, ref Color[] data, int imageWidth, Color color)
        {
            for (int iy = y; iy < y + h; iy++)
            {
                for (int ix = x; ix < x + w; ix++)
                {
                    data[ix + (iy) * imageWidth] = color;
                }
            }
        }
    }
}