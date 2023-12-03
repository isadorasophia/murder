using Murder.Utilities;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Assets.Localization;

public class LocalizationAsset : GameAsset
{
    public override char Icon => '\uf891';
    public override string EditorFolder => "#\uf891Localization";
    public override Vector4 EditorColor => "#34ebcf".ToVector4Color();

    [JsonProperty]
    private ImmutableDictionary<Guid, LocalizedStringData> _resources = ImmutableDictionary<Guid, LocalizedStringData>.Empty;

    public ImmutableDictionary<Guid, LocalizedStringData> Resources => _resources;

    public void AddResource(Guid id)
    {
        _resources = _resources.Add(id, new());
    }

    public void SetResource(Guid id, LocalizedStringData value)
    {
        _resources = _resources.SetItem(id, value);
    }
}
