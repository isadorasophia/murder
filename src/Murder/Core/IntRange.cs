using Murder.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components
{
    public readonly struct IntRange
    {
        public static readonly IntRange Zero = new IntRange(0, 0);

        public readonly int Start;

        public readonly int End;

        public IntRange(int single) => (Start, End) = (single, single);
        public IntRange(int start, int end) => (Start, End) = (start, end);

        public bool Contains(float v) => v >= Start && v <= End;
        public bool Contains(int v) => v >= Start && v <= End;

        public float GetRandom() => GetRandom(Game.Random);
        public int GetRandom(Random random)
        {
            return (int)random.NextInt64(Start, End);
        }
        public float GetRandomFloat(Random random)
        {
            return random.Next(Start, End);
        }

        public float Get(float progress)
        {
            return Calculator.Lerp(Start, End, progress);
        }
    }
}
