using Bang.Components;
using Bang.Contexts;
using Bang.Systems;
using Murder.Components.Cutscenes;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;

namespace Murder.Editor.Systems
{
    /// <summary>
    /// This system will draw selected entities and drag them through the map.
    /// </summary>
    [DoNotPause]
    [OnlyShowOnDebugView]
    [WorldEditor(startActive: true)]
    [Filter(ContextAccessorFilter.AllOf, ContextAccessorKind.Read, typeof(ITransformComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(CutsceneAnchorsComponent))] // Skip custscene renderer.
    public class EntitiesSelectorSystem : GenericSelectorSystem, IStartupSystem, IUpdateSystem, IGuiSystem, IMonoRenderSystem
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
