using Murder.Core.Graphics;
using Bang;
using Bang.Systems;

namespace Murder.Core
{
    /// <summary>
    /// World implementation based in MonoGame.
    /// </summary>
    public class MonoWorld : World
    {
        public readonly Camera2D Camera;

        public readonly Guid WorldAssetGuid;

        public MonoWorld(
            IList<(ISystem system, bool isActive)> systems, 
            Camera2D camera,
            Guid worldAssetGuid) : base(systems) => (Camera, WorldAssetGuid) = (camera, worldAssetGuid);

        public override void Pause()
        {
            base.Pause();
            Game.Instance.Pause();
        }

        public override void Resume()
        {
            base.Resume();
            Game.Instance.Resume();
        }

        public void PreDraw()
        {
            foreach (var (_, (system, contextId)) in _cachedRenderSystems)
            {
                if (system is IMonoPreRenderSystem preRenderSystem)
                {
                    preRenderSystem.BeforeDraw(Contexts[contextId]);
                }
            }
        }

        public void Draw(RenderContext render)
        {
            foreach (var (_, (system, contextId)) in _cachedRenderSystems)
            {
                if (system is IMonoRenderSystem monoSystem)
                {
                    monoSystem.Draw(render, Contexts[contextId]);
                }
            }
        }

        public void DrawGui(RenderContext render)
        {
            foreach (var (_, (system, contextId)) in _cachedRenderSystems)
            {
                if (system is IGuiSystem monoSystem)
                {
                    monoSystem.DrawGui(render, Contexts[contextId]);
                }
            }
        }
    }
}
