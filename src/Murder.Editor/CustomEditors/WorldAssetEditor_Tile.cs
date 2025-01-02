using Bang.Components;
using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Stages;
using Murder.Editor.Utilities;
using Murder.Prefabs;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Editor.CustomEditors
{
    internal partial class WorldAssetEditor
    {
        protected virtual bool DrawTileEditor(Stage stage)
        {
            bool modified = false;

            ImGui.Dummy(new(10, 0));

            IList<IEntity> tileset = stage.FindEntitiesWith(typeof(TilesetComponent));
            if (tileset.Count != 0)
            {
                using TableMultipleColumns table = new("editor_settings_tile", flags: ImGuiTableFlags.NoBordersInBody,
                    (ImGuiTableColumnFlags.WidthFixed, (int)ImGui.GetContentRegionAvail().X));

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                modified |= DrawRoomTilesetWithTable(stage, tileset[0]);
            }

            ImGui.Separator();
            ImGui.Dummy(new(10, 0));

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

                    string? name = _world?.GetGroupOf(room.Guid);

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
                        (ImGuiTableColumnFlags.WidthFixed, -1), (ImGuiTableColumnFlags.WidthFixed, (int)ImGui.GetContentRegionAvail().X));

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

                ImGui.Dummy(new Vector2(0, 2));
                ImGui.SameLine();

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

                        int buttonSize = 46;

                        ImGui.BeginGroup();

                        ImGui.Dummy(new Vector2(1, 0));


                        ImGui.Dummy(new Vector2(1, 0));
                        ImGui.SameLine();
                        if (EditorAssetHelpers.DrawPreviewButton(tileset, buttonSize, currentSelectedTile == i))
                        {
                            // Update new selected tile.
                            currentSelectedTile = stage.EditorHook.CurrentSelectedTile = i;
                        }
                        ImGui.SameLine();
                        ImGui.Dummy(new Vector2(1, 0));

                        ImGui.Dummy(new Vector2(1, 0));
                        ImGui.SameLine();
                        if (ImGui.Button(" ", new Vector2(22, 22)))
                        {
                            ImGui.OpenPopup("delete_tile");
                        }
                        ImGui.SameLine();
                        if (ImGui.Button("", new Vector2(22, 22)))
                        {
                            ImGui.OpenPopup("replace_tile");
                        }
                        ImGui.Dummy(new Vector2(1, 0));
                        
                        ImGui.EndGroup();
                        ImGuiHelpers.DrawBorderOnPreviousItem(currentSelectedTile == i? Game.Profile.Theme.White : Game.Profile.Theme.Faded, 2);


                        if (ImGui.BeginPopup("delete_tile", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.AlwaysAutoResize))
                        {
                            if (ImGui.Selectable("You sure?"))
                            {
                                tilesetComponent = tilesetComponent.WithTiles(tilesetComponent.Tilesets.RemoveAt(i));
                                modified = true;
                            }
                            ImGui.EndPopup();
                        }

                        if (ImGui.BeginPopup("replace_tile", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.AlwaysAutoResize))
                        {
                            ImGui.SeparatorText("Replace Tile");
                            Guid replaceTileGuid = Guid.Empty;

                            if (SearchBox.SearchAsset(ref replaceTileGuid, typeof(TilesetAsset), SearchBoxFlags.Unfolded, tilesetComponent.Tilesets.ToArray()))
                            {
                                tilesetComponent = tilesetComponent.WithTiles(tilesetComponent.Tilesets.Replace(tilesetComponent.Tilesets[i], replaceTileGuid));
                                modified = true;
                            }

                            ImGui.EndPopup();
                        }

                        ImGui.PopID();
                    }
                }

                ImGui.PushID("tileset_component_search");

                Guid newTileGuid = Guid.Empty;
                if (SearchBox.SearchAsset(ref newTileGuid, typeof(TilesetAsset), SearchBoxFlags.None, tilesetComponent.Tilesets.ToArray()))
                {
                    tilesetComponent = tilesetComponent.WithTile(newTileGuid);
                    modified = true;
                }

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

            RoomComponent r = e.GetComponent<RoomComponent>();

            ImGui.Text("Floor");
            ImGui.TableNextColumn();

            ImGui.PushID("tileset_floor_search");
            modified |= CustomField.DrawValue(ref r, nameof(RoomComponent.Floor));
            ImGui.PopID();

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            TileGridComponent c = e.GetComponent<TileGridComponent>();

            ImGui.Text("Width");
            ImGui.TableNextColumn();

            ImGui.PushItemWidth(150);
            ImGui.PushID("tile_width");
            modified |= CustomField.DrawValue(ref c, nameof(TileGridComponent.Width));
            ImGui.PopID();
            ImGui.PopItemWidth();

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            ImGui.Text("Height");
            ImGui.TableNextColumn();

            ImGui.PushItemWidth(150);
            ImGui.PushID("tile_height");
            modified |= CustomField.DrawValue(ref c, nameof(TileGridComponent.Height));
            ImGui.PopID();
            ImGui.PopItemWidth();

            if (modified)
            {
                TileGrid newGrid = c.Grid;
                newGrid.Resize(c.Width, c.Height, origin: c.Origin);

                ReplaceComponent(parent: null, e, new TileGridComponent(newGrid));
                ReplaceComponent(parent: null, e, r);
            }

            return modified;
        }

        /// <summary>
        /// Move an entire map by an offset.
        /// </summary>
        private void MoveMap(Stage stage, Point offset)
        {
            IList<IEntity> entities = stage.FindEntitiesWith(typeof(TileGridComponent));
            foreach (IEntity e in entities)
            {
                TileGridComponent c = e.GetComponent<TileGridComponent>();

                TileGrid newGrid = c.Grid;
                newGrid.Resize(new IntRectangle(c.Origin + offset, c.Width, c.Height));

                ReplaceComponent(parent: null, e, new TileGridComponent(newGrid));
            }

            MoveAllEntities(offset, Instances);
        }

        /// <summary>
        /// This will group all the entities of the map to a delta position from <paramref name="from"/> to <paramref name="to"/>.
        /// </summary>
        private void MoveAllEntities(Point offset, ImmutableArray<Guid> entities)
        {
            GameLogger.Verify(_world is not null);

            Point worldDelta = offset * Grid.CellSize;

            foreach (Guid guid in entities)
            {
                if (_world?.TryGetInstance(guid) is not IEntity entity)
                {
                    // Entity is not valid?
                    continue;
                }

                if (!entity.HasComponent(typeof(PositionComponent)))
                {
                    // Entity doesn't really have a transform component to move.
                    continue;
                }

                IMurderTransformComponent? transform =
                    (PositionComponent)entity.GetComponent(typeof(PositionComponent));

                ReplaceComponent(parent: null, entity, (PositionComponent)transform.Add(worldDelta));
            }
        }
    }
}