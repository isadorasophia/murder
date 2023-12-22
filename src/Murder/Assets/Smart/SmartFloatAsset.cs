using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Assets;

public class SmartFloatAsset : GameAsset
{
    public override char Icon => '';
    public override string EditorFolder => "#Smart";

    public override Vector4 EditorColor => new Vector4(1f, 0.5f, 0.7f, 1f);

    public ImmutableArray<float> Values = ImmutableArray<float>.Empty;
    public ImmutableArray<string> Titles = ImmutableArray<string>.Empty;
}