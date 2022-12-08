using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Murder.Data;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Utilities;
using Murder.Diagnostics;

namespace Murder.Editor.ImGuiExtended
{
    /// <summary>
    /// Manages texture being displayed within the ImGui.
    /// </summary>
    public class ImGuiTextureManager : IDisposable
    {
        private readonly CacheDictionary<string, IntPtr> _images = new(128);

        private IntPtr GetNextTextureId() => Architect.Instance.ImGuiRenderer.GetNextIntPtr();

        public bool HasTexture(string id) => _images.ContainsKey(id);
        
        /// <summary>
        /// Cache a <paramref name="texture"/> with <paramref name="id"/> identifier.
        /// </summary>
        public IntPtr CacheTexture(string id, Texture2D texture)
        {
            IntPtr textureId = GetNextTextureId();

            Architect.Instance.ImGuiRenderer.BindTexture(textureId, texture, false);
            _images[id] = textureId;

            return textureId;
        }

        /// <summary>
        /// Fetch an existing loaded texture.
        /// </summary>
        /// <param name="id">Identifier used to the texture.</param>
        public IntPtr? FetchTexture(string id)
        {
            bool fetched = _images.TryGetValue(id, out IntPtr result);

            return fetched ? result : null;
        }
        
        /// <summary>
        /// Creates a texture based on <paramref name="atlas"/> at <paramref name="atlasFrameId"/>.
        /// Caches the texture according to <paramref name="textureName"/>.
        /// </summary>
        public nint? CreateTexture(TextureAtlas atlas, string atlasFrameId, string textureName)
        {
            if (!atlas.TryCreateTexture(atlasFrameId, out Texture2D t))
            {
                return null;
            }
            
            t.Name = textureName;
            return CacheTexture(textureName, t);
        }

        /// <summary>
        /// Get the pointer for the missing image.
        /// </summary>
        public nint? MissingImage()
        {
            string name = "missingImage";
            string id = $"preview_{name}";

            if (_images.TryGetValue(id, out IntPtr textureId))
            {
                // There we go! Return it right away.
                return textureId;
            }

            if (Game.Data.TryFetchAtlas(AtlasId.Editor) is not TextureAtlas atlas)
            {
                GameLogger.Warning("Unable to retrieve missing image. It's our fallback!");
                return null;
            }

            if (!atlas.TryCreateTexture(name, out Texture2D t))
            {
                GameLogger.Warning("Unable to retrieve missing image. It's our fallback!");
                return null;
            }

            t.Name = name;
            return CacheTexture(id, t);
        }

        public bool DrawPreviewImage(string atlasFrameId, float maxSize, TextureAtlas? atlas, float scale = 1)
        {
            string id = $"preview_{atlasFrameId}";
            
            if (_images.TryGetValue(id, out IntPtr textureId) &&
                Architect.Instance.ImGuiRenderer.GetLoadedTexture(textureId) is Texture2D texture) 
            {
                // Image is already cached, so draw it right away.
                DrawImage(textureId, texture, maxSize, scale);
                
                return true;
            }

            if (atlas is null)
            {
                // Fetch the texture directly from data in the lack of an atlas.
                Texture2D t = Game.Data.FetchTexture(atlasFrameId);

                CacheTexture(atlasFrameId, t);
                DrawImage(textureId, t, maxSize, scale);

                return true;
            }
            
            nint? atlasTextureId = CreateTexture(atlas, atlasFrameId, id);
            if (atlasTextureId is not null)
            {
                DrawImage(atlasTextureId.Value, Architect.Instance.ImGuiRenderer.GetLoadedTexture(atlasTextureId.Value)!, maxSize, scale);
                return true;
            }

            return false;
        }

        private void DrawImage(IntPtr id, Texture2D texture, float maxSize, float scale)
        {
            var size = new Vector2(texture.Width, texture.Height);

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