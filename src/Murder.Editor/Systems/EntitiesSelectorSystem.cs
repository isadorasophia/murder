﻿using Bang.Components;
using Bang.Contexts;
using Bang.Systems;
using ImGuiNET;
using Murder.Components;
using Murder.Components.Cutscenes;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using System.Numerics;

namespace Murder.Editor.Systems
{
    /// <summary>
    /// This system will draw selected entities and drag them through the map.
    /// </summary>
    [DoNotPause]
    [OnlyShowOnDebugView]
    [WorldEditor(startActive: true)]
    [Filter(ContextAccessorFilter.AllOf, ContextAccessorKind.Read, typeof(ITransformComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(CutsceneAnchorsComponent), typeof(SoundParameterComponent), typeof(SkipComponent))] // Skip cutscene and sounds.
    public class EntitiesSelectorSystem : GenericSelectorSystem, IStartupSystem, IUpdateSystem, IGuiSystem, IMurderRenderSystem
    {
        private Vector2 _previousCursorPosition;
        private float _lastMove;
        public void Start(Context context)
        {
            StartImpl(context.World);
        }

        /// <summary>
        /// This is only used for rendering the entity components during the game (on debug mode).
        /// </summary>
        public void DrawGui(RenderContext render, Context context)
        {
            if (render.RenderToScreen)
            {
                DrawGuiImpl(context.World, context.Entities);
            }

            if (context.World.TryGetUnique<EditorComponent>() is EditorComponent editorComponent)
            {
                EditorHook hook = editorComponent.EditorHook;

                IEnumerable<string>? availableFolders = hook.GetAvailableFolders?.Invoke();

                if (hook.AllSelectedEntities.Count > 0 && 
                    availableFolders is not null && availableFolders.Any())
                {
                    // Move to context menu
                    if (ImGui.BeginPopupContextItem())
                    {
                        if (ImGui.BeginMenu("Move To..."))
                        {
                            foreach (string room in availableFolders)
                            {
                                if (ImGui.MenuItem(room))
                                {
                                    hook.MoveEntitiesToFolder?.Invoke(room, hook.AllSelectedEntities.Keys);
                                }
                            }

                            ImGui.EndMenu();
                        }

                        ImGui.EndPopup();
                    }
                }

                if (hook.Hovering.Length > 0)
                {
                    if (hook.CursorWorldPosition is not null)
                    {
                        if (_previousCursorPosition == hook.CursorWorldPosition)
                        {
                            if (_lastMove < Game.NowUnscaled - 0.2f && !hook.CursorIsBusy.Any() && !hook.UsingGui)
                            {
                                if (ImGui.BeginTooltip())
                                {
                                    foreach (var entity in hook.Hovering)
                                    {
                                        if (hook.AllSelectedEntities.Where(e => e.Key == entity).Any())
                                        {
                                            ImGui.TextColored(Game.Profile.Theme.Accent, $"({entity}) {hook.GetNameForEntityId?.Invoke(entity)}");
                                        }
                                        else
                                        {
                                            ImGui.Text($"({entity}) {hook.GetNameForEntityId?.Invoke(entity)}");
                                        }
                                    }

                                    ImGui.EndTooltip();
                                }
                            }
                        }
                        else
                        {
                            _previousCursorPosition = hook.CursorWorldPosition.Value;
                            _lastMove = Game.NowUnscaled;
                        }
                    }

                }
            }
        }

        public void Update(Context context)
        {
            Update(context.World, context.Entities);
        }

        public void Draw(RenderContext render, Context context)
        {
            DrawImpl(render, context.World, context.Entities);
        }
    }
}