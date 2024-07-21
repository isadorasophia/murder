using Murder.Helpers;
using System.Numerics;

namespace Murder.Core.Sounds
{
    /// <summary>
    /// Properties 
    /// </summary>
    public struct SoundSpatialAttributes
    {
        /// <summary>
        /// Position in 2D space. Used for panning and attenuation.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Forwards orientation, must be of unit length (1.0) and perpendicular to up.
        /// </summary>
        public Direction Direction;

        /// <summary>
        /// Velocity in 2D space. Used for doppler effect.
        /// </summary>
        public Vector2 Velocity;
    }
}
