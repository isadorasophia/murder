using Bang.Contexts;
using Bang.Entities;
using Murder.Editor.Attributes;
using Murder.Editor.Utilities;

namespace Murder.Editor.Systems;

[SoundEditor]
public class SoundShapeWorldEditorSystem : SoundShapeEditorSystem
{
    private bool _hasAnySelected;

    protected override void ProcessEntity(EditorHook hook, Entity e)
    {
        // Check if we are in edit mode and if the entity is selected
        if (hook.EditorMode == EditorHook.EditorModes.EditMode && hook.AllSelectedEntities.ContainsKey(e.EntityId))
        {
            _hasAnySelected = true;
        }
    }

    protected override IEnumerable<Entity> GetEligibleEntities(Context context, EditorHook hook) => hook.AllSelectedEntities.Values;

    protected override void OnToggledMode(bool mode)
    {
        base.OnToggledMode(mode);

        _hasAnySelected = false;
    }

    protected override bool IsAnySelected => _hasAnySelected;

    protected override bool IsSelected(EditorHook hook, Entity e) => hook.AllSelectedEntities.ContainsKey(e.EntityId);

    /// <summary>
    /// Always enable on world.
    /// </summary>
    protected override bool IsEnabled(Context context, EditorHook hook) => true;
}
