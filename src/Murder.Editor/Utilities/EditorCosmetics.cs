using Murder.Assets;

namespace Murder.Editor;

public static class EditorCosmetics
{
    private static EditorCosmeticsAsset? _editorProperties = null;

    public static EditorCosmeticsAsset? TryGetEditorCosmetic()
    {
        if (Game.Data.LoadContentProgress is not null && !Game.Data.LoadContentProgress.IsCompleted)
        {
            return null;
        }

        if (_editorProperties is null)
        {
            foreach ((Guid g, GameAsset a) in Game.Data.FilterAllAssets(typeof(EditorCosmeticsAsset)))
            {
                _editorProperties = (EditorCosmeticsAsset)a;
            }
        }

        if (_editorProperties is EditorCosmeticsAsset properties)
        {
            return properties;
        }

        // Otherwise, this means we need to actually create one...
        EditorCosmeticsAsset instance = new();
        instance.Name = "_EditorCosmetics";
        instance.MakeGuid();

        Architect.EditorData.SaveAsset(instance);

        _editorProperties = instance;
        return _editorProperties;
    }

    public static void Play(string @event)
    {
        if (!Architect.Instance.IsPlayingGame)
        {
            // only play those in f2
            return;
        }

        if (Architect.EditorSettings.MuteEditorSounds)
        {
            // nope!
            return;
        }

        EditorCosmeticsAsset? cosmetics = TryGetEditorCosmetic();
        cosmetics?.PlayEvent(@event);
    }
}
