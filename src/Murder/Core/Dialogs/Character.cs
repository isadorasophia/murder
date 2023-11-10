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
        /// The default portrait for <see cref="Speaker"/>. If null, use the speaker
        /// default portrait.
        /// </summary>
        public readonly string? Portrait;

        /// <summary>
        /// All situations for the character.
        /// </summary>
        public readonly ImmutableDictionary<int, Situation> Situations;

        public Character(Guid guid, Guid speaker, string? portrait, ImmutableArray<Situation> situations)
        {
            Situations = situations.ToDictionary(s => s.Id, s => s).ToImmutableDictionary();

            Speaker = speaker;
            Portrait = portrait;

            Guid = guid;
        }
    }
}