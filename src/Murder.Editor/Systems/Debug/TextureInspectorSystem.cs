using Bang.Contexts;
using Bang.Systems;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Services;
using System.Numerics;

namespace Murder.Editor.Systems
{
    [DoNotPause]
    [OnlyShowOnDebugView]
    [Filter(ContextAccessorFilter.None)]
    internal class TextureInspectorSystem : IGuiSystem
    {
        private bool _show = false;
        private int _selected = -1;
        private int _zoom = 1;
        private Texture2D? _selectedTexture = null;
        private static IntPtr? _textureInspectorPreview = null;

        public void DrawGui(RenderContext render, Context context)
        {
            ImGui.BeginMainMenuBar();

            if (ImGui.BeginMenu("Show"))
            {
                ImGui.MenuItem("Texture Inspector", "", ref _show);
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();

            if (!_show)
                return;


            int maxWidth = 1200;

            // Graphics
            ImGui.SetNextWindowBgAlpha(0.9f);
            ImGui.SetNextWindowSizeConstraints(
                size_min: new Vector2(500, 350),
                size_max: new Vector2(EditorSystem.WINDOW_MAX_WIDTH, EditorSystem.WINDOW_MAX_HEIGHT));

            int padding = 25;
            ImGui.SetWindowPos(new(x: render.ScreenSize.X - maxWidth, y: padding), ImGuiCond.Appearing);
            
            if (!ImGui.Begin("Texture Inspector", ref _show))
            {
                ImGui.End();
                // Window is closed, so just go way...
                return;
            }

            {
                using TableMultipleColumns table = new("textures_inspector", ImGuiTableFlags.Resizable, 0, -1);

                float height = -1;

                ImGui.TableNextColumn();
                ImGui.SliderInt("Zoom", ref _zoom, 1, 4);

                ImGui.BeginChild("textures_inspector",
                    size: new Vector2(-1, height), ImGuiChildFlags.None, ImGuiWindowFlags.NoDocking);


                if (DebugServices.DebugPreviewImage != null && ImGui.Selectable("Preview Image", _selected == -2))
                {
                    _selected = -2;
                    _selectedTexture = DebugServices.DebugPreviewImage;

                    BindCurrentTexture();
                }

                ImGui.Separator();
                for (int i = 0; i < Game.Data.AvailableUniqueTextures.Length; i++)
                {
                    if (ImGui.Selectable(Game.Data.AvailableUniqueTextures[i], _selected == i))
                    {
                        _selected = i;
                        _selectedTexture = Game.Data.FetchTexture(Game.Data.AvailableUniqueTextures[i]);

                        BindCurrentTexture();
                    }
                }

                for (int i = 0; i < RuntimeAtlas.AllLoadedAtlas.Count; i++)
                {
                    var texture = RuntimeAtlas.AllLoadedAtlas[i].GetFullAtlas();
                    if (ImGui.Selectable(texture.Name, _selected == i))
                    {
                        _selected = i;
                        _selectedTexture = (Texture2D)texture;

                        BindCurrentTexture();
                    }

                    var brushTexture = RuntimeAtlas.AllLoadedAtlas[i].GetBrush();
                    if (ImGui.Selectable(brushTexture.Name, _selected == i))
                    {
                        _selected = i;
                        _selectedTexture = (Texture2D)brushTexture;

                        BindCurrentTexture();
                    }
                }

                ImGui.EndChild();

                ImGui.TableNextColumn();

                ImGui.BeginChild("image_preview");

                if (_textureInspectorPreview != null && _selectedTexture != null)
                {
                    ImGui.Image(_textureInspectorPreview.Value, new System.Numerics.Vector2(_selectedTexture.Width, _selectedTexture.Height) * _zoom);
                }

                ImGui.EndChild();
            }
            
            ImGui.End();

        }

        private void BindCurrentTexture()
        {
            if (_selectedTexture == null)
            {
                return;
            }

            if (_textureInspectorPreview == null)
            {
                _textureInspectorPreview = Architect.Instance.ImGuiRenderer.GetNextIntPtr();
            }

            Architect.Instance.ImGuiRenderer.BindTexture(_textureInspectorPreview.Value, _selectedTexture, false);
        }
    }
}
