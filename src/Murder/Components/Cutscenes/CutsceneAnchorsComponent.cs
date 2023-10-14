using Bang.Components;
using Murder.Attributes;
using Murder.Core.Cutscenes;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components.Cutscenes
{
    /// <summary>
    /// This is a list of anchor points of cutscene.
    /// </summary>
    [RuntimeOnly]
    [PersistOnSave]
    public readonly struct CutsceneAnchorsComponent : IComponent
    {
        public readonly ImmutableDictionary<string, Anchor> Anchors = ImmutableDictionary<string, Anchor>.Empty;

        public CutsceneAnchorsComponent() { }

        public CutsceneAnchorsComponent(ImmutableDictionary<string, Anchor> anchors) =>
            Anchors = anchors;
    }
}