using Bang.Contexts;
using Bang.Systems;

namespace Murder.Core.Graphics
{
    /// <summary>
    /// System for rendering Gui entities.
    /// </summary>
    public interface IGuiSystem : IRenderSystem
    {
        /// <summary>
        /// Called before rendering starts.
        /// </summary>
        public abstract void DrawGui(RenderContext render, Context context);
    }
}
