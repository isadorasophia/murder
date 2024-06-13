using Murder.Components;
using System.Collections.Immutable;

namespace Murder.Utilities
{
    public static class WorldHelper
    {
        public static EventListenerComponent ToEventListener(this EventListenerEditorComponent c)
        {
            return new(ToEventListener(c.Events));
        }

        public static ImmutableDictionary<string, SpriteEventInfo> ToEventListener(ImmutableArray<SpriteEventInfo> messages)
        {
            var builder = ImmutableDictionary.CreateBuilder<string, SpriteEventInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (SpriteEventInfo info in messages)
            {
                builder[info.Id] = info;
            }

            return builder.ToImmutable();
        }
    }
}