using Bang.Contexts;
using Bang.Systems;
using Murder.Core.Graphics;

namespace Murder.Core
{
    /// <summary>
    /// Main render system. This is used to draw on the screen and should not 
    /// have any update logic.
    /// </summary>
    public interface IMonoRenderSystem : IRenderSystem
    {
        /// <summary>
        /// Called on rendering.
        /// </summary>
        public abstract ValueTask Draw(RenderContext render, Context context);
    }
}
