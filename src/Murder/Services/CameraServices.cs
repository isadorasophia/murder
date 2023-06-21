using Bang;
using Murder.Core;
using Murder.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Services;

public static class CameraServices
{
    public static void ShakeCamera(World world, float intensity, float time)
    {
        var w = (MonoWorld)world;
        w.Camera.ShakeIntensity = intensity;
        w.Camera.ShakeTime = time;
    }
}
