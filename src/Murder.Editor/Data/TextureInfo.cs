using Murder.Core.Geometry;

namespace Murder.Editor.Data
{
    /// <summary>
    /// Represents a Texture in an atlas
    /// </summary>
    public class TextureInfo
    {
        /// <summary>
        /// Path of the source texture on disk
        /// </summary>
        public string Source = String.Empty;

        /// <summary>
        /// Final size in Pixels
        /// </summary>
        public Point SliceSize;

        /// <summary>
        /// Cropped bounds of the texture, in relation to the top left of the original size
        /// </summary>
        public IntRectangle CroppedBounds;

        public IntRectangle TrimArea;

        /// <summary>
        /// The frame number of this animation
        /// </summary>
        public int Frame;

        /// <summary>
        /// The layer index of this image. Use -1 if flattening the image is intended.
        /// </summary>
        public int Layer = -1;

        /// <summary>
        /// The current slice of this image. If no slices are present 0 is the full image.
        /// </summary>
        //public int Slice;

        /// <summary>
        /// The name of the slice to append at the end of the file.
        /// </summary>
        public string SliceName = string.Empty;

        /// <summary>
        /// If this texture has more than one slice
        /// </summary>
        public bool HasSlices = false;

        /// <summary>
        /// The Asepritefile index
        /// </summary>
        internal int AsepriteFile;

        /// <summary>
        /// Is this part of an animation sequence?
        /// </summary>
        internal bool IsAnimation;

        /// <summary>
        /// Is this part of a layered file?
        /// </summary>
        internal bool HasLayers;

        /// <summary>
        /// AtlasId of the layer
        /// </summary>
        public string LayerName = String.Empty;
    }
}