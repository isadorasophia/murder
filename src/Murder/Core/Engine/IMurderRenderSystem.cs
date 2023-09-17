using Bang.Contexts;
using Bang.Systems;

namespace Murder.Core.Graphics
{
    /// <summary>
    /// Main render system. This is used to draw on the screen and should not 
    /// have any update logic.
    /// </summary>
    public interface IMurderRenderSystem : IRenderSystem
    {
        /// <summary>
        /// Called on rendering.
        /// </summary>
        public abstract void Draw(RenderContext render, Context context);
    }
}
