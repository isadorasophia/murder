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
        public Dictionary<string, AtlasTexture> _entries = new(StringComparer.InvariantCultureIgnoreCase);

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
        public IEnumerable<AtlasTexture> GetAllEntries() => _entries.Values;

        public void PopulateAtlas(IEnumerable<(string id, AtlasTexture coord)> entries)
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
        public bool TryGet(string id, out AtlasTexture coord)
        {
            if (_entries.TryGetValue(id.EscapePath(), out AtlasTexture result))
            {
                coord = result;
                return true;
            }
            coord = AtlasTexture.Empty;
            return false;
        }

        public AtlasTexture Get(string id)
        {
            if (_entries.TryGetValue(id.EscapePath(), out AtlasTexture coord))
            {
                return coord;
            }
            else
            {
                GameLogger.Log($"Image '{id}' is missing from the atlas");
                return _entries["missingImage"];
            }
        }

        /// <summary>
        /// This creates a new texture on the fly and should be *AVOIDED!*. Use `Get` instead.
        /// </summary>
        public Texture2D CreateTextureFromAtlas(string id)
        {
            if (_entries.TryGetValue(id.EscapePath(), out var textureCoord))
            {
                _graphicsDevice ??= Game.GraphicsDevice;

                RenderTarget2D result = 
                    new RenderTarget2D(_graphicsDevice,
                    Math.Max(1, textureCoord.SourceRectangle.Width),
                    Math.Max(1, textureCoord.SourceRectangle.Height), false, SurfaceFormat.Rgba64, DepthFormat.None);

                _graphicsDevice.SetRenderTarget(result);
                _graphicsDevice.Clear(Color.Transparent);

                // Draw the cropped image from the atlas
                RenderServices.DrawTextureQuad(
                    textureCoord.Atlas,
                    textureCoord.SourceRectangle,
                    new Rectangle(0, 0, textureCoord.SourceRectangle.Width, textureCoord.SourceRectangle.Height),
                    Microsoft.Xna.Framework.Matrix.Identity,
                    Color.White,
                    BlendState.AlphaBlend
                    );
                
                // Return the graphics device to the screen
                _graphicsDevice.SetRenderTarget(null);
                result.Name = id;

                return result;
            }
            else
            {
                throw new ArgumentException($"Texture path not found in atlas '{id}'");
            }
        }

        internal bool TryCreateTexture(string id, out Texture2D texture)
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
            var atlasFiles = new DirectoryInfo(Path.Join(Game.Data.ContentDirectoryPath, Game.Profile.AtlasFolderName)).EnumerateFiles($"{Id.GetDescription()}????.png").ToArray();
            
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
            if (Textures is not null)
                foreach (var t in Textures)
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