using Bang.Components;
using Murder.Core.Cutscenes;
using System.Collections.Immutable;

namespace Murder.Components.Cutscenes
{
    /// <summary>
    /// This is a list of anchor points of cutscene.
    /// </summary>
    public readonly struct CutsceneAnchorsComponent : IComponent
    {
        public readonly ImmutableDictionary<string, Anchor> Anchors = 
            ImmutableDictionary<string, Anchor>.Empty;

        public CutsceneAnchorsComponent() { }
    }
}
