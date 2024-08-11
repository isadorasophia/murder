using Murder.Editor.Assets;
using Murder.Editor.Core;

namespace Murder.Editor;

/// <summary>
/// This is the game loop for a murder editor project. 
/// This may have custom editor events.
/// </summary>
public interface IMurderArchitect : IMurderGame 
{
    /// <summary>
    /// Get all the assets available to load save state from.
    /// </summary>
    public (Guid Guid, string Name)[] GetAllAvailableStartGameFrom() => [];

    /// <summary>
    /// Apply game state informoation before start applying. This assumes that a save is previously loaded
    /// or can be created.
    /// </summary>
    public void OnBeforePlayGame(StartPlayGameInfo state) { }

    public EditorSettingsAsset CreateEditorSettings(string name, string sourcePath) => new(name, sourcePath);
}