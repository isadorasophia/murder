using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Core.Graphics;
using Murder.Data;
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
        /// Draw an ImGui preview image of the asset.
        /// </summary>
        /// <returns>
        /// Whether the preview succeeded.
        /// </returns>
        public static bool DrawPreview(AsepriteAsset asset)
        {
            return DrawPreview(AtlasId.Gameplay, idToDraw: asset.Frames[0]);
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
                Architect.ImGuiTextureManager.Image(idToDraw, 256, gameplayAtlas, Game.Instance.DPIScale / 100);
        }

        /// <summary>
        /// Draw an ImGui preview image of the tileset asset.
        /// </summary>
        /// <returns>
        /// Whether the preview succeeded.
        /// </returns>
        public static bool DrawPreview(TilesetAsset asset)
        {
            AsepriteAsset? image = Game.Data.TryGetAsset<AsepriteAsset>(asset.Image);
            if (image is null)
            {
                return false;
            }

            //var texture = asset.CreatePreviewImage();

            foreach (string frame in image.Frames)
            {
                DrawPreview(AtlasId.Gameplay, frame);
            }

            return true;
        }
    }
}