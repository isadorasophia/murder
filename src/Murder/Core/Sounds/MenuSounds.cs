namespace Murder.Core.Sounds
{
    public readonly struct MenuSounds
    {
        public readonly SoundEventId MenuSubmit { get; init; }

        public readonly SoundEventId TabSwitchChange { get; init; }
        public readonly SoundEventId SelectionChange { get; init; }
        public readonly SoundEventId Cancel { get; init; }

        public readonly SoundEventId OnError { get; init; }
    }
}