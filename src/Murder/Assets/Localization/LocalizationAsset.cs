using Murder.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

    public void RemoveResource(Guid id)
    {
        if (!_resources.TryGetValue(id, out LocalizedStringData data))
        {
            return;
        }

        if (data.Counter == 1)
        {
            _resources = _resources.Remove(id);
            return;
        }

        _resources = _resources.SetItem(id, data with { Counter = data.Counter - 1 });
    }

    public void SetResource(Guid id, LocalizedStringData value)
    {
        _resources = _resources.SetItem(id, value);
    }

    public LocalizedStringData? TryGetResource(Guid id)
    {
        if (!_resources.TryGetValue(id, out LocalizedStringData data))
        {
            return null;
        }

        return data;
    }
}