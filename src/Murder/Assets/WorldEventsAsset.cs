using Murder.Attributes;
using Murder.Core;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Assets;

public class WorldEventsAsset : GameAsset
{
    public override char Icon => '\uf7c0';
    public override string EditorFolder => "#\uf7c0Global Events";

    public override Vector4 EditorColor => "#34ebcf".ToVector4Color();

    [Tooltip("Events that will be placed on all maps (or the specified one)")]
    public ImmutableArray<TriggerEventOn> Watchers = [];
}