using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Editor.ImGuiExtended;
using Murder.Serialization;

namespace Murder.Editor.Utilities
{
    public static class EditorAssetHelpers
    {
        /// <summary>
        /// Get the path to load or save <paramref name="asset"/>.
        /// </summary>
        public static string? GetEditorAssetPath(this GameAsset asset, bool useBinPath = false)
        {
            if (!string.IsNullOrEmpty(asset.FilePath) && Path.IsPathRooted(asset.FilePath))
            {
                // Not valid for bin paths.
                return useBinPath ? null : asset.FilePath;
            }

            if (asset.IsStoredInSaveData)
            {
                // Not valid for bin paths.
                return useBinPath ? null : Path.Join(GameDataManager.SaveBasePath, asset.FilePath);
            }

            return FileHelper.GetPath(
                useBinPath ? Architect.EditorSettings.BinResourcesPath : Architect.EditorSettings.SourceResourcesPath,
                asset.StoreInDatabase ? Game.Profile.AssetResourcesPath : string.Empty,
                asset.SaveLocation,
                asset.FilePath);
        }

        public static string? GetEditorAssetDirectoryPath(this GameAsset asset)
        {
            if (!string.IsNullOrEmpty(asset.FilePath) && Path.IsPathRooted(asset.FilePath))
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

        public static bool DrawPreview(SpriteAsset asset, int maxSize, string animationId)
        {
            (AtlasId atlas, _) = asset.GetPreviewId();

            // Override default id.
            animationId = animationId != null && asset.Animations.ContainsKey(animationId) ?
                animationId : asset.Animations.First().Key;
            int frame = asset.Animations[animationId].Frames[0];

            // TODO: [Editor] Fix this logic when the atlas comes from somewhere else. Possibly refactor AtlasId? Save it in the asset?
            if (!DrawPreview(atlas, asset.Frames[frame].Name, maxSize))
            {
                return DrawPreview(AtlasId.Editor, asset.Frames[frame].Name, maxSize);
            }

            return false;
        }

        /// <summary>
        /// Draw an ImGui preview image of the id to frame within the Gameplay atlas.
        /// </summary>
        /// <returns>
        /// Whether the preview succeeded.
        /// </returns>
        private static bool DrawPreview(AtlasId atlasId, string idToDraw, int maxSize = 256)
        {
            return Game.Data.TryFetchAtlas(atlasId) is TextureAtlas gameplayAtlas &&
                Architect.ImGuiTextureManager.DrawPreviewImage(idToDraw, maxSize, gameplayAtlas);
        }

        private static string GetTextureId(GameAsset asset) => asset.Guid.ToString();

        private static string GetTextureId(GameAsset asset, string? animationId) => $"{asset.Guid}_{animationId}";

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
                string? animationId = asepriteComponent?.CurrentAnimation;

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
                            animationId = asepriteComponent.Value.CurrentAnimation;

                            break;
                        }
                    }
                }

                if (animationGuid is not null)
                {
                    SpriteAsset? aseprite = Game.Data.TryGetAsset<SpriteAsset>(animationGuid.Value);
                    if (aseprite is null)
                    {
                        return false;
                    }

                    string frameId = string.IsNullOrEmpty(animationId) ? aseprite.Frames[0].Name : aseprite.Animations.ContainsKey(animationId) ?
                        aseprite.Frames[aseprite.Animations[animationId].Frames[0]].Name : aseprite.Frames[0].Name;

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

            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1f);

            ImGui.PushStyleColor(ImGuiCol.Button, Game.Profile.Theme.BgFaded);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Game.Profile.Theme.Bg);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, Game.Profile.Theme.Bg);

            System.Numerics.Vector2 dimensions = new(size, size);
            if (pressed)
            {
                ImGuiHelpers.SelectedImageButton(texturePtr.Value, dimensions, color: Game.Profile.Theme.Bg);
            }
            else
            {
                clicked = ImGui.ImageButton($"{asset.Guid}_preview", texturePtr.Value, dimensions);
            }

            ImGuiHelpers.HelpTooltip(asset.GetSimplifiedName());

            ImGui.PopStyleVar();
            ImGui.PopStyleColor(3);

            // Unsupported.
            return clicked;
        }

        public static bool DrawPrettyPreviewButton(SpriteAsset asset, string animationId, Vector2 size, bool pressed)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1f);

            ImGui.PushStyleColor(ImGuiCol.Button, Game.Profile.Theme.BgFaded);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Game.Profile.Theme.BgFaded * .9f);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, Game.Profile.Theme.BgFaded * .9f);

            bool result = EditorAssetHelpers.DrawPreviewButton(asset, animationId, size, pressed);

            ImGui.PopStyleVar();
            ImGui.PopStyleColor(3);

            return result;
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
        public static bool DrawPreviewButton(SpriteAsset asset, string? animationId, Vector2 size, bool pressed)
        {
            bool clicked = false;
            
            string id = GetTextureId(asset, animationId);
            string frameName = animationId is not null && asset.Animations.ContainsKey(animationId) ?
                asset.Frames[asset.Animations[animationId].Frames[0]].Name : asset.Frames[0].Name;
            
            nint? texturePtr = Architect.ImGuiTextureManager.FetchTexture(id);
            
            if (texturePtr is null && Game.Data.TryFetchAtlas(AtlasId.Gameplay) is TextureAtlas atlas)
            {
                texturePtr = Architect.ImGuiTextureManager.CreateTexture(atlas, frameName, id);
            }

            if (texturePtr is null)
            {
                return ImGui.Button("Recover image?");
            }
            
            if (pressed)
            {
                ImGuiHelpers.SelectedImageButton(texturePtr.Value, size);
            }
            else
            {
                clicked = ImGui.ImageButton($"{asset.Guid}_preview", texturePtr.Value, size);
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

            SpriteAsset? image = Game.Data.TryGetAsset<SpriteAsset>(asset.Image);
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
                System.Numerics.Vector2 size = new(Grid.CellSize * 2, Grid.CellSize * 2);
                if (pressed)
                {
                    ImGuiHelpers.SelectedImageButton(texturePtr.Value, size);
                }
                else
                {
                    clicked = ImGui.ImageButton($"{asset.Guid}_preview", texturePtr.Value, size);
                }

                ImGuiHelpers.HelpTooltip(asset.Name);
            }

            return clicked;
        }

        public static bool DrawShapeCombo(string id, Type? currentType, out IShape? shape, string additionalOption, params Type[] supportedShapes)
        {
            shape = null;

            int index = currentType == null ? -1 : Array.IndexOf(supportedShapes, currentType);

            string[] shapeToString = supportedShapes.Select(k => Prettify.FormatNameWithoutSuffix(k.Name, "Shape"))
                .Append(additionalOption).ToArray();
            if (index == -1)
            {
                // Point to "none"!
                index = shapeToString.Length - 1;
            }

            if (ImGui.Combo($"##{id}", ref index, shapeToString, shapeToString.Length))
            {
                if (index != shapeToString.Length - 1)
                {
                    shape = Activator.CreateInstance(supportedShapes[index]) as IShape;
                }

                return true;
            }

            return false;
        }
        
        public static bool DrawComboBoxFor(Guid guid, ref string animationId)
        {
            bool modified = false;

            if (Game.Data.TryGetAsset<SpriteAsset>(guid) is SpriteAsset ase)
            {
                if (ImGui.BeginCombo($"##AnimationID", animationId))
                {
                    foreach (string value in ase.Animations.Keys)
                    {
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            continue;
                        }

                        if (ImGui.MenuItem(value))
                        {
                            animationId = value;
                            modified = true;
                        }
                    }

                    ImGui.EndCombo();
                }
            }

            return modified;
        }
    }
}