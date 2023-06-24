﻿using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Components;
using Murder.Core.Sounds;
using Murder.Services;

namespace Murder.Interactions
{
    public readonly struct PlayMusicInteraction : Interaction
    {
        public readonly SoundEventId Music = new();

        [Tooltip("Whether it should stop playing the last music with fade-out.")]
        public readonly bool StopLastMusic = false;

        public PlayMusicInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            if (StopLastMusic)
            {
                SoundServices.StopAll(fadeOut: true);
            }

            if (world.TryGetUniqueEntity<MusicComponent>() is not Entity e)
            {
                e = world.AddEntity();
            }

            e.SetMusic(Music);
        }
    }
}
