using Bang;
using Bang.Diagnostics;
using Murder.Assets;
using Murder.Attributes;
using Murder.Data;
using Murder.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Murder.Editor.Assets;

[Flags]
public enum EditorRevisionFlags
{
    None = 0,
    Music = 1 << 0,
    Art = 1 << 1,
    Code = 1 << 2,
    Design = 1 << 3,
    Bug = 1 << 4
}

public readonly struct RevisionNotes
{
    /// <summary>
    /// Instance reference for the notes.
    /// </summary>
    public readonly Guid Guid = Guid.Empty;

    public EditorRevisionFlags Flags { get; init; }
    public string Notes { get; init; } = string.Empty;

    public RevisionNotes(Guid guid)
    {
        Guid = guid;
    }

    public bool Empty => Flags == EditorRevisionFlags.None && string.IsNullOrEmpty(Notes);
}

public class WorldRevisionNotes
{
    // A specific entity in a WorldAsset
    [GameAssetId<GameAsset>]
    public readonly Guid Guid = Guid.Empty;

    [Serialize]
    private readonly List<RevisionNotes> _notes = [];

    private Dictionary<Guid, RevisionNotes>? _notesCache = null;

    [JsonConstructor]
    public WorldRevisionNotes(Guid guid)
    {
        Guid = guid;
    }

    public void ModifyNotes(Guid instanceGuid, RevisionNotes notes)
    {
        ClearCache();

        for (int i = 0; i < _notes.Count; ++i)
        {
            if (_notes[i].Guid == instanceGuid)
            {
                // found it!
                if (notes.Empty)
                {
                    _notes.RemoveAt(i);
                }
                else
                {
                    _notes[i] = notes;
                }

                return;
            }
        }

        _notes.Add(notes);
    }

    public RevisionNotes? TryGetNotesForInstance(Guid guid)
    {
        if (_notesCache is null)
        {
            BuildCache();
        }

        if (!_notesCache.TryGetValue(guid, out RevisionNotes notes))
        {
            return null;
        }

        return notes;
    }

    [MemberNotNull(nameof(_notesCache))]
    private void BuildCache()
    {
        _notesCache = [];

        foreach (RevisionNotes note in _notes)
        {
            _notesCache[note.Guid] = note;
        }
    }

    public void ClearMissingReferences(WorldAsset world)
    {
        for (int i = 0; i < _notes.Count; ++i)
        {
            Guid instanceGuid = _notes[i].Guid;

            if (world.TryGetInstance(instanceGuid) is null || _notes[i].Empty)
            {
                GameLogger.Log($"\uf00c Clearing notes for {world.Guid} at {_notes[i].Guid}.");

                _notes.RemoveAt(i);
                i--;

                continue;
            }
        }

        ClearCache();
    }

    private void ClearCache()
    {
        _notesCache = null;
    }

    public bool Empty => _notes.Count == 0;

    public int Length => _notes.Count;
}

[HideInEditor] // This is created by the engine and should never be actually exposed to the UI.
public class RevisionNotesTrackerAsset : GameAsset
{
    /// <summary>
    /// Use <see cref="GameDataManager.SKIP_CHAR"/> to hide this in the editor.
    /// </summary>
    public override string EditorFolder => GameDataManager.HiddenAssetsRelativePath;

    [Serialize]
    public ImmutableArray<WorldRevisionNotes> WorldNotes { get; private set; } = [];

    private Dictionary<Guid, WorldRevisionNotes>? _worldNotesCache = null;

    public RevisionNotesTrackerAsset() { }

    public WorldRevisionNotes GetOrCreateRevisionNotesForOwner(Guid /* WorldAsset */ guid)
    {
        if (TryGetRevisionNotesForOwner(guid) is WorldRevisionNotes entry)
        {
            return entry;
        }

        entry = new WorldRevisionNotes(guid);

        WorldNotes = WorldNotes.Add(entry);
        ClearCache();

        return entry;
    }

    public WorldRevisionNotes? TryGetRevisionNotesForOwner(Guid /* WorldAsset */ guid)
    {
        if (_worldNotesCache is null)
        {
            BuildCache();
        }

        if (!_worldNotesCache.TryGetValue(guid, out WorldRevisionNotes? entry))
        {
            return null;
        }

        return entry;
    }

    public RevisionNotes? TryGetRevisionNotesForInstance(Guid ownerGuid, Guid instanceGuid)
    {
        return TryGetRevisionNotesForOwner(ownerGuid)?.TryGetNotesForInstance(instanceGuid);
    }

    public void ClearMissingReferences()
    {
        bool modified = false;
        for (int i = 0; i < WorldNotes.Length; ++i)
        {
            Guid worldGuid = WorldNotes[i].Guid;

            bool removeWorldNotes = true;

            if (Game.Data.TryGetAsset<WorldAsset>(worldGuid) is WorldAsset worldAsset)
            {
                WorldNotes[i].ClearMissingReferences(worldAsset);
                removeWorldNotes = WorldNotes[i].Empty;
            }

            if (removeWorldNotes)
            {
                modified = true;
                GameLogger.Log($"\uf00c Clearing notes for {worldGuid}.");

                WorldNotes = WorldNotes.RemoveAt(i);
                i--;

                continue;
            }
        }

        if (modified)
        {
            ClearCache();
        }

        GameLogger.Log($"\uf00c Successfully checked for revision notes!");
    }

    [MemberNotNull(nameof(_worldNotesCache))]
    private void BuildCache()
    {
        _worldNotesCache = [];

        foreach (WorldRevisionNotes note in WorldNotes)
        {
            _worldNotesCache[note.Guid] = note;
        }
    }

    protected override void OnModified()
    {
        base.OnModified();

        ClearCache();
        ClearMissingReferences();
    }

    private void ClearCache()
    {
        _worldNotesCache = null;
    }
}
