using Assimp;
using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Diagnostics;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Stages;
using Murder.Editor.Utilities;
using Murder.Prefabs;

namespace Murder.Editor.CustomEditors
{
    internal partial class WorldAssetEditor
    {
        protected virtual bool DrawTileEditor(Stage stage)
        {
            GameLogger.Verify(_world is not null);

            bool modified = false;

            ImGui.Dummy(new(10.WithDpi(), 0));

            IList<IEntity> tileset = stage.FindEntitiesWith(typeof(TilesetComponent));
            if (tileset.Count != 0)
            {
                using TableMultipleColumns table = new("editor_settings_tile", flags: ImGuiTableFlags.NoBordersInBody,
                    (ImGuiTableColumnFlags.WidthFixed, (int)ImGui.GetContentRegionMax().X));

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                modified |= DrawRoomTilesetWithTable(stage, tileset[0]);
            }

            ImGui.Separator();
            ImGui.Dummy(new(10.WithDpi(), 0));

            IList<IEntity> rooms = stage.FindEntitiesWith(typeof(TileGridComponent));
            if (rooms.Count > 0)
            {
                foreach (IEntity room in rooms)
                {
                    if (ImGuiHelpers.DeleteButton($"Delete#{room.Guid}"))
                    {
                        DeleteEntityWithGroup(room.Guid);
                    }

                    ImGui.SameLine();

                    // Do not modify the name for entity assets, only instances.
                    if (ImGuiHelpers.IconButton('\uf304', $"rename_{room.Guid}"))
                    {
                        ImGui.OpenPopup($"Rename#{room.Guid}");
                    }

                    string? name = _world.GetGroupOf(room.Guid);

                    if (ImGui.BeginPopup($"Rename#{room.Guid}"))
                    {
                        if (DrawRenameInstanceModal(parent: null, room))
                        {
                            if (name is not null)
                            {
                                RenameGroup(name, newName: room.Name);
                                
                                // Update so the number of the room matches the group.
                                name = room.Name;
                            }

                            ImGui.CloseCurrentPopup();
                        }

                        ImGui.EndPopup();
                    }

                    ImGui.SameLine();

                    ImGui.Text(name ?? room.Name);

                    using TableMultipleColumns table = new("editor_settings", flags: ImGuiTableFlags.BordersOuter,
                        (ImGuiTableColumnFlags.WidthFixed, (int)ImGui.GetContentRegionMax().X));

                    // Do this so we can have a padding space between tables. There is probably a fancier api for this.
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();

                    modified |= DrawRoomDimensionsWithTable(room);

                    // Do this so we can have a padding space between tables. There is probably a fancier api for this.
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                }
            }

            return modified;
        }

        private void DeleteEntityWithGroup(Guid guid)
        {
            GameLogger.Verify(_world is not null);

            string? group = _world.GetGroupOf(guid);
            if (group is null)
            {
                // No group actually associated, just delete the instance.
                DeleteInstance(parent: null, guid);
                return;
            }

            DeleteGroupWithEntities(group);
        }
        
        /// <summary>
        /// Delete instance and all the entities in that same room.
        /// </summary>
        private void DeleteGroupWithEntities(string name)
        {
            GameLogger.Verify(_world is not null);
            
            foreach (Guid guid in _world.FetchEntitiesOfGroup(name))
            {
                DeleteInstance(parent: null, guid);
            }

            _world.DeleteGroup(name);
        }

        /// <summary>
        /// Draw tileset preview of the room within a table.
        /// </summary>
        /// <returns>
        /// Whether it modified the value of asset.
        /// </returns>
        private bool DrawRoomTilesetWithTable(Stage stage, IEntity e)
        {
            bool modified = false;

            int currentSelectedTile = stage.EditorHook.CurrentSelectedTile;

            TilesetComponent tilesetComponent = (TilesetComponent)e.GetComponent(typeof(TilesetComponent));

            {
                for (int i = 0; i < tilesetComponent.Tilesets.Length; ++i)
                {
                    if (i != 0)
                    {
                        ImGui.SameLine();
                    }
                    
                    TilesetAsset? tileset = Game.Data.TryGetAsset<TilesetAsset>(tilesetComponent.Tilesets[i]);
                    if (tileset is not null)
                    {
                        ImGui.PushID($"tileset_{i}");

                        if (EditorAssetHelpers.DrawPreviewButton(tileset, currentSelectedTile == i))
                        {
                            // Update new selected tile.
                            currentSelectedTile = stage.EditorHook.CurrentSelectedTile = i;
                        }

                        if (ImGui.BeginPopupContextItem())
                        {
                            if (ImGui.Selectable("Delete"))
                            {
                                tilesetComponent = tilesetComponent.WithTiles(tilesetComponent.Tilesets.RemoveAt(i));
                                modified = true;
                            }

                            ImGui.EndPopup();
                        }

                        ImGui.PopID();
                    }
                }

                ImGui.PushID("tileset_component_search");
                
                Guid newTileGuid = Guid.Empty;
                if (SearchBox.SearchAsset(ref newTileGuid, typeof(TilesetAsset), tilesetComponent.Tilesets.ToArray()))
                {
                    tilesetComponent = tilesetComponent.WithTile(newTileGuid);
                    modified = true;
                }
                
                ImGui.PopID();
            }

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            ImGui.PushID("tileset_floor_search");
            modified |= CustomField.DrawValue(ref tilesetComponent, nameof(TilesetComponent.Floor));
            ImGui.PopID();

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