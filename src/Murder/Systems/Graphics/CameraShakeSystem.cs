using Bang.Contexts;
using Bang.Systems;
using Murder.Core;
using Murder.Core.Graphics;

namespace Murder.Systems;

[DoNotPause]
[Filter(ContextAccessorFilter.None)]
public class CameraShakeSystem : IUpdateSystem
{
    public CameraShakeSystem() { }

    public void Update(Context context)
    {
        Camera2D camera = ((MonoWorld)context.World).Camera;
        if (camera.ShakeTime > 0)
        {
            camera.ShakeTime -= Game.UnscaledDeltaTime;
            if (camera.ShakeTime < 0)
            {
                camera.ShakeTime = 0f;
                camera.ShakeIntensity = 0f;
            }

            camera.ClearCache();
        }
    }
}