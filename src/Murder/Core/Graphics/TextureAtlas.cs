using Murder.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Serialization;
using Murder.Services;
using Newtonsoft.Json;

namespace Murder.Core.Graphics
{
    /// <summary>
    /// A texture atlas, the texture2D can be loaded and unloaded from the GPU at any time
    /// We will keep the texture lists in memory all the time, though.
    /// </summary>
    public class TextureAtlas : IDisposable
    {
        /// <summary>Used publically only for the json serializer</summary>
        public Dictionary<string, AtlasCoordinates> _entries = new(StringComparer.InvariantCultureIgnoreCase);

        [JsonIgnore]
        private GraphicsDevice? _graphicsDevice;

        public readonly string Name;
        public readonly AtlasId Id;

        [JsonIgnore]
        private Texture2D[] _textures = null!;
        internal Texture2D[] Textures
        {
            get
            {
                if (_textures is null)
                {
                    LoadTextures();
                }

                return _textures!;
            }
        }

        public TextureAtlas(string name, AtlasId id)
        {
            Name = name;
            Id = id;
        }

        public bool Exist(string id) => _entries.ContainsKey(id.EscapePath());
        public int CountEntries => _entries.Count;
        public IEnumerable<AtlasCoordinates> GetAllEntries() => _entries.Values;

        public void PopulateAtlas(IEnumerable<(string id, AtlasCoordinates coord)> entries)
        {
            foreach (var entry in entries)
            {
                _entries[entry.id] = entry.coord;
            }
        }

        public bool HasId(string id)
        {
            return _entries.ContainsKey(id.EscapePath());
        }
        public bool TryGet(string id, out AtlasCoordinates coord)
        {
            if (_entries.TryGetValue(id.EscapePath(), out AtlasCoordinates result))
            {
                coord = result;
                return true;
            }
            coord = AtlasCoordinates.Empty;
            return false;
        }

        public AtlasCoordinates Get(string id)
        {
            if (_entries.TryGetValue(id.EscapePath(), out AtlasCoordinates coord))
            {
                return coord;
            }
            else
            {
                GameLogger.Log($"Image '{id}' is missing from the atlas");
                return Game.Data.FetchAtlas(AtlasId.Editor).Get("missingImage");
            }
        }

        /// <summary>
        /// This creates a new texture on the fly and should be *AVOIDED!*. Use `Get` instead.
        /// </summary>
        /// <param name="textureCoord">Coordinate of where the texture is located in the atlas.</param>
        /// <param name="format">Specifies the surface format. Some resources require Color or some other setting.</param>
        /// <param name="scale">Scale which will be applied to result.</param>
        public Texture2D CreateTextureFromAtlas(AtlasCoordinates textureCoord, SurfaceFormat format = SurfaceFormat.Color, int scale = 1)
        {
            _graphicsDevice ??= Game.GraphicsDevice;

            RenderTarget2D result =
                new RenderTarget2D(_graphicsDevice,
                Math.Max(1, textureCoord.SourceRectangle.Width) * scale,
                Math.Max(1, textureCoord.SourceRectangle.Height) * scale, false, format, DepthFormat.None);

            _graphicsDevice.SetRenderTarget(result);
            _graphicsDevice.Clear(Color.Transparent);

            // Draw the cropped image from the atlas
            RenderServices.DrawTextureQuad(
                textureCoord.Atlas,
                textureCoord.SourceRectangle,
                result.Bounds,
                Microsoft.Xna.Framework.Matrix.Identity,
                Color.White,
                BlendState.AlphaBlend
                );

            // Return the graphics device to the screen
            _graphicsDevice.SetRenderTarget(null);
            result.Name = textureCoord.Name;

            return result;
        }

        /// <summary>
        /// This creates a new texture on the fly and should be *AVOIDED!*. Use `Get` instead.
        /// </summary>
        public Texture2D CreateTextureFromAtlas(string id)
        {
            if (_entries.TryGetValue(id.EscapePath(), out var textureCoord))
            {
                return CreateTextureFromAtlas(textureCoord);
            }
            else
            {
                throw new ArgumentException($"Texture path not found in atlas '{id}'");
            }
        }

        /// <summary>
        /// Create a texture on the fly. Be careful, as the texture needs to be manually *disposed*!
        /// </summary>
        /// <param name="id"></param>
        /// <param name="texture"></param>
        /// <returns></returns>
        public bool TryCreateTexture(string id, out Texture2D texture)
        {
            var cleanName = id.EscapePath();

            if (!string.IsNullOrWhiteSpace(cleanName))
            {
                if (_entries.ContainsKey(cleanName))
                {
                    texture = CreateTextureFromAtlas(cleanName);
                    return true;
                }
            }

            texture = null!;
            return false;
        }

        public void LoadTextures()
        {
            string atlasPath = Path.Join(Game.Data.PackedBinDirectoryPath, Game.Profile.AtlasFolderName);
            var atlasFiles = new DirectoryInfo(atlasPath).EnumerateFiles($"{Id.GetDescription()}????.png").ToArray();

            if (atlasFiles.Length == 0)
            {
                throw new FileNotFoundException($"Atlas '{Id}' not found in '{atlasPath}'");
            }

            _textures = new Texture2D[atlasFiles.Length];

            for (int i = 0; i < atlasFiles.Length; i++)
            {
                var path = atlasFiles[i].FullName;
                Textures[i] = Texture2D.FromFile(Game.GraphicsDevice, path);
                GameLogger.Verify(Textures[i] is not null, $"Couldn't load atlas file at {path}");
            }
        }

        public void UnloadTextures()
        {
            if (_textures != null)
                foreach (var t in _textures)
                {
                    t.Dispose();
                }
        }

        public void Dispose()
        {
            UnloadTextures();
            _entries.Clear();
        }
    }
}