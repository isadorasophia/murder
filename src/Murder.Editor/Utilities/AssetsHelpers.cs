using Assimp.Unmanaged;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Editor.Data;
using Murder.ImGuiExtended;
using Murder.Serialization;
using System;
using System.Runtime.CompilerServices;
using static Murder.Editor.Data.Graphics.Aseprite;
using static System.Net.Mime.MediaTypeNames;

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
                Architect.ImGuiTextureManager.DrawPreviewImage(idToDraw, 256, gameplayAtlas, Game.Instance.DPIScale / 100);
        }

        private static string GetTextureId(GameAsset asset) => asset.Guid.ToString();

        public static bool DrawPreviewButton(PrefabAsset asset, int size, bool pressed)
        {
            bool clicked = false;
            string id = GetTextureId(asset);
            nint? texturePtr = Architect.ImGuiTextureManager.FetchTexture(id);

            if (texturePtr is null)
            {
                AsepriteComponent? asepriteComponent = asset.HasComponent(typeof(AsepriteComponent)) ?
                    (AsepriteComponent)asset.GetComponent(typeof(AsepriteComponent)) : null;

                AgentSpriteComponent? agentSpriteComponent = asset.HasComponent(typeof(AgentSpriteComponent)) ?
                    (AgentSpriteComponent)asset.GetComponent(typeof(AgentSpriteComponent)) : null;

                Guid? animationGuid = asepriteComponent?.AnimationGuid ?? agentSpriteComponent?.AnimationGuid;
                string? animationId = asepriteComponent?.AnimationId;

                // Entity does not have an aseprite, look for its children.
                if (asepriteComponent is null)
                {
                    foreach (Guid child in asset.Children)
                    {
                        asepriteComponent =
                            asset.GetChildComponents(child).Where(c => c is AsepriteComponent).FirstOrDefault() as AsepriteComponent?;

                        if (asepriteComponent is not null)
                        {
                            // Found it!
                            animationGuid = asepriteComponent.Value.AnimationGuid;
                            animationId = asepriteComponent.Value.AnimationId;

                            break;
                        }
                    }
                }

                if (animationGuid is not null)
                {
                    AsepriteAsset aseprite = Game.Data.GetAsset<AsepriteAsset>(animationGuid.Value);
                    string frameId = string.IsNullOrEmpty(animationId) ? aseprite.Frames[0] :
                        aseprite.Animations[animationId].Frames[0];

                    if (Game.Data.TryFetchAtlas(AtlasId.Gameplay) is TextureAtlas atlas)
                    {
                        texturePtr = Architect.ImGuiTextureManager.CreateTexture(atlas, frameId, id);
                    }
                }
            }

            if (texturePtr is null)
            {
                texturePtr = Architect.ImGuiTextureManager.MissingImage();
                if (texturePtr is null)
                {
                    return false;
                }
            }

            ImGui.PushID($"{asset.Guid}_preview");

            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1f);

            ImGui.PushStyleColor(ImGuiCol.Button, Game.Profile.Theme.BgFaded);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Game.Profile.Theme.Bg);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, Game.Profile.Theme.Bg);

            System.Numerics.Vector2 dimensions = new(size, size);
            if (pressed)
            {
                ImGuiHelpers.SelectedImageButton(texturePtr.Value, dimensions);
            }
            else
            {
                clicked = ImGui.ImageButton(texturePtr.Value, dimensions);
                ImGuiHelpers.HelpTooltip(asset.GetSimplifiedName());
            }
            
            ImGui.PopStyleVar();
            ImGui.PopStyleColor(3);
            ImGui.PopID();

            // Unsupported.
            return clicked;
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
        public static bool DrawPreviewButton(AsepriteAsset asset, bool pressed, string? frameId)
        {
            bool clicked = false;

            string id = GetTextureId(asset);
            nint? texturePtr = Architect.ImGuiTextureManager.FetchTexture(id);
            
            if (texturePtr is null && Game.Data.TryFetchAtlas(AtlasId.Gameplay) is TextureAtlas atlas)
            {
                texturePtr = Architect.ImGuiTextureManager.CreateTexture(atlas, string.IsNullOrEmpty(frameId) ? asset.Frames[0] : frameId, id);
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