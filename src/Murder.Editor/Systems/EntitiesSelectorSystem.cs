using Bang.Components;
using Bang.Contexts;
using Bang.Systems;
using ImGuiNET;
using Murder.Components;
using Murder.Components.Cutscenes;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;

namespace Murder.Editor.Systems
{
    /// <summary>
    /// This system will draw selected entities and drag them through the map.
    /// </summary>
    [DoNotPause]
    [OnlyShowOnDebugView]
    [WorldEditor(startActive: true)]
    [Filter(ContextAccessorFilter.AllOf, ContextAccessorKind.Read, typeof(ITransformComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(CutsceneAnchorsComponent), typeof(SoundParameterComponent))] // Skip cutscene and sounds.
    public class EntitiesSelectorSystem : GenericSelectorSystem, IStartupSystem, IUpdateSystem, IGuiSystem, IMurderRenderSystem
    {
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

                if (hook.AllSelectedEntities.Count>0 &&
                    hook.AvailableFolders != null && hook.AvailableFolders.Value.Length > 0)
                {
                    // Move to context menu
                    if (ImGui.BeginPopupContextItem())
                    {
                        if (ImGui.BeginMenu("Move To..."))
                        {
                            foreach (var room in hook.AvailableFolders.Value)
                            {
                                if (ImGui.MenuItem(room))
                                {
                                    hook.MoveSelectedEntitiesToFolder?.Invoke(room);
                                }
                            }
                            ImGui.EndMenu();
                        }

                        ImGui.EndPopup();
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