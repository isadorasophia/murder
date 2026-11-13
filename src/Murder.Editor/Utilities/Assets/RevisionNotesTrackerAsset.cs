using Bang;
using Murder.Assets;
using Murder.Attributes;
using Murder.Data;
using Murder.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Road.Editor;

[Flags]
public enum EditorRevisionFlags
{
    None = 0,
    Music = 1 << 0,
    Art = 1 << 1,
    Code = 1 << 2,
    Design = 1 << 3,
    Bug = 1 << 4,
}

public readonly struct RevisionNotes
{
    // A specific entity in a WorldAsset
    public readonly Guid Entity = Guid.Empty;
    public readonly EditorRevisionFlags Flags;
    public readonly string Notes = string.Empty;
    public readonly bool IsAsset = false;

    public RevisionNotes() { }

    public RevisionNotes(EditorRevisionFlags flags, Guid entity, bool isAsset) : this()
    {
        Flags = flags;
        Entity = entity;
        IsAsset = isAsset;
    }
    public RevisionNotes(Guid entity, EditorRevisionFlags flags, string notes, bool isAsset)
    {
        Entity = entity;
        Flags = flags;
        Notes = notes;
        IsAsset = isAsset;
    }

    public bool IsEmpty => Flags == EditorRevisionFlags.None && string.IsNullOrEmpty(Notes);
}

[HideInEditor] // This is created by the engine and should never be actually exposed to the UI.
public class RevisionNotesTrackerAsset : GameAsset
{
    /// <summary>
    /// Use <see cref="GameDataManager.SKIP_CHAR"/> to hide this in the editor.
    /// </summary>
    public override string EditorFolder => GameDataManager.HiddenAssetsRelativePath;

    [Serialize]
    public ImmutableArray<RevisionNotes> Notes = [];

    private Dictionary<Guid, RevisionNotes>? _entitiesCache = null;

    public RevisionNotesTrackerAsset() { }

    public RevisionNotes FetchRevisionFor(Guid guid)
    {
        BuildCache();
        return _entitiesCache.TryGetValue(guid, out var entry) ? entry : default;
    }
    public EditorRevisionFlags FetchFlagsForEntity(Guid guid)
    {
        BuildCache();
        return _entitiesCache.TryGetValue(guid, out var entry) ? entry.Flags : EditorRevisionFlags.None;
    }

    public string FetchTextForEntity(Guid guid)
    {
        BuildCache();
        return _entitiesCache.TryGetValue(guid, out var entry) ? entry.Notes : string.Empty;
    }

    public void RemoveEntity(Guid guid)
    {
        BuildCache();
        _entitiesCache.Remove(guid);
        Notes = [.. _entitiesCache.Values.OrderBy(n => n.Entity)];
    }

    public void SetFlagForEntity(Guid guid, EditorRevisionFlags flags, bool isAsset)
    {
        BuildCache();
        string notes = _entitiesCache.TryGetValue(guid, out var entry) ? entry.Notes : string.Empty;
        UpdateOrInsert(new RevisionNotes(guid, flags, notes, isAsset));
    }

    public void SetTextForEntity(Guid guid, string text, bool isAsset)
    {
        BuildCache();
        EditorRevisionFlags flags = _entitiesCache.TryGetValue(guid, out var entry) ? entry.Flags : EditorRevisionFlags.None;
        string notes = string.IsNullOrWhiteSpace(text) ? string.Empty : text;
        UpdateOrInsert(new RevisionNotes(guid, flags, notes, isAsset));
    }

    private void UpdateOrInsert(RevisionNotes reference)
    {
        BuildCache();

        if (reference.IsEmpty)
        {
            _entitiesCache.Remove(reference.Entity);
        }
        else
        {
            _entitiesCache[reference.Entity] = reference;
        }

        // Sorted so the serialized file doesn't reshuffle on every time.
        Notes = [.. _entitiesCache.Values.OrderBy(n => n.Entity)];
    }


    [MemberNotNull(nameof(_entitiesCache))]
    private void BuildCache()
    {
        if (_entitiesCache is not null)
        {
            return;
        }

        _entitiesCache = [];
        foreach (RevisionNotes note in Notes)
        {
            _entitiesCache[note.Entity] = note;
        }
    }

    protected override void OnModified()
    {
        base.OnModified();

        _entitiesCache = null;
    }

    internal void Clear()
    {
        _entitiesCache = null;
        Notes = [];
    }

    internal bool HasAny()
    {
        return Notes.Length > 0;
    }

}
