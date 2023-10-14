﻿using Bang.Components;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Sounds;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// Sound component which will be immediately played and destroyed.
    /// </summary>
    [Sound]
    public readonly struct SoundComponent : IComponent
    {
        public readonly SoundEventId? Sound = default;

        public readonly bool DestroyEntity = false;

        public SoundComponent() { }

        public SoundComponent(SoundEventId sound, bool destroyEntity)
        {
            Sound = sound;
            DestroyEntity = destroyEntity;
        }
    }
}