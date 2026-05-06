using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    [Flags]
    public enum FreezeWorldFlags
    {
        None = 0,
        AllowInputMessages = 1,
        ShowUi = 1 << 1
    }

    [Unique, DoNotPersistOnSave, RuntimeOnly]
    public readonly struct FreezeWorldComponent : IComponent
    {
        public readonly int Count = 1;
        public readonly float StartTime;

        public readonly FreezeWorldFlags Flags = FreezeWorldFlags.None;

        public readonly bool ShowUi => Flags.HasFlag(FreezeWorldFlags.ShowUi);
        
        public FreezeWorldComponent(float startTime)
        {
            StartTime = startTime;
        }

        public FreezeWorldComponent(float startTime, int count) : this(startTime)
        {
            Count = count;
        }

        public FreezeWorldComponent(float startTime, FreezeWorldFlags flags) : this(startTime)
        {
            Flags = flags;
        }

        public FreezeWorldComponent(float startTime, int count, FreezeWorldFlags flags) : this(startTime, count)
        {
            Flags = flags;
        }
    }
}