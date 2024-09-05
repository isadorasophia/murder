using Bang;
using Bang.Components;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Dialogs;
using Murder.Diagnostics;
using Murder.Serialization;
using System.Collections.Immutable;

namespace Murder.Assets;

public readonly struct LineInfo
{
    public readonly Guid Speaker { get; init; } = Guid.Empty;
    public readonly string? Portrait { get; init; } = null;

    public string? Event { get; init; } = null;

    /// <summary>
    /// Component modified within a dialog.
    /// </summary>
    public IComponent? Component { get; init; } = null;

    public LineInfo() { }

    public bool Empty => Speaker == Guid.Empty && Portrait is null && Event is null && Component is null;
}

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
    private ImmutableDictionary<string, Situation> _situations = ImmutableDictionary<string, Situation>.Empty;

    /// <summary>
    /// List of all the lines that have custom information within a dialogue.
    /// </summary>
    [Serialize]
    private readonly ComplexDictionary<DialogueId, LineInfo> _data = [];

    public ImmutableDictionary<string, Situation> Situations => _situations;

    /// <summary>
    /// Set the situation to a list. 
    /// This is called when updating the scripts with the latest data.
    /// </summary>
    public void SetSituations(ImmutableDictionary<string, Situation> situations)
    {
        _situations = situations;
        FileChanged = true;
    }

    /// <summary>
    /// Set the situation on <paramref name="situation"/>.
    /// </summary>
    public void SetSituation(Situation situation)
    {
        _situations = _situations.SetItem(situation.Name, situation);
        FileChanged = true;
    }

    public Situation? TryFetchSituation(string name)
    {
        if (_situations.TryGetValue(name, out Situation value))
        {
            return value;
        }

        return null;
    }

    public void SetCustomComponentAt(DialogueId id, IComponent c)
    {
        InitializeDataAt(id);

        _data[id] = _data[id] with { Component = c };
        FileChanged = true;
    }

    public void SetCustomPortraitAt(DialogueId id, Guid speaker, string? portrait)
    {
        InitializeDataAt(id);

        _data[id] = _data[id] with { Speaker = speaker, Portrait = portrait };
        FileChanged = true;
    }

    public void SetEventInfoAt(DialogueId id, string? @event)
    {
        InitializeDataAt(id);

        _data[id] = _data[id] with { Event = @event };
        FileChanged = true;
    }

    public void PrunUnusedComponents(IEnumerable<DialogueId> unusedComponents)
    {
        foreach (DialogueId id in unusedComponents)
        {
            InitializeDataAt(id);

            _data[id] = _data[id] with { Component = null };
        }

        FileChanged = true;
    }

    private void InitializeDataAt(DialogueId id)
    {
        if (!_data.ContainsKey(id))
        {
            _data[id] = new();
        }
    }

    public void PrunUnusedData()
    {
        foreach ((DialogueId id, LineInfo info) in Data)
        {
            if (info.Empty)
            {
                _data.Remove(id);
            }
        }

        FileChanged = true;
    }

    public void RemoveCustomPortraits(IEnumerable<DialogueId> actionIds)
    {
        foreach (DialogueId id in actionIds)
        {
            InitializeDataAt(id);

            _data[id] = _data[id] with { Speaker = Guid.Empty, Portrait = null };
        }

        FileChanged = true;
    }

    public ImmutableDictionary<DialogueId, LineInfo> Data => _data.ToImmutableDictionary();
}