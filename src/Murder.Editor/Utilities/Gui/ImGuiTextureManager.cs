using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Vector4  = System.Numerics.Vector4;
using Murder.Data;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace Murder.ImGuiExtended
{
    /// <summary>
    /// Manages texture being displayed within the ImGui.
    /// </summary>
    public class ImGuiTextureManager : IDisposable
    {
        private readonly CacheDictionary<string, IntPtr> _images = new(128);

        private IntPtr GetNextTextureId() => Game.Instance.ImGuiRenderer.GetNextIntPtr();

        public bool Image(string id, float maxSize, AtlasId atlasId, float scale = 1)
        {
            if (Game.Data.TryFetchAtlas(atlasId) is not TextureAtlas atlas)
            {
                return false;
            }

            return Image(id, maxSize, atlas, scale);
        }

        public bool Image(string id, float maxSize, TextureAtlas? atlas, float scale = 1)
        {
            if (_images.TryGetValue(id, out IntPtr textureId) && 
                Game.Instance.ImGuiRenderer.GetLoadedTexture(textureId) is Texture2D texture) 
            {
                DrawImage(textureId, texture, maxSize, scale);
                return false;
            }

            if (atlas is null)
            {
                textureId = GetNextTextureId();
                Texture2D t = Game.Data.FetchTexture(id);

                Game.Instance.ImGuiRenderer.BindTexture(textureId, t, false);
                _images[id] = textureId;

                DrawImage(textureId, t, maxSize, scale);
            }
            else if (atlas.TryCreateTexture(id, out Texture2D t))
            {
                textureId = GetNextTextureId();
                t.Name = $"preview{textureId}";

                Game.Instance.ImGuiRenderer.BindTexture(textureId, t, false);
                _images[id] = textureId;

                DrawImage(textureId, t, maxSize, scale);
            } 
            else
            {
                // TODO: Add missing image resource.
                // GameLogger.Warning($"Missing image '{id}' on atlas '{atlas.Name}'")
                // ImGui.Image(MissingImage, Vector2.One * maxSize);

                return false;
            }

            return true;
        }

        private void DrawImage(IntPtr id, Texture2D texture, float maxSize, float scale)
        {
            var size = new Vector2(texture.Width, texture.Height) * Game.Instance.DPIScale / 100f;

            var factor = (texture.Width > maxSize) ? maxSize / texture.Width : 1;
            factor = (texture.Height * factor > maxSize) ? maxSize / texture.Height : factor;
            ImGui.Image(id, size * factor * scale);
        }

        public void Dispose()
        {
            // Dispose textures? Already handled in ImGuiRenderer?
        }
    }
}