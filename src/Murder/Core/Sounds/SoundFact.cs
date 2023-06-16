namespace Murder.Core.Sounds
{
    public readonly record struct SoundFact : IComparable<SoundFact>
    {
        /// <summary>
        /// If null, grab the default blackboard.
        /// </summary>
        public readonly string? Blackboard = null;

        public readonly string Name = string.Empty;

        public SoundFact() { }

        public SoundFact(string? blackboard, string name)
        {
            Blackboard = blackboard;
            Name = name;
        }

        public int CompareTo(SoundFact other) => Name.CompareTo(other.Name);
    }
}
