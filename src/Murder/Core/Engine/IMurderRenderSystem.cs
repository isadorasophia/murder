using Bang.Contexts;
using Bang.Systems;
using Murder.Components.Agents;

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



    /// <summary>
    /// Main render system. This is used to draw on the screen and should not 
    /// have any update logic. This one includes a converter for your own
    /// <see cref="RenderContext"/> that you extended.
    /// </summary>
    public interface IMurderRenderSystem<T> : IMurderRenderSystem where T : RenderContext
    {
        /// <summary>
        /// Called on rendering.
        /// </summary>
        public abstract void Draw(T render, Context context);

        void IMurderRenderSystem.Draw(RenderContext render, Context context)
        {
            Draw((T)render, context);
        }
    }

}