using ImGuiNET;
using System.Text.RegularExpressions;
using Murder.Assets.Graphics;
using Murder.Editor.Attributes;
using Murder.Assets;
using Murder.Diagnostics;
using Murder.ImGuiExtended;
using Murder.Data;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(SpriteAsset))]
    internal class SpriteAssetEditor : CustomEditor
    {
        private SpriteAsset _sprite = null!;
        private readonly List<string> _atlasEntries = new();

        public override GameAsset Target => _sprite!;

        public override void OpenEditor(ImGuiRenderer imGuiRenderer, object target)
        {
            _sprite = (SpriteAsset)target;
        }

        public override ValueTask DrawEditor()
        {
            var theme = Game.Profile.Theme;

            GameLogger.Verify(_sprite is not null);

            var re = new Regex(@"([a-zA-Z\\/_]+)(\d$)$");

            if (ImGui.Button("RefreshAtlas"))
            {
                Game.Data.RefreshAtlas();
            }
            if (ImGui.Button("DisposeAtlases"))
            {
                Game.Data.DisposeAtlases();
            }

            if (ImGui.BeginCombo($"##{_sprite.Guid}_editor_combo", $"{_sprite.Atlas}> {_sprite.FirstFrame}"))
            {
                _atlasEntries.Clear();

                foreach (var frame in Architect.EditorData.HiResImages)
                {
                    var fullPath = frame;
                    int splitPoint = fullPath.IndexOf('\\');

                    var spritePath = fullPath.Substring(splitPoint+1);
                    var atlas = Game.Data.GetAtlasEnum(fullPath.Substring(0, splitPoint));

                    var match = re.Match(spritePath);

                    if (!string.IsNullOrWhiteSpace(match.Groups[2].Value)) // This is an animation
                    {
                        
                        if (_atlasEntries.Contains(spritePath))
                            continue;

                        _atlasEntries.Add(spritePath);
                        if (ImGui.MenuItem($"{atlas}> {spritePath} *"))
                        {
                            _sprite.FirstFrame = match.Groups[1].Value + "0".ToString().PadLeft(match.Groups[2].Value.Length, '0');
                            _sprite.Atlas = atlas;
                            Target.FileChanged = true;
                            //_animation.RecalculateAnimation();
                        }
                    }
                    else
                    {
                        if (ImGui.MenuItem($"{atlas}> {spritePath}"))
                        {
                            _sprite.FirstFrame = spritePath;
                            _sprite.Atlas = atlas;
                            Target.FileChanged = true;
                            //_animation.RecalculateAnimation();
                        }
                    }
                }
                ImGui.EndCombo();


            }
            ImGui.Text($"Sprite in '{_sprite.Atlas}' atlas");

            var result = re.Match(_sprite.FirstFrame);
            var prefix = result.Groups[1].Value;
            var padding = result.Groups[2].Value.Length;
            if (_sprite.Atlas != AtlasId.None)
            {
                var textureAtlas = Game.Data.FetchAtlas(_sprite.Atlas);

                int i = 0;
                if (textureAtlas != null)
                {
                    while (textureAtlas.Exist(prefix + i.ToString().PadLeft(padding, '0')))
                    {
                        i++;
                    }
                }
                else
                {
                    while (Game.Data.TextureExists(prefix + i.ToString().PadLeft(padding, '0')))
                    {
                        i++;
                    }
                }

                if (i > 0)
                {
                    ImGui.PushID("Frames per second");
                    ImGui.Text("Frames per second: ");
                    ImGui.SameLine();
                    if (ImGui.InputFloat("", ref _sprite.FramesPerSecond))
                    {
                        _sprite.RecalculateAnimation();
                    }
                    ImGui.PopID();

                    ImGui.TextColored(theme.Faded, $"(from {prefix + "0".PadLeft(padding, '0')} to {prefix + (i - 1).ToString().PadLeft(padding, '0')})");

                    ImGui.PushID("Frame List");
                    ImGui.Text("Frames: ");
                    ImGui.SameLine();
                    if (ImGui.InputText("", ref _sprite.Frames, 128))
                    {
                        _sprite.RecalculateAnimation();
                    }
                    ImGui.TextColored(theme.Faded, string.Join(',', _sprite.Animation.Frames));
                    ImGui.PopID();

                    var image = _sprite.Animation.Evaluate(0, Game.Now).animationFrame;
                    if (textureAtlas is not null)
                    {
                        if (textureAtlas.HasId(image))
                            ImGuiHelpers.Image(image, 256, textureAtlas, 1);
                    }
                    else
                    {
                        if (Game.Data.TextureExists(image))
                            ImGuiHelpers.Image(image, 256, null, 1);
                    }
                }
                else
                {
                    ImGui.TextColored(theme.Red, "Animation frames not found");

                    if (textureAtlas is not null)
                    {
                        if (textureAtlas.HasId(_sprite.FirstFrame))
                            ImGuiHelpers.Image(_sprite.FirstFrame, 256, textureAtlas, 1);
                    }
                    else
                    {
                        if (Game.Data.TextureExists(_sprite.FirstFrame))
                            ImGuiHelpers.Image(_sprite.FirstFrame, 256, null, 1);
                    }
                }
            }

            return default;
        }
    }
}
