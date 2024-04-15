using Murder.Core.Geometry;

namespace Murder.Editor.Assets
{
    public readonly struct PersistStageInfo
    {
        public readonly Point Position;

        /// <summary>
        /// Size of the camera when this was saved.
        /// </summary>
        public readonly Point Size;

        public readonly int Zoom;

        public PersistStageInfo(Point position, Point size, int zoom) =>
            (Position, Size, Zoom) = (position, size, zoom);
    }
}