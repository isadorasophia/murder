using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    [Unique, DoNotPersistOnSave, RuntimeOnly]
    public readonly struct FreezeWorldComponent : IComponent
    {
        public readonly int Count = 1;
        public readonly float StartTime;

        public readonly bool ShowUi = false;

        public FreezeWorldComponent(float startTime)
        {
            StartTime = startTime;
        }

        public FreezeWorldComponent(float startTime, int count) : this(startTime)
        {
            Count = count;
        }

        public FreezeWorldComponent(float startTime, bool show) : this(startTime)
        {
            ShowUi = show;
        }

        public FreezeWorldComponent(float startTime, int count, bool show) : this(startTime, count)
        {
            ShowUi = show;
        }
    }
}