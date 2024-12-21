using Bang;
using Bang.Systems;
using Murder.Core.Graphics;
using Murder.Core.Sounds;
using Murder.Services;

namespace Murder.Core
{
    /// <summary>
    /// World implementation based in MonoGame.
    /// </summary>
    public partial class MonoWorld : World
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

            SoundServices.Pause(SoundLayer.Sfx);
        }

        public override void Resume()
        {
            base.Resume();
            Game.Instance.Resume();

            SoundServices.Resume(SoundLayer.Sfx);
        }

        public void PreDraw()
        {
            // TODO: Do not make a copy every frame.
            foreach (var (systemId, (system, contextId)) in _cachedRenderSystems)
            {
                if (system is IMonoPreRenderSystem preRenderSystem)
                {
                    if (DIAGNOSTICS_MODE)
                    {
                        _stopwatch.Reset();
                        _stopwatch.Start();
                    }

                    preRenderSystem.BeforeDraw(Contexts[contextId]);

                    if (DIAGNOSTICS_MODE)
                    {
                        InitializeDiagnosticsCounters();

                        _stopwatch.Stop();
                        PreRenderCounters[systemId].Update(_stopwatch.Elapsed.TotalMicroseconds, Contexts[contextId].Entities.Length);
                    }
                }
            }
        }

        public void Draw(RenderContext render)
        {
            // TODO: Do not make a copy every frame.
            foreach (var (systemId, (system, contextId)) in _cachedRenderSystems)
            {
                if (system is IMurderRenderSystem monoSystem)
                {
                    if (DIAGNOSTICS_MODE)
                    {
                        _stopwatch.Reset();
                        _stopwatch.Start();
                    }

                    monoSystem.Draw(render, Contexts[contextId]);

                    if (DIAGNOSTICS_MODE)
                    {
                        InitializeDiagnosticsCounters();

                        _stopwatch.Stop();
                        RenderCounters[systemId].Update(_stopwatch.Elapsed.TotalMicroseconds, Contexts[contextId].Entities.Length);
                    }
                }
            }
        }

        public void DrawGui(RenderContext render)
        {
            // TODO: Do not make a copy every frame.
            foreach (var (systemId, (system, contextId)) in _cachedRenderSystems)
            {
                if (system is IGuiSystem monoSystem)
                {
                    if (DIAGNOSTICS_MODE)
                    {
                        _stopwatch.Reset();
                        _stopwatch.Start();
                    }

                    monoSystem.DrawGui(render, Contexts[contextId]);

                    if (DIAGNOSTICS_MODE)
                    {
                        InitializeDiagnosticsCounters();

                        _stopwatch.Stop();
                        GuiCounters[systemId].Update(_stopwatch.Elapsed.TotalMicroseconds, Contexts[contextId].Entities.Length);
                    }
                }
            }
        }
    }
}