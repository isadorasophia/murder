using Bang;
using Bang.Entities;
using Murder.Components;
using Murder.Core;
using Murder.Utilities;

namespace Murder.Services;

public static class CameraServices
{
    public static void AddSecondaryTarget(World world, Entity secondaryTarget)
    {
        Entity camera = world.GetUniqueEntityCameraFollow();

        CameraFollowComponent follow =
            new CameraFollowComponent(true, secondaryTarget);

        camera.RemoveIdTarget();
        camera.SetCameraFollow(follow);
    }

    public static void RemoveSecondaryTarget(World world)
    {
        Entity camera = world.GetUniqueEntityCameraFollow();

        CameraFollowComponent follow =
            new CameraFollowComponent(true);

        camera.RemoveIdTarget();
        camera.SetCameraFollow(follow);
    }

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

    public static void BumpCamera(World world, float intensity)
    {
        var w = (MonoWorld)world;
        w.Camera.Bump(intensity);
    }
}