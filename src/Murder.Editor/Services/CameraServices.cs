using Murder.Core;
using Murder.Core.Geometry;
using Murder.Editor.Components;
using Murder.Editor.Utilities;

namespace Murder.Editor.Services
{
    public static class CameraServices
    {
        public static Point GetCursorWorldPosition(MonoWorld world)
        {
            EditorHook hook = world.GetUnique<EditorComponent>().EditorHook;

            return world.Camera.GetCursorWorldPosition(
                hook.Offset, new(hook.StageSize.X, hook.StageSize.Y));
        }
    }
}
