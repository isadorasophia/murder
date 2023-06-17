using Murder.Attributes;

namespace Murder.Core.Sounds
{
    public readonly record struct LocalParameterId
    {
        private const string Empty = "Unknown Name";

        [Tooltip("Parameter owner")]
        public readonly SoundEventId Event = new();

        [Tooltip("Local parameter")]
        public readonly ParameterId Parameter = new();

        public readonly string EditorName
        {
            get
            {
                if (Parameter.Name is null)
                {
                    return string.Empty;
                }

                return $"{Parameter.Name} ({Event.EditorName})";
            }
        }

        public LocalParameterId() { }

        public LocalParameterId(SoundEventId @event, ParameterId parameter)
        {
            Event = @event;
            Parameter = parameter;
        }
    }
}
