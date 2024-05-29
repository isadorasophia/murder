using Bang;
using Bang.Diagnostics;
using Bang.Systems;
using Murder.Core.Graphics;

namespace Murder.Core
{
    /// <summary>
    /// World implementation based in MonoGame.
    /// </summary>
    public partial class MonoWorld
    {
        /// <summary>
        /// This has the duration of each reactive system (id) to its corresponding time (in ms).
        /// See <see cref="World.IdToSystem"/> on how to fetch the actual system.
        /// </summary>
        public readonly Dictionary<int, SmoothCounter> PreRenderCounters = new();

        /// <summary>
        /// This has the duration of each render system (id) to its corresponding time (in ms).
        /// See <see cref="World.IdToSystem"/> on how to fetch the actual system.
        /// </summary>
        public readonly Dictionary<int, SmoothCounter> RenderCounters = new();

        /// <summary>
        /// This has the duration of each gui render system (id) to its corresponding time (in ms).
        /// See <see cref="World.IdToSystem"/> on how to fetch the actual system.
        /// </summary>
        public readonly Dictionary<int, SmoothCounter> GuiCounters = new();

        protected override void ClearDiagnosticsCountersForSystem(int id)
        {
            if (RenderCounters.TryGetValue(id, out var value)) value.Clear();
            if (GuiCounters.TryGetValue(id, out value)) value.Clear();
            if (PreRenderCounters.TryGetValue(id, out value)) value.Clear();
        }

        protected override void InitializeDiagnosticsForSystem(int systemId, ISystem system)
        {
            if (system is IMurderRenderSystem)
            {
                RenderCounters[systemId] = new();
            }

            if (system is IGuiSystem)
            {
                GuiCounters[systemId] = new();
            }

            if (system is IMonoPreRenderSystem)
            {
                PreRenderCounters[systemId] = new();
            }
        }
    }
}