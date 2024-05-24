using Murder.Editor.Stages;

namespace Murder.Editor.CustomEditors;

internal partial class WorldAssetEditor
{
    protected virtual bool DrawPathfindEditor(Stage stage)
    {
        if (_world is null)
        {
            return false;
        }

        stage.EditorHook.DrawGrid = true;

        bool modified = false;

        return modified;
    }
}