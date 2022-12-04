using Bang.Components;
using Murder.Core.Cutscenes;
using Murder.Core.Geometry;
using System.Collections.Immutable;

namespace Murder.Components.Cutscenes
{
    /// <summary>
    /// This is a list of anchor points of cutscene.
    /// </summary>
    public readonly struct CutsceneAnchorsComponent : IComponent
    {
        public readonly ImmutableDictionary<string, Anchor> Anchors = ImmutableDictionary<string, Anchor>.Empty;

        public CutsceneAnchorsComponent() { }

        public CutsceneAnchorsComponent(ImmutableDictionary<string, Anchor> anchors) =>
            Anchors = anchors;
    
        public CutsceneAnchorsComponent WithAnchorAt(string name, Vector2 newPosition)
        {
            ImmutableDictionary<string, Anchor> anchors = Anchors.SetItem(name, Anchors[name].WithPosition(newPosition));
            
            return new(anchors);
        }

        public CutsceneAnchorsComponent WithoutAnchorAt(string name)
        {
            ImmutableDictionary<string, Anchor> anchors = Anchors.Remove(name);

            return new(anchors);
        }

        public CutsceneAnchorsComponent AddAnchorAt(Vector2 newPosition)
        {
            ImmutableDictionary<string, Anchor> anchors = Anchors.Add(Anchors.Count.ToString(), new Anchor(newPosition));

            return new(anchors);
        }
    }
}
