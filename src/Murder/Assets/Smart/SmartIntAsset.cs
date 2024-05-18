using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Assets;

public class SmartIntAsset : GameAsset
{
    public override char Icon => '';
    public override string EditorFolder => "#Smart";

    public override Vector4 EditorColor => new Vector4(1f, 0.7f, 0.5f, 1f);

    public ImmutableArray<int> Values = ImmutableArray<int>.Empty;
    public ImmutableArray<string> Titles = ImmutableArray<string>.Empty;
}
