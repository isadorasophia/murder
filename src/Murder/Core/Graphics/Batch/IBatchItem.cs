using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Graphics;

namespace Murder.Core.Graphics
{
    public interface IBatchItem
    {
        VertexInfo[] VertexData { get; }
        int[] IndexData { get; }
        Texture2D? Texture { get; }

        void Clear();
    }
}