using Murder.Components.Utilities;

namespace Murder.Core
{
    public readonly struct Tags
    {
        public readonly bool All = true;
        public readonly int Flags;
        public Tags()
        {
        }

        public Tags(int flags) : this()
        {
            All = false;
            Flags = flags;
        }
        public Tags(int flags, bool all) : this()
        {
            All = all;
            Flags = flags;
        }

        public bool HasTag(int tag)
        {
            return All || ((tag & Flags) != 0);
        }

        public bool HasTags(Tags tags)
        {
            return HasTag(tags.Flags);
        }

        internal bool HasTags(TagsComponent? tagsComponent)
        {
            if (tagsComponent == null)
            {
                return false;
            }
            else
            {
                return HasTags(tagsComponent.Value.Tags);
            }
        }
    }
}
