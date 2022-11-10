namespace Murder.Core.Geometry
{
    /// <summary>
    /// Implements a matrix within our engine. It can be converted to other matrix data types.
    /// </summary>
    public struct Matrix : IEquatable<Matrix>
    {
        /// <summary>
        /// A first row and first column value.
        /// </summary>
        public float M11;

        /// <summary>
        /// A first row and second column value.
        /// </summary>
        public float M12;
        
        /// <summary>
        /// A first row and third column value.
        /// </summary>
        public float M13;

        /// <summary>
        /// A first row and fourth column value.
        /// </summary>
        public float M14;

        /// <summary>
        /// A second row and first column value.
        /// </summary>
        public float M21;

        /// <summary>
        /// A second row and second column value.
        /// </summary>
        public float M22;

        /// <summary>
        /// A second row and third column value.
        /// </summary>
        public float M23;

        /// <summary>
        /// A second row and fourth column value.
        /// </summary>
        public float M24;

        /// <summary>
        /// A third row and first column value.
        /// </summary>
        public float M31;

        /// <summary>
        /// A third row and second column value.
        /// </summary>
        public float M32;

        /// <summary>
        /// A third row and third column value.
        /// </summary>
        public float M33;

        /// <summary>
        /// A third row and fourth column value.
        /// </summary>
        public float M34;

        /// <summary>
        /// A fourth row and first column value.
        /// </summary>
        public float M41;

        /// <summary>
        /// A fourth row and second column value.
        /// </summary>
        public float M42;

        /// <summary>
        /// A fourth row and third column value.
        /// </summary>
        public float M43;

        /// <summary>
        /// A fourth row and fourth column value.
        /// </summary>
        public float M44;
        
        public bool Equals(Matrix other)
        {
            return M11 == other.M11 &&
                M22 == other.M22 &&
                M33 == other.M33 &&
                M44 == other.M44 &&
                M12 == other.M12 &&
                M13 == other.M13 &&
                M14 == other.M14 &&
                M21 == other.M21 &&
                M23 == other.M23 &&
                M24 == other.M24 &&
                M31 == other.M31 &&
                M32 == other.M32 &&
                M34 == other.M34 &&
                M41 == other.M41 &&
                M42 == other.M42 &&
                M43 == other.M43;
        }

        public Microsoft.Xna.Framework.Matrix ToXnaMatrix() =>
            new(M11, M12, M13, M14,
                M21, M22, M23, M24,
                M31, M32, M33, M34,
                M41, M42, M43, M44);
    }
}
