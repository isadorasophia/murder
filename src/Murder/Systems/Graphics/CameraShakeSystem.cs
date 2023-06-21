using Bang.Contexts;
using Bang.Systems;
using Murder.Core.Graphics;

namespace Murder.Systems;

public class CameraShakeSystem : IMonoRenderSystem
{
    public CameraShakeSystem()
    {
    }

    public void Draw(RenderContext render, Context context)
    {
        var camera = render.Camera;
        if (camera.ShakeTime > 0)
        {
            camera.ShakeTime -= Game.DeltaTime;
            if (camera.ShakeTime < 0)
            {
                camera.ShakeTime = 0f;
                camera.ShakeIntensity = 0f;
            }
        }
    }
}
