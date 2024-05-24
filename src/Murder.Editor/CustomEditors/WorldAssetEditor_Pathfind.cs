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

        bool modified = false;

        return modified;
    }
}