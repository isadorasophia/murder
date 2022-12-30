using Bang.Components;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;

namespace Murder.Components.Graphics
{
    /// <summary>
    /// This component makes sure that any aseprite will render as a 3-slice instead,
    /// as specified.
    /// </summary>
    [Requires(typeof(AsepriteComponent))]
    public readonly struct ThreeSliceComponent : IComponent
    {
        [Tooltip("Size that will stretch according to the orientation.")]
        public readonly int Size = 1;

        [Tooltip("This is the size of the image which will be cropped to be stretched over.")]
        public readonly Rectangle CoreSliceRectangle = Rectangle.One;

        public readonly Orientation Orientation = Orientation.Horizontal;

        public ThreeSliceComponent() { }
    }
}
