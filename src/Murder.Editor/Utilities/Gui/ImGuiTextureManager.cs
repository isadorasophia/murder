using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Editor.ImGuiExtended
{
    /// <summary>
    /// Manages texture being displayed within the ImGui.
    /// </summary>
    public class ImGuiTextureManager : IDisposable
    {
        private readonly CacheDictionary<string, TextureReference> _images = new(1024);

        private IntPtr GetNextTextureId() => Architect.Instance.ImGuiRenderer.GetNextIntPtr();

        public bool HasTexture(string id) => _images.ContainsKey(id);

        /// <summary>
        /// Cache a <paramref name="texture"/> with <paramref name="id"/> identifier.
        /// </summary>
        public IntPtr CacheTexture(string id, Texture2D texture)
        {
            if (_images.TryGetValue(id, out TextureReference? pointer))
            {
                return pointer.Value;
            }

            IntPtr textureId = GetNextTextureId();
            Architect.Instance.ImGuiRenderer.BindTexture(textureId, texture, true);

            _images[id] = new(textureId);

            return textureId;
        }

        /// <summary>
        /// Fetch an existing loaded texture.
        /// </summary>
        /// <param name="id">Identifier used to the texture.</param>
        public IntPtr? FetchTexture(string id)
        {
            bool fetched = _images.TryGetValue(id, out TextureReference? result);

            return result?.Value;
        }

        /// <summary>
        /// Creates a texture based on <paramref name="atlas"/> at <paramref name="atlasFrameId"/>.
        /// Caches the texture according to <paramref name="textureName"/>.
        /// </summary>
        public nint? CreateTexture(TextureAtlas atlas, string atlasFrameId, string textureName, float scale)
        {
            if (!atlas.TryCreateTexture(atlasFrameId, out Texture2D? t, scale))
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

            return GetEditorImage(name, id, 1f);
        }

        /// <summary>
        /// Get the pointer for an editor image. If no pointer if found, try to load it.
        /// </summary>
        public nint? GetEditorImage(string path, string id, float scale)
        {
            if (_images.TryGetValue(id, out TextureReference? textureId))
            {
                // There we go! Return it right away.
                return textureId.Value;
            }

            if (Game.Data.TryFetchAtlas(AtlasId.Editor) is not TextureAtlas atlas)
            {
                GameLogger.Warning($"Unable to retrieve editor image {path}");
                return null;
            }

            if (!atlas.TryCreateTexture(path, out Texture2D? texture, scale))
            {
                GameLogger.Warning($"Unable to retrieve editor image {path}");
                return null;
            }

            texture.Name = path;

            return CacheTexture(id, texture);
        }

        /// <summary>
        /// Get the pointer for an editor image. If no pointer if found, try to load it.
        /// </summary>
        public nint? GetImage(Texture2D texture, string id)
        {
            if (_images.TryGetValue(id, out TextureReference? textureId))
            {
                // There we go! Return it right away.
                return textureId.Value;
            }

            return CacheTexture(id, texture);
        }

        public bool DrawPreviewImage(string atlasFrameId, float maxSize, TextureAtlas? atlas, float scale = 1)
        {
            string id = $"preview_{atlasFrameId}";

            if (_images.TryGetValue(id, out TextureReference? textureId) &&
                Architect.Instance.ImGuiRenderer.GetLoadedTexture(textureId.Value) is Texture2D texture)
            {
                // Image is already cached, so draw it right away.
                DrawImage(textureId.Value, texture, maxSize, scale);

                return true;
            }

            if (atlas is null)
            {
                if (textureId is not null)
                {
                    // Fetch the texture directly from data in the lack of an atlas.
                    Texture2D t = Game.Data.FetchTexture(atlasFrameId);

                    CacheTexture(atlasFrameId, t);
                    DrawImage(textureId.Value, t, maxSize, scale);

                    return true;
                }

                return false;
            }

            nint? AtlasCoordinatesId = CreateTexture(atlas, atlasFrameId, id, 1f);
            if (AtlasCoordinatesId is not null)
            {
                DrawImage(AtlasCoordinatesId.Value, Architect.Instance.ImGuiRenderer.GetLoadedTexture(AtlasCoordinatesId.Value)!, maxSize, scale);
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

        private class TextureReference : IDisposable
        {
            public IntPtr Value { get; init; }

            public TextureReference(IntPtr value) => Value = value;

            public void Dispose()
            {
                Architect.Instance.ImGuiRenderer.UnbindTexture(Value);
            }
        }
    }
}