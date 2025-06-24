using Bang;
using Microsoft.Xna.Framework.Graphics;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Serialization;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Core.Graphics
{
    /// <summary>
    /// A texture atlas, the texture2D can be loaded and unloaded from the GPU at any time
    /// We will keep the texture lists in memory all the time, though.
    /// </summary>
    [Serializable]
    public class TextureAtlas : IDisposable
    {
        public readonly string AtlasId;

        [Serialize]
        private readonly Dictionary<string, AtlasCoordinates> _entries = new(StringComparer.InvariantCultureIgnoreCase);

        [Serialize]
        private int _atlasMaxIndex;

        private Texture2D[]? _textures = null!;
        private GraphicsDevice? _graphicsDevice;

        public Texture2D[] Textures
        {
            get
            {
                if (_textures is null)
                {
                    LoadTextures();
                }

                return _textures;
            }
        }

        public TextureAtlas(string atlasId)
        {
            AtlasId = atlasId;
        }

        public int CountEntries => _entries.Count;

        public IEnumerable<AtlasCoordinates> GetAllEntries() => _entries.Values;

        public void PopulateAtlas(IEnumerable<(string id, AtlasCoordinates coord)> entries)
        {
            _atlasMaxIndex = -1;

            foreach ((string Id, AtlasCoordinates Coord) entry in entries)
            {
                if (entry.Coord.AtlasIndex > _atlasMaxIndex)
                {
                    _atlasMaxIndex = entry.Coord.AtlasIndex;
                }

                _entries[entry.Id.EscapePath()] = entry.Coord;
            }
        }

        public bool HasId(string id)
        {
            return _entries.ContainsKey(id);
        }
        public bool TryGet(string id, out AtlasCoordinates coord)
        {
            if (_entries.TryGetValue(id, out AtlasCoordinates result))
            {
                coord = result;
                return true;
            }

            coord = AtlasCoordinates.Empty;
            return false;
        }

        public AtlasCoordinates Get(string id)
        {
            if (_entries.TryGetValue(id, out AtlasCoordinates coord))
            {
                return coord;
            }
            else
            {
                GameLogger.Log($"Image '{id}' is missing from the atlas");
                return Game.Data.FetchAtlas(AtlasIdentifiers.Editor).Get("missingImage");
            }
        }

        /// <summary>
        /// This creates a new texture on the fly and should be *AVOIDED!*. Use `Get` instead.
        /// </summary>
        /// <param name="textureCoord">Coordinate of where the texture is located in the atlas.</param>
        /// <param name="format">Specifies the surface format. Some resources require Color or some other setting.</param>
        /// <param name="scale">Scale which will be applied to result.</param>
        public Texture2D CreateTextureFromAtlas(AtlasCoordinates textureCoord, SurfaceFormat format = SurfaceFormat.Color, float scale = 1)
        {
            _graphicsDevice ??= Game.GraphicsDevice;

            RenderTarget2D result =
                new RenderTarget2D(_graphicsDevice,
                Calculator.RoundToInt(Math.Max(1, textureCoord.SourceRectangle.Width) * scale),
                Calculator.RoundToInt(Math.Max(1, textureCoord.SourceRectangle.Height) * scale), false, format, DepthFormat.None);

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
        public Texture2D CreateTextureFromAtlas(string id, float scale)
        {
            if (_entries.TryGetValue(id, out var textureCoord))
            {
                return CreateTextureFromAtlas(textureCoord, SurfaceFormat.Color, scale);
            }
            else
            {
                throw new ArgumentException($"Texture path not found in atlas '{id}'");
            }
        }

        /// <summary>
        /// Create a texture on the fly. Be careful, as the texture needs to be manually *disposed*!
        /// </summary>
        /// <param name="id">Texture identifier.</param>
        /// <param name="texture">Resultin texture.</param>
        /// <param name="scale">Scale of the texture.</param>
        /// <returns>Whether it succeeded fetching the texture.</returns>
        public bool TryCreateTexture(string id, [NotNullWhen(true)] out Texture2D? texture, float scale)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (_entries.ContainsKey(id))
                {
                    try
                    {
                        texture = CreateTextureFromAtlas(id, scale);
                        return true;
                    }
                    catch
                    { }
                }
            }

            texture = null;
            return false;
        }

        [MemberNotNull(nameof(_textures))]
        public void LoadTextures()
        {
            if (_textures is not null)
            {
                return;
            }

            string atlasPath = FileHelper.GetPath(Game.Data.PackedBinDirectoryPath, Game.Profile.AtlasFolderName);
            if (!Directory.Exists(atlasPath))
            {
                throw new FileNotFoundException($"Atlas '{AtlasId}' not found in '{atlasPath}'. No atlas directory exists!");
            }

            if (_atlasMaxIndex == -1)
            {
                throw new FileNotFoundException($"Atlas '{AtlasId}' not found in '{atlasPath}'");
            }

            GameLogger.LogPerf($"Loading textures for: {AtlasId}");

            _textures = new Texture2D[_atlasMaxIndex + 1];

            for (int i = 0; i < _textures.Length; ++i)
            {
                string path = Path.Join(atlasPath, $"{AtlasId}{i:000}{TextureServices.QOI_GZ_EXTENSION}");
                _textures[i] = TextureServices.FromFile(Game.GraphicsDevice, path);
                _textures[i].Name = $"(Atlas){AtlasId}{i:000}";

                GameLogger.Verify(Textures[i] is not null, $"Couldn't load atlas file at {path}");
            }
        }

        public void UnloadTextures()
        {
            if (_textures != null)
            {
                foreach (var t in _textures)
                {
                    t?.Dispose();
                }
            }

            _textures = null;
        }

        public void Dispose()
        {
            UnloadTextures();
            _entries.Clear();
        }
    }
}