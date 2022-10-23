namespace Murder.Core.Graphics
{
    /// <summary>
    /// How SpriteBatch rendering should behave.
    /// </summary>
    public enum BatchMode
    {
        /// <summary>
        /// Standard way. Will respect SpriteBatch.Draw*() call order, ignoring Layer Depth, but still trying to group as many batches as possible to reduce draw calls.
        /// </summary>
        DrawOrder = 0,

        /// <summary>
        /// Sort batches by Layer Depth using ascending order, but still trying to group as many batches as possible to reduce draw calls.
        /// </summary>
        DepthSortAscending,

        /// <summary>
        /// Sort batches by Layer Depth using descending order, but still trying to group as many batches as possible to reduce draw calls.
        /// </summary>
        DepthSortDescending,

        /// <summary>
        /// By leaving depth sorting job to Canvas' depth buffer, we can try to minimize Texture/Shader swap and reduce draw calls.
        /// This mode is to be used in cooperation with a Canvas with enabled depth buffer.
        /// </summary>
        //DepthBuffer,
        //DepthBufferDescending,

        /// <summary>
        /// Every SpriteBatch.Draw*() will result in an isolate draw call. No batching will be made, so be careful.
        /// </summary>
        Immediate
    }

    internal static class BatchModeComparer
    {
        internal class DepthAscending : IComparer<IBatchItem>
        {
            public int Compare(IBatchItem? itemA, IBatchItem? itemB)
            {
                if (itemA == null || itemB == null)
                {
                    return itemA == itemB ? 1 : 0;
                }

                return itemA.VertexData[0].Position.Z.CompareTo(itemB.VertexData[0].Position.Z);
            }
        }

        internal class DepthDescending : IComparer<IBatchItem>
        {
            public int Compare(IBatchItem? itemA, IBatchItem? itemB)
            {
                if (itemA == null || itemB == null)
                {
                    return itemA == itemB ? 1 : 0;
                }

                return itemB.VertexData[0].Position.Z.CompareTo(itemA.VertexData[0].Position.Z);
            }
        }

        //internal class DepthBuffer : IComparer<IBatchItem>
        //{
        //    public int Compare(IBatchItem itemA, IBatchItem itemB)
        //    {
        //        if (itemA.Shader == itemB.Shader)
        //        {
        //            return itemA.VertexData[0].Position.Z.CompareTo(itemB.VertexData[0].Position.Z);
        //        }
        //        else if (itemA.Shader == null)
        //        {
        //            return -1;
        //        }
        //        else if (itemB.Shader == null)
        //        {
        //            return 1;
        //        }

        //        return itemA.Shader.Id.CompareTo(itemB.Shader.Id);
        //    }
        //}

        //internal class DepthBufferDescending : IComparer<IBatchItem>
        //{
        //    public int Compare(IBatchItem itemA, IBatchItem itemB)
        //    {
        //        if (itemA.Shader == itemB.Shader)
        //        {
        //            return itemB.VertexData[0].Position.Z.CompareTo(itemA.VertexData[0].Position.Z);
        //        }
        //        else if (itemA.Shader == null)
        //        {
        //            return -1;
        //        }
        //        else if (itemB.Shader == null)
        //        {
        //            return 1;
        //        }

        //        return itemA.Shader.Id.CompareTo(itemB.Shader.Id);
        //    }
        //}
    }
}