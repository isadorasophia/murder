using Murder.Diagnostics;
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
#if DEBUG
                if (builder.TryGetValue(_resources[i].Guid, out int value))
                {
                    GameLogger.Warning($"Replacing {_resources[i].Guid} of {_resources[value].String} with {_resources[i].String}");
                }
#endif

                builder[_resources[i].Guid] = i;
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

    public bool UpdateOrSetResource(Guid id, string translated, string? notes)
    {
        LocalizedStringData value;

        LocalizationAsset @default = Game.Data.GetDefaultLocalization();
        if (@default.TryGetResource(id) is not LocalizedStringData defaultData)
        {
            return false;
        }

        if (string.IsNullOrEmpty(translated))
        {
            translated = defaultData.String;
        }

        if (!GuidToResourceIndex.TryGetValue(id, out int index))
        {
            value = new LocalizedStringData(id) with { String = translated, Notes = notes };
            _resources = _resources.Add(value);

            _guidToIndexCache = null;

            return true;
        }

        if (!string.IsNullOrEmpty(notes) && @default.Guid != Guid)
        {
            @default.UpdateOrSetNotes(id, notes);
        }

        value = _resources[index] with { String = translated, Notes = string.IsNullOrEmpty(notes) ? null : notes };
        _resources = _resources.SetItem(index, value);
        return true;
    }

    public bool UpdateOrSetNotes(Guid id, string? notes)
    {
        if (!GuidToResourceIndex.TryGetValue(id, out int index))
        {
            return false;
        }

        LocalizedStringData value = _resources[index] with { Notes = notes };
        _resources = _resources.SetItem(index, value);

        return true;
    }

    /// <summary>
    /// Used when setting data from a reference data.
    /// </summary>
    public void SetAllResources(ImmutableArray<LocalizedStringData> resources)
    {
        _resources = resources;
        _guidToIndexCache = null;
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

        if (index >= _resources.Length)
        {
            _guidToIndexCache = null;
            return null;
        }

        return _resources[index];
    }

    public bool HasResource(Guid id) => GuidToResourceIndex.ContainsKey(id);

    public void RemoveResourceForDialogue(Guid dialogueAssetId)
    {
        var builder = ImmutableArray.CreateBuilder<ResourceDataForAsset>();
        builder.AddRange(_dialogueResources);

        for (int i = 0; i < builder.Count; ++i)
        {
            ResourceDataForAsset data = builder[i];
            if (data.DialogueResourceGuid == dialogueAssetId)
            {
                foreach (LocalizedDialogueData r in data.DataResources)
                {
                    RemoveResource(r.Guid);
                }

                builder.RemoveAt(i);
                i--;
            }
        }

        _dialogueResources = builder.ToImmutable();
    }

    public void SetResourcesForDialogue(Guid guid, ImmutableArray<LocalizedDialogueData> resources)
    {
        for (int i = 0; i < _dialogueResources.Length; ++i)
        {
            ResourceDataForAsset data = _dialogueResources[i];
            if (data.DialogueResourceGuid == guid)
            {
                HashSet<LocalizedDialogueData> newResources = [.. resources];
                foreach (LocalizedDialogueData previousResource in data.DataResources)
                {
                    if (newResources.Contains(previousResource))
                    {
                        continue;
                    }

                    RemoveResource(previousResource.Guid);
                }

                _dialogueResources = _dialogueResources.SetItem(i, data with { DataResources = resources });
                return;
            }
        }

        _dialogueResources = _dialogueResources.Add(
            new ResourceDataForAsset() with { DialogueResourceGuid = guid, DataResources = resources });
    }

    /// <summary>
    /// Expose all resources tied to a particular dialogue.
    /// </summary>
    public ImmutableArray<LocalizedDialogueData> FetchResourcesForDialogue(Guid guid)
    {
        foreach (ResourceDataForAsset data in _dialogueResources)
        {
            if (data.DialogueResourceGuid == guid)
            {
                return data.DataResources;
            }
        }

        return [];
    }

    public readonly struct ResourceDataForAsset
    {
        /// <summary>
        /// Which asset originated this resource and its respective strings.
        /// </summary>
        public readonly Guid DialogueResourceGuid { get; init; } = Guid.Empty;

        public readonly ImmutableArray<LocalizedDialogueData> DataResources { get; init; } = [];

        public ResourceDataForAsset() { }
    }
}

public readonly struct LocalizedDialogueData
{
    /// <summary>
    /// Guid for the string itself.
    /// </summary>
    public readonly Guid Guid;

    /// <summary>
    /// Guid for the speaker.
    /// </summary>
    public readonly Guid Speaker;

    public LocalizedDialogueData(Guid speaker, Guid guid) => (Guid, Speaker) = (guid, speaker);

    public override int GetHashCode()
    {
        return Guid.GetHashCode();
    }

    public override bool Equals(object? d)
    {
        if (d is not LocalizedDialogueData data)
        {
            return false;
        }

        return Guid == data.Guid;
    }
}