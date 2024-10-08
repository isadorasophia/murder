using Bang;
using Murder.Core;
using Murder.Utilities;

namespace Murder.Services;

public static class CameraServices
{
    public static void ShakeCamera(World world, float intensity, float time)
    {
        var w = (MonoWorld)world;
        if (w.Camera.ShakeIntensity > intensity)
        {
            w.Camera.ShakeIntensity = Calculator.Lerp(w.Camera.ShakeIntensity, intensity, 0.5f);
        }
        else
        {
            w.Camera.ShakeIntensity = intensity;
        }

        w.Camera.ShakeTime = MathF.Max(time, w.Camera.ShakeTime);
    }
}