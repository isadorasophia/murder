using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Assets.Localization;

public class LocalizationAsset : GameAsset
{
    public override char Icon => '\uf891';

    public override string EditorFolder => "#\uf891Localization";

    public override Vector4 EditorColor => "#34ebcf".ToVector4Color();

    [Bang.Serialize]
    private ImmutableArray<LocalizedStringData> _resources = ImmutableArray<LocalizedStringData>.Empty;

    [Bang.Serialize]
    private ImmutableArray<ResourceDataForAsset> _dialogueResources = ImmutableArray<ResourceDataForAsset>.Empty;

    /// <summary>
    /// Expose all the dialogue resources (for editor, etc.).
    /// </summary>
    public ImmutableArray<ResourceDataForAsset> DialogueResources => _dialogueResources;

    /// <summary>
    /// Expose all the resources (for editor, etc.).
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

    public LocalizedString AddResource(string text, bool isGenerated)
    {
        LocalizedStringData data = new LocalizedStringData(
            Guid.NewGuid()) with { String = text, IsGenerated = isGenerated };

        _resources = _resources.Add(data);
        _guidToIndexCache = null;

        return new LocalizedString(data.Guid);
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

    public void UpdateOrSetResource(Guid id, string translated, string? notes)
    {
        LocalizedStringData value;

        if (!GuidToResourceIndex.TryGetValue(id, out int index))
        {
            value = new LocalizedStringData(id) with { String = translated, Notes = notes };
            _resources = _resources.Add(value);

            _guidToIndexCache = null;

            return;
        }

        value = _resources[index] with { String = translated, Notes = string.IsNullOrEmpty(notes) ? null : notes };
        _resources = _resources.SetItem(index, value);
    }

    /// <summary>
    /// Used when setting data from a reference data.
    /// </summary>
    public void SetAllDialogueResources(ImmutableArray<ResourceDataForAsset> resources)
    {
        _dialogueResources = resources;
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

    public void SetResourcesForDialogue(Guid guid, ImmutableArray<Guid> resources)
    {
        for (int i = 0; i < _dialogueResources.Length; ++i)
        {
            ResourceDataForAsset data = _dialogueResources[i];
            if (data.DialogueResourceGuid == guid)
            {
                HashSet<Guid> newResources = resources.ToHashSet();
                foreach (Guid previousResource in data.Resources)
                {
                    if (newResources.Contains(previousResource))
                    {
                        continue;
                    }

                    RemoveResource(previousResource);
                }

                _dialogueResources = _dialogueResources.SetItem(i, data with { Resources = resources });
                return;
            }
        }

        _dialogueResources = _dialogueResources.Add(
            new ResourceDataForAsset() with { DialogueResourceGuid = guid, Resources = resources });
    }

    /// <summary>
    /// Expose all resources tied to a particular dialogue.
    /// </summary>
    public ImmutableArray<Guid> FetchResourcesForDialogue(Guid guid)
    {
        foreach (ResourceDataForAsset data in _dialogueResources)
        {
            if (data.DialogueResourceGuid == guid)
            {
                return data.Resources;
            }
        }

        return ImmutableArray<Guid>.Empty;
    }

    public readonly struct ResourceDataForAsset
    {
        /// <summary>
        /// Which asset originated this resource and its respective strings.
        /// </summary>
        public readonly Guid DialogueResourceGuid { get; init; }

        public readonly ImmutableArray<Guid> Resources { get; init; }
    }
}