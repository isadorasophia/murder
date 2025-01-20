using Bang;
using Bang.Components;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Serialization;
using System.Collections.Immutable;

namespace Murder.Assets;

public class CharacterAsset : GameAsset
{
    public override char Icon => '\uf075';
    public override string EditorFolder => "#\uf518Story\\#\uf075Characters";

    [GameAssetId(typeof(SpeakerAsset), allowInheritance: true)]
    public readonly Guid Owner = Guid.Empty;

    /// <summary>
    /// Portrait which will be shown by default from <see cref="Owner"/>.
    /// </summary>
    public readonly string? Portrait = null;

    /// <summary>
    /// Notes regarding this script. This will give context when localizing it.
    /// </summary>
    public readonly string? LocalizationNotes = null;

    /// <summary>
    /// List of tasks or events that the <see cref="Situations"/> may do.
    /// </summary>
    [Serialize]
    private ImmutableArray<Situation> _allSituations = [];

    [Serialize]
    private ImmutableArray<DialogueLineInfo> _dialogueData = [];

    private ImmutableDictionary<string, Situation>? _cachedSituations = null;
    private ImmutableDictionary<DialogueId, LineInfo>? _cachedDialogueData = null;

    /// <summary>
    /// Fetch all situations for this character.
    /// </summary>
    public ImmutableDictionary<string, Situation> Situations
    {
        get
        {
            if (_cachedSituations is null)
            {
                var builder = ImmutableDictionary.CreateBuilder<string, Situation>();
                foreach (Situation situation in _allSituations)
                {
                    builder[situation.Name] = situation;
                }

                _cachedSituations = builder.ToImmutable();
            }

            return _cachedSituations;
        }
    }

    /// <summary>
    /// Fetch all the dialogue custom data.
    /// </summary>
    public ImmutableDictionary<DialogueId, LineInfo> Data
    {
        get
        {
            if (_cachedDialogueData is null)
            {
                var builder = ImmutableDictionary.CreateBuilder<DialogueId, LineInfo>();
                foreach (DialogueLineInfo data in _dialogueData)
                {
                    builder[data.Id] = data.Info;
                }

                _cachedDialogueData = builder.ToImmutable();
            }

            return _cachedDialogueData;
        }
    }

    /// <summary>
    /// Set the situation to a list. 
    /// This is called when updating the scripts with the latest data.
    /// </summary>
    public void SetSituations(ImmutableArray<Situation> situations)
    {
        _allSituations = situations;
        FileChanged = true;
    }

    /// <summary>
    /// Set the situation on <paramref name="situation"/>.
    /// </summary>
    public void SetSituation(Situation situation)
    {
        int index = FindIndexForSituation(situation.Name);
        if (index == -1)
        {
            return;
        }

        _allSituations = _allSituations.SetItem(index, situation);
        FileChanged = true;
    }

    public Situation? TryFetchSituation(string name)
    {
        int index = FindIndexForSituation(name);
        if (index == -1)
        {
            return null;
        }

        return _allSituations[index];
    }

    public void SetCustomComponentAt(DialogueId id, IComponent c)
    {
        int index = GetOrCreateDataAt(id);

        DialogueLineInfo data = _dialogueData[index];
        _dialogueData = _dialogueData.SetItem(
            index,
            data with { Info = data.Info with { Component = c } });

        FileChanged = true;
    }

    public void SetCustomPortraitAt(DialogueId id, Guid speaker, string? portrait)
    {
        int index = GetOrCreateDataAt(id);

        DialogueLineInfo data = _dialogueData[index];
        _dialogueData = _dialogueData.SetItem(
            index, 
            data with { Info = data.Info with { Speaker = speaker, Portrait = portrait } });

        FileChanged = true;
    }

    public void SetEventInfoAt(DialogueId id, string? @event)
    {
        int index = GetOrCreateDataAt(id);

        DialogueLineInfo data = _dialogueData[index];
        _dialogueData = _dialogueData.SetItem(index, data with { Info = data.Info with { Event = @event } });

        FileChanged = true;
    }

    public void PrunUnusedComponents(IEnumerable<DialogueId> unusedComponents)
    {
        foreach (DialogueId id in unusedComponents)
        {
            int index = FindIndexForDialogueId(id);
            if (index == -1)
            {
                continue;
            }

            DialogueLineInfo data = _dialogueData[index];
            _dialogueData = _dialogueData.SetItem(index, data with { Info = data.Info with { Component = null } });
        }

        FileChanged = true;
    }

    public void PrunUnusedData()
    {
        for (int i = 0; i < _dialogueData.Length; i++)
        {
            DialogueLineInfo data = _dialogueData[i];
            if (data.Info.Empty)
            {
                _dialogueData = _dialogueData.RemoveAt(i);
                i--; // adjust index which will be added in the next iteration
            }
        }

        FileChanged = true;
    }

    protected override void OnModified()
    {
        _cachedSituations = null;
        _cachedDialogueData = null;
    }

    private int GetOrCreateDataAt(DialogueId id)
    {
        int index = FindIndexForDialogueId(id);
        if (index == -1)
        {
            index = _dialogueData.Length;
            _dialogueData = _dialogueData.Add(new(id));
        }

        return index;
    }

    private int FindIndexForDialogueId(DialogueId id)
    {
        for (int i = 0; i < _dialogueData.Length; ++i)
        {
            DialogueLineInfo info = _dialogueData[i];
            if (info.Id == id)
            {
                return i;
            }
        }

        return -1;
    }

    private int FindIndexForSituation(string name)
    {
        for (int i = 0; i < _allSituations.Length; ++i)
        {
            Situation target = _allSituations[i];
            if (name.Equals(target.Name))
            {
                return i;
            }
        }

        return -1;
    }
}