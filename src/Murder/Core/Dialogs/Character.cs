using System.Collections.Immutable;

namespace Murder.Core.Dialogs
{
    public readonly struct Character
    {
        /// <summary>
        /// The guid of the character asset being tracked by this.
        /// </summary>
        public readonly Guid Guid;

        /// <summary>
        /// The speaker is the owner of this dialog. Used when a null
        /// speaker is found.
        /// </summary>
        public readonly Guid Speaker;

        /// <summary>
        /// All situations for the character.
        /// </summary>
        public readonly ImmutableDictionary<int, Situation> Situations;

        public Character(Guid guid, Guid speaker, ImmutableArray<Situation> situations)
        {
            Situations = situations.ToDictionary(s => s.Id, s => s).ToImmutableDictionary();

            Speaker = speaker;
            Guid = guid;
        }
    }
}
