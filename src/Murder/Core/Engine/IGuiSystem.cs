using Murder.Core.Graphics;
using Bang.Contexts;
using Bang.Systems;

namespace Murder.Core
{
    /// <summary>
    /// System for rendering Gui entities.
    /// </summary>
    internal interface IGuiSystem : IRenderSystem
    {
        /// <summary>
        /// Called before rendering starts.
        /// This gets called before the ImGuiRenderer.BeforeLayout() and ImGuiRenderer.AfterLayout() starts.
        /// </summary>
        public abstract ValueTask DrawGui(RenderContext render, Context context);
    }
}
