using Bang.Components;
using Murder.Core.Cutscenes;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Components.Serialization
{
    public readonly struct AnchorId
    {
        public readonly string Id = string.Empty;
        public readonly Anchor Anchor = default;

        public AnchorId() { }

        public AnchorId(string id, Anchor anchor) => (Id, Anchor) = (id, anchor);
    }

    public readonly struct CutsceneAnchorsEditorComponent : IComponent
    {
        public readonly ImmutableArray<AnchorId> Anchors = ImmutableArray<AnchorId>.Empty;

        public CutsceneAnchorsEditorComponent() { }

        public CutsceneAnchorsEditorComponent(ImmutableArray<AnchorId> anchors) =>
            Anchors = anchors;

        public CutsceneAnchorsEditorComponent WithAnchorAt(string name, Vector2 newPosition)
        {
            int index = FindIndexOf(name);
            if (index == -1)
            {
                return this;
            }

            ImmutableArray<AnchorId> anchors = Anchors.SetItem(
                index, new(name, Anchors[index].Anchor.WithPosition(newPosition)));

            return new(anchors);
        }

        public CutsceneAnchorsEditorComponent WithoutAnchorAt(string name)
        {
            int index = FindIndexOf(name);
            if (index == -1)
            {
                return this;
            }

            return new(Anchors.RemoveAt(index));
        }

        public CutsceneAnchorsEditorComponent AddAnchorAt(Vector2 newPosition)
        {
            ImmutableArray<AnchorId> anchors = Anchors.Add(new(Anchors.Length.ToString(), new Anchor(newPosition)));
            return new(anchors);
        }

        public Anchor FindAnchor(string name)
        {
            for (int i = 0; i < name.Length; ++i)
            {
                AnchorId anchor = Anchors[i];
                if (anchor.Id == name)
                {
                    return anchor.Anchor;
                }
            }

            return default;
        }

        private int FindIndexOf(string name)
        {
            for (int i = 0; i < Anchors.Length; ++i)
            {
                AnchorId anchor = Anchors[i];
                if (anchor.Id.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
