using Murder.Components;
using System.Collections.Immutable;

namespace Murder.Utilities
{
    public static class WorldHelper
    {
        public static EventListenerComponent ToEventListener(this EventListenerEditorComponent c)
        {
            var builder = ImmutableDictionary.CreateBuilder<string, SpriteEventInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (SpriteEventInfo info in c.Events)
            {
                builder[info.Id] = info;
            }

            return new(builder.ToImmutable());
        }
    }
}