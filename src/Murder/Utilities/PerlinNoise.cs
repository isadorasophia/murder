using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Utilities;
public static class PerlinNoise
{
    // Permutation table. This is just a random jumble of all 8-bit numbers,
    // repeated twice to avoid wrapping the index at 255 for each lookup.
    // This needs to be exactly the same for all instances on all platforms,
    // so it's easiest to just keep it as static explicit data.
    // This also removes the need for any initial randomization of the data.
    private static readonly int[] _permutation = {
        151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7,
        225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247,
        120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
        88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134,
        139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220,
        105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80,
        73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
        164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38,
        147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189,
        28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101,
        155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232,
        178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12,
        191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181,
        199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236,
        205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
    };

    // To remove the need for index wrapping, double the permutation table length
    private static readonly int[] p;

    static PerlinNoise()
    {
        p = new int[512];
        for (int x = 0; x < 512; x++)
        {
            p[x] = _permutation[x % 256];
        }
    }

    // Compute Perlin noise at coordinates x, y, z
    public static float Noise(float x, float y, float z)
    {
        // Find unit grid cell containing point
        int X = (int)Math.Floor(x) & 255;
        int Y = (int)Math.Floor(y) & 255;
        int Z = (int)Math.Floor(z) & 255;

        // Get relative xyz coordinates of point within that cell
        x -= (int)Math.Floor(x);
        y -= (int)Math.Floor(y);
        z -= (int)Math.Floor(z);

        // Compute fade curves for each of x, y, z
        float u = Fade(x);
        float v = Fade(y);
        float w = Fade(z);

        // Hash coordinates of the 8 cube corners
        int A = p[X] + Y;
        int AA = p[A] + Z;
        int AB = p[A + 1] + Z;
        int B = p[X + 1] + Y;
        int BA = p[B] + Z;
        int BB = p[B + 1] + Z;

        // And add blended results from 8 corners of the cube
        float res = Lerp(w, Lerp(v, Lerp(u, Grad(p[AA], x, y, z),
                                       Grad(p[BA], x - 1, y, z)),
                               Lerp(u, Grad(p[AB], x, y - 1, z),
                                       Grad(p[BB], x - 1, y - 1, z))),
                       Lerp(v, Lerp(u, Grad(p[AA + 1], x, y, z - 1),
                                       Grad(p[BA + 1], x - 1, y, z - 1)),
                               Lerp(u, Grad(p[AB + 1], x, y - 1, z - 1),
                                       Grad(p[BB + 1], x - 1, y - 1, z - 1))));
        return (res + 1.0f) / 2.0f; // Normalize to range 0-1
    }

    private static float Fade(float t)
    {
        // Fade function as defined by Ken Perlin. This eases coordinate values
        // so that they will ease towards integral values. This ends up smoothing
        // the final output.
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float Lerp(float t, float a, float b)
    {
        // Linear interpolate between a and b
        return a + t * (b - a);
    }

    private static float Grad(int hash, float x, float y, float z)
    {
        // Convert low 4 bits of hash code into 12 gradient directions
        int h = hash & 15;
        float u = h < 8 ? x : y;
        float v = h < 4 ? y : h == 12 || h == 14 ? x : z;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }
}
