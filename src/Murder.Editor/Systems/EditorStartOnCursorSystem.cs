using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Systems;
using ImGuiNET;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Core;
using Murder.Editor.Services;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Editor.Systems
{
    [WorldEditor(startActive: false)]
    [OnlyShowOnDebugView]
    [DoNotPause]
    [Filter(ContextAccessorFilter.None)]
    public class EditorStartOnCursorSystem : IStartupSystem, IUpdateSystem, IMurderRenderSystem
    {
        private bool _pressedControl = false;

        /// <summary>
        /// Tracks tween for <see cref="_selectPosition"/>.
        /// </summary>
        private float _tweenStart;

        /// <summary>
        /// This is the position currently selected by the cursor.
        /// </summary>
        private Point? _selectPosition;

        /// <summary>
        /// Track existing save states that we can load a state from.
        /// </summary>
        private (Guid Guid, string Name)[]? _saveStateInfo = null;

        public void Start(Context context)
        {
            Guid guid = context.World.Guid();
            if (guid == Guid.Empty)
            {
                // Deactivate itself if this not belongs to a world asset.
                context.World.DeactivateSystem<EditorStartOnCursorSystem>();
            }
        }

        public void Update(Context context)
        {
            MonoWorld world = (MonoWorld)context.World;

            _pressedControl = Game.Input.Down(MurderInputButtons.Ctrl);
            if (_pressedControl)
            {
                _tweenStart = Game.Now;
                _selectPosition = EditorCameraServices.GetCursorWorldPosition(world);
            }

            if (_pressedControl && Game.Input.Pressed(MurderInputButtons.RightClick))
            {
                Architect.EditorSettings.TestWorldPosition = _selectPosition;
                Architect.Instance.QueueStartPlayingGame(quickplay: false, startingScene: world.WorldAssetGuid);
            }
        }

        public void Draw(RenderContext render, Context context)
        {
            DrawSelectionTween(render);
            DrawStartHere(context.World);
        }

        private void DrawSelectionTween(RenderContext render)
        {
            if (_selectPosition is Point position)
            {
                float tween = Ease.ZeroToOne(Ease.BackOut, 2f, _tweenStart);
                if (tween == 1)
                {
                    _selectPosition = null;
                }
                else
                {
                    float expand = (1 - tween) * 2;

                    float startAlpha = .9f;
                    Color color = Game.Profile.Theme.HighAccent * (startAlpha - startAlpha * tween);

                    float size = 3 + expand;

                    RenderServices.DrawCircleOutline(render.DebugBatch, position, size, 10, color);
                }
            }
        }

        /// <summary>
        /// This draws and create a new empty entity if the user prompts.
        /// </summary>
        private bool DrawStartHere(World world)
        {
            EditorHook hook = world.GetUnique<EditorComponent>().EditorHook;
            if (hook.EditorMode == EditorHook.EditorModes.EditMode)
            {
                return false;
            }

            if (ImGui.BeginPopupContextItem("GameplayContextMenu", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.NoReopen))
            {
                hook.IsPopupOpen = true;
                var allSaves = Game.Data.GetAllSaves();
                if (allSaves.Count == 0)
                {
                    if (ImGui.Selectable("Start playing here"))
                    {
                        hook.Cursor = CursorStyle.Normal;

                        Architect.EditorSettings.TestWorldPosition = hook.LastCursorWorldPosition;
                        Architect.Instance.QueueStartPlayingGame(quickplay: false, startingScene: world.Guid());
                    }
                }
                else
                {
                    if (ImGui.Selectable("Start playing here (New save)"))
                    {
                        hook.Cursor = CursorStyle.Normal;

                        Architect.EditorSettings.TestWorldPosition = hook.LastCursorWorldPosition;
                        Architect.Instance.QueueStartPlayingGame(new StartPlayGameInfo
                        {
                            IsQuickplay = false,
                            SaveSlot = Game.Data.GetNextAvailableSlot(),
                            StartingScene = world.Guid()
                        });
                    }
                }

                if (ImGui.BeginMenu("Start playing with save"))
                {
                    ImGui.Text("Select a save to start playing with");

                    if (allSaves.Count > 0)
                    {
                        ImGui.Separator();

                        foreach (var save in allSaves.Keys)
                        {
                            if (ImGui.MenuItem($"Slot {save}"))
                            {
                                hook.Cursor = CursorStyle.Normal;

                                Architect.EditorSettings.TestWorldPosition = hook.LastCursorWorldPosition;
                                Architect.Instance.QueueStartPlayingGame(new StartPlayGameInfo
                                {
                                    IsQuickplay = false,
                                    SaveSlot = save,
                                    StartingScene = world.Guid()
                                });
                            }
                        }
                    }
                    
                    _saveStateInfo ??= Architect.EditorData.GetAllAvailableStartGameFrom();

                    if (_saveStateInfo.Length > 0)
                    {
                        ImGui.Separator();

                        foreach ((Guid g, string name) in _saveStateInfo)
                        {
                            if (ImGui.MenuItem($"State '{name}'"))
                            {
                                hook.Cursor = CursorStyle.Normal;

                                Architect.EditorSettings.TestWorldPosition = hook.LastCursorWorldPosition;
                                Architect.Instance.QueueStartPlayingGame(new StartPlayGameInfo
                                {
                                    IsQuickplay = false,
                                    SaveSlot = Game.Data.GetNextAvailableSlot(),
                                    StartingScene = world.Guid(),
                                    LoadStateFrom = g
                                });
                            }
                        }
                    }
                    
                    ImGui.EndMenu();
                }

                ImGui.EndPopup();
            }
            else
            {
                hook.IsPopupOpen = false;
            }

            return true;
        }
    }
}