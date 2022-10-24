using Bang.Contexts;
using Bang.Systems;

namespace Murder.Core.Graphics
{
    /// <summary>
    /// System called right before rendering.
    /// </summary>
    public interface IMonoPreRenderSystem : IRenderSystem
    {
        /// <summary>
        /// Called before rendering starts.
        /// This gets called before the SpriteBatch.Begin() and SpriteBatch.End() starts.
        /// </summary>
        public abstract ValueTask BeforeDraw(Context context);
    }
}
