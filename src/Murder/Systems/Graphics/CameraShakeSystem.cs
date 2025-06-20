using Bang.Contexts;
using Bang.Systems;
using Murder.Core;
using Murder.Core.Graphics;

namespace Murder.Systems;

[Filter(ContextAccessorFilter.None)]
public class CameraShakeSystem : IMonoPreRenderSystem
{
    public CameraShakeSystem()
    {
    }

    public void BeforeDraw(Context context)
    {
        var camera = ((MonoWorld)context.World).Camera;
        if (camera.ShakeTime > 0)
        {
            camera.ShakeTime -= Game.Instance.LastFrameDuration;
            if (camera.ShakeTime < 0)
            {
                camera.ShakeTime = 0f;
                camera.ShakeIntensity = 0f;
            }

            camera.ClearCache();
        }
    }
}