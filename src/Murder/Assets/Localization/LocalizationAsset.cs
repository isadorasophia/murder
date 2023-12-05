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
    private ImmutableArray<LocalizedStringData> _resources = ImmutableArray<LocalizedStringData>.Empty;

    /// <summary>
    /// Expose all the resources (for editor, etc.)
    /// </summary>
    public ImmutableArray<LocalizedStringData> Resources => _resources;

    private ImmutableDictionary<Guid, int>? _guidToIndexCache = null;

    private ImmutableDictionary<Guid, int> GuidToResourceIndex
    {
        get
        {
            if (_guidToIndexCache is not null)
            {
                return _guidToIndexCache;
            }

            var builder = ImmutableDictionary.CreateBuilder<Guid, int>();
            for (int i = 0; i < _resources.Length; i++) 
            {
                builder.Add(_resources[i].Guid, i);
            }

            _guidToIndexCache = builder.ToImmutable();
            return _guidToIndexCache;
        }
    }

    public void AddResource(Guid id)
    {
        if (!GuidToResourceIndex.TryGetValue(id, out int index))
        {
            _resources = _resources.Add(new(id));
            _guidToIndexCache = null;

            return;
        }

        LocalizedStringData data = _resources[index];
        int counter = data.Counter is null ? 2 : data.Counter.Value + 1;

        _resources = _resources.SetItem(index, data with { Counter = counter });
    }

    public void RemoveResource(Guid id, bool force = false)
    {
        if (!GuidToResourceIndex.TryGetValue(id, out int index))
        {
            return;
        }

        LocalizedStringData data = _resources[index];
        if (force || data.Counter is null)
        {
            _resources = _resources.RemoveAt(index);
            _guidToIndexCache = null;

            return;
        }

        int? counter = data.Counter.Value == 2 ? null : data.Counter.Value - 1;
        _resources = _resources.SetItem(index, data with { Counter = counter });
    }

    public void SetResource(LocalizedStringData value)
    {
        if (!GuidToResourceIndex.TryGetValue(value.Guid, out int index))
        {
            _resources = _resources.Add(value);
            _guidToIndexCache = null;

            return;
        }

        _resources = _resources.SetItem(index, value);
    }

    public LocalizedStringData? TryGetResource(Guid id)
    {
        if (!GuidToResourceIndex.TryGetValue(id, out int index))
        {
            return null;
        }

        return _resources[index];
    }

    public bool HasResource(Guid id) => GuidToResourceIndex.ContainsKey(id);
}