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

        public async ValueTask PreDraw()
        {
            foreach (var (_, (system, contextId)) in _cachedRenderSystems)
            {
                if (system is IMonoPreRenderSystem preRenderSystem)
                {
                    await preRenderSystem.BeforeDraw(Contexts[contextId]);
                }
            }
        }

        public async ValueTask Draw(RenderContext render)
        {
            foreach (var (_, (system, contextId)) in _cachedRenderSystems)
            {
                if (system is IMonoRenderSystem monoSystem)
                {
                    await monoSystem.Draw(render, Contexts[contextId]);
                }
            }
        }

        public async ValueTask DrawGui(RenderContext render)
        {
            foreach (var (_, (system, contextId)) in _cachedRenderSystems)
            {
                if (system is IGuiSystem monoSystem)
                {
                    await monoSystem.DrawGui(render, Contexts[contextId]);
                }
            }
        }
    }
}
