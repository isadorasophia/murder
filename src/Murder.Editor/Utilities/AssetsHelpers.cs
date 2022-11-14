using Assimp.Unmanaged;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Editor.Data;
using Murder.ImGuiExtended;
using Murder.Serialization;
using System.Runtime.CompilerServices;

namespace Murder.Editor.Utilities
{
    internal static class AssetsHelpers
    {
        /// <summary>
        /// Get the path to load or save <paramref name="asset"/>.
        /// </summary>
        public static string? GetAssetPath(this GameAsset asset, bool useBinPath = false)
        {
            if (string.IsNullOrEmpty(asset.FilePath) && Path.IsPathRooted(asset.FilePath))
            {
                // Not valid for bin paths.
                return useBinPath ? null : asset.FilePath;
            }

            if (asset.IsStoredInSaveData)
            {
                // Not valid for bin paths.
                return useBinPath ? null : Path.Join(GameDataManager.SaveBasePath, asset.SaveLocation, asset.FilePath);
            }

            return FileHelper.GetPath(
                useBinPath ? Architect.EditorSettings.BinResourcesPath : Architect.EditorSettings.SourceResourcesPath,
                asset.StoreInDatabase ? Game.Profile.AssetResourcesPath : string.Empty,
                asset.SaveLocation,
                asset.FilePath);
        }

        public static string? GetAssetDirectoryPath(this GameAsset asset)
        {
            if (string.IsNullOrEmpty(asset.FilePath) && Path.IsPathRooted(asset.FilePath))
            {
                return Path.GetDirectoryName(asset.FilePath);
            }

            if (asset.IsStoredInSaveData)
            {
                return Path.GetDirectoryName(
                    Path.Join(GameDataManager.SaveBasePath, asset.SaveLocation, asset.FilePath));
            }

            return Path.GetDirectoryName(FileHelper.GetPath(
                Architect.EditorSettings.SourceResourcesPath,
                Game.Profile.AssetResourcesPath,
                asset.SaveLocation,
                asset.FilePath));
        }

        public static bool DrawPreview(IPreview preview)
        {
            (AtlasId atlas, string id) = preview.GetPreviewId();

            // TODO: [Editor] Fix this logic when the atlas comes from somewhere else. Possibly refactor AtlasId? Save it in the asset?
            if (!DrawPreview(atlas, id))
            {
                return DrawPreview(AtlasId.Editor, id);
            }

            return false;
        }
        
        /// <summary>
        /// Draw an ImGui preview image of the id to frame within the Gameplay atlas.
        /// </summary>
        /// <returns>
        /// Whether the preview succeeded.
        /// </returns>
        private static bool DrawPreview(AtlasId atlasId, string idToDraw)
        {
            return Game.Data.TryFetchAtlas(atlasId) is TextureAtlas gameplayAtlas &&
                Architect.ImGuiTextureManager.DrawImage(idToDraw, 256, gameplayAtlas, Game.Instance.DPIScale / 100);
        }

        /// <summary>
        /// Draw an ImGui preview image of the asset.
        /// </summary>
        /// <returns>
        /// Whether the preview succeeded.
        /// </returns>
        /// <param name="asset">Asset to be drawn.</param>
        /// <param name="pressed">
        /// Whether the button is already presset or not. If so,
        /// it will always return false, since the button will not be interactable.
        /// </param>
        public static bool DrawPreviewButton(AsepriteAsset asset, bool pressed)
        {
            bool clicked = false;

            string id = asset.Guid.ToString();
            nint? texturePtr = Architect.ImGuiTextureManager.FetchTexture(id);
            
            if (texturePtr is null && Game.Data.TryFetchAtlas(AtlasId.Gameplay) is TextureAtlas atlas)
            {
                texturePtr = Architect.ImGuiTextureManager.CreateTexture(atlas, asset.Frames[0], id);
            }

            if (texturePtr is null)
            {
                return false;
            }
            
            System.Numerics.Vector2 size = new(Grid.CellSize, Grid.CellSize);
            if (pressed)
            {
                ImGuiHelpers.SelectedImageButton(texturePtr.Value, size);
            }
            else
            {
                clicked = ImGui.ImageButton(texturePtr.Value, size);
            }

            return clicked;
        }
        
        /// <summary>
        /// Draw an ImGui preview image of the tileset asset.
        /// </summary>
        /// <returns>
        /// Whether the preview succeeded.
        /// </returns>
        /// <param name="asset">Asset to be drawn.</param>
        /// <param name="pressed">
        /// Whether the button is already presset or not. If so,
        /// it will always return false, since the button will not be interactable.
        /// </param>
        public static bool DrawPreviewButton(TilesetAsset asset, bool pressed)
        {
            bool clicked = false;

            AsepriteAsset? image = Game.Data.TryGetAsset<AsepriteAsset>(asset.Image);
            if (image is null)
            {
                return false;
            }

            string id = asset.Guid.ToString();
            if (!Architect.ImGuiTextureManager.HasTexture(id))
            {
                Texture2D t = asset.CreatePreviewImage();
                Architect.ImGuiTextureManager.CacheTexture(id, t);
            }

            IntPtr? texturePtr = Architect.ImGuiTextureManager.FetchTexture(id);
            if (texturePtr is not null)
            {
                System.Numerics.Vector2 size = new(Grid.CellSize, Grid.CellSize);
                if (pressed)
                {
                    ImGuiHelpers.SelectedImageButton(texturePtr.Value, size);
                }
                else
                {
                    clicked = ImGui.ImageButton(texturePtr.Value, size);
                }            
            }

            return clicked;
        }
    }
}