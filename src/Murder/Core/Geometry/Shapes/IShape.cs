namespace Murder.Core.Geometry
{
    /// <summary>
    /// An IShape is a component which will be applied physics properties for collision.
    /// </summary>
    public interface IShape
    {
        public Rectangle GetBoundingBox();
        public PolygonShape GetPolygon();
    }
}