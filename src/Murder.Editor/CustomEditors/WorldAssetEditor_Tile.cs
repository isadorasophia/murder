using Bang.Components;
using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Stages;
using Murder.Editor.Utilities;
using Murder.ImGuiExtended;
using Murder.Prefabs;

namespace Murder.Editor.CustomEditors
{
    internal partial class WorldAssetEditor
    {
        protected virtual bool DrawTileEditor(Stage stage)
        {
            bool modified = false;

            IList<IEntity> rooms = stage.FindTileEntities();
            if (rooms.Count > 0)
            {
                foreach (IEntity room in rooms)
                {
                    if (ImGuiHelpers.DeleteButton($"Delete#{room.Guid}"))
                    {
                        DeleteInstance(parent: null, room.Guid);
                    }

                    ImGui.SameLine();

                    // Do not modify the name for entity assets, only instances.
                    if (ImGuiHelpers.IconButton('\uf304', $"rename_{room.Guid}"))
                    {
                        ImGui.OpenPopup($"Rename#{room.Guid}");
                    }

                    if (ImGui.BeginPopup($"Rename#{room.Guid}"))
                    {
                        if (DrawRenameInstanceModal(parent: null, room))
                        {
                            ImGui.CloseCurrentPopup();
                        }

                        ImGui.EndPopup();
                    }

                    ImGui.SameLine();

                    ImGui.Text(room.Name);

                    using TableMultipleColumns table = new("editor_settings", flags: ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.BordersOuter,
                    (ImGuiTableColumnFlags.WidthFixed, -1), (ImGuiTableColumnFlags.WidthStretch, -1));

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();

                    modified |= DrawRoomTilesetWithTable(room);

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TableNextColumn();

                    modified |= DrawRoomDimensionsWithTable(room);
                }
            }

            return modified;
        }

        /// <summary>
        /// Draw tileset preview of the room within a table.
        /// </summary>
        /// <returns>
        /// Whether it modified the value of asset.
        /// </returns>
        private bool DrawRoomTilesetWithTable(IEntity e)
        {
            bool modified = false;

            ImGui.Text("Tileset");
            ImGui.TableNextColumn();

            TilesetComponent tilesetComponent = (TilesetComponent)e.GetComponent(typeof(TilesetComponent));
            TilesetAsset? tileset = Game.Data.TryGetAsset<TilesetAsset>(tilesetComponent.Tileset);
            if (tileset is not null)
            {
                AssetsHelpers.DrawPreview(tileset);
            }
            else
            {
                ImGui.PushID("tileset_component_search");
                modified |= CustomField.DrawValue(ref tilesetComponent, nameof(TilesetComponent.Tileset));
                ImGui.PopID();
            }

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            ImGui.Text("Floor");
            ImGui.TableNextColumn();

            AsepriteAsset? floor = Game.Data.TryGetAsset<AsepriteAsset>(tilesetComponent.Floor);
            if (floor is not null)
            {
                AssetsHelpers.DrawPreview(floor);
            }
            else
            {
                ImGui.PushID("tileset_floor_search");
                modified |= CustomField.DrawValue(ref tilesetComponent, nameof(TilesetComponent.Floor));
                ImGui.PopID();
            }

            if (modified)
            {
                ReplaceComponent(parent: null, e, tilesetComponent);
            }

            return modified;
        }

        /// <summary>
        /// Draw dimensions of the room within a table.
        /// </summary>
        /// <returns>
        /// Whether it modified the value of asset.
        /// </returns>
        private bool DrawRoomDimensionsWithTable(IEntity e)
        {
            bool modified = false;
            TileGridComponent c = e.GetComponent<TileGridComponent>();

            ImGui.PushItemWidth(100);

            ImGui.Text("W");
            ImGui.SameLine();

            ImGui.PushID("tile_width");
            modified |= CustomField.DrawValue(ref c, nameof(TileGridComponent.Width));
            ImGui.PopID();

            ImGui.SameLine();

            ImGui.Text("H");
            ImGui.SameLine();

            ImGui.PushID("tile_height");
            modified |= CustomField.DrawValue(ref c, nameof(TileGridComponent.Height));
            ImGui.PopID();
            ImGui.PopItemWidth();

            if (modified)
            {
                TileGrid newGrid = c.Grid;
                newGrid.Resize(c.Width, c.Height, origin: c.Origin);

                ReplaceComponent(parent: null, e, new TileGridComponent(newGrid));
            }

            return modified;
        }
    }
}